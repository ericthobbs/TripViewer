/**
 * MIT License
 * 
 * Copyright (c) 2025 Eric Hobbs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LeafSpy.DataParser;
using LeafSpy.DataParser.Parsers;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Styles;
using Mapsui.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;
using System.Collections.ObjectModel;
using TripView.Configuration;
using TripView.ViewModels.Charts;
using TripView.ViewModels.Messages;
using WinRT;


namespace TripView.ViewModels
{
    /// <summary>
    /// Represents a view model for managing trip-related data, charts, and configuration settings.
    /// </summary>
    /// <remarks>This class provides functionality for loading and managing trip data, including charts,
    /// events, and map layers. It also supports operations such as resetting trip data, building chart menu items, and
    /// handling event changes. The view model integrates with various configurations, such as startup, color, and
    /// LeafSpy import settings.</remarks>
    public partial class TripDataViewModel : ObservableObject
    {
        private readonly ILogger<TripDataViewModel> _logger;
        private readonly StartupConfiguration _configuration;
        private readonly ColorConfiguration _colorConfig;
        public readonly LeafspyImportConfiguration _leafSpyImportConfig; //FIX THIS (public needed for KML Export)

        [ObservableProperty]
        private ObservableCollection<BaseChartViewModel> charts;

        [ObservableProperty]
        private BaseChartViewModel? activeChart;

        [ObservableProperty]
        private ObservableCollection<MenuItemViewModel> chartMenuItems;

        [ObservableProperty]
        private ObservableCollection<MenuItemViewModel> mapLayersMenuItems;

        [ObservableProperty]
        private MemoryLayer points = new();

        [ObservableProperty]
        private MemoryLayer gpsAccLayer = new();

        public ObservableCollection<TripLog> Events { get; set; } = [];

        [ObservableProperty]
        private string title = "TripView: No Data Loaded";

        [ObservableProperty]
        private string? carImageAsBase64;

        [ObservableProperty]
        private string? routeStartImageAsBase64;

        [ObservableProperty]
        private string? routeEndImageAsBase64;

        /// <summary>
        /// Trip Start Date
        /// </summary>
        public string StartDate
        {
            get
            {
                if (Events.Count > 0)
                {
                    return DateOnly.FromDateTime(Events.First().DateTime).ToShortDateString();
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Trip Start Time
        /// </summary>
        public string StartTime
        {
            get
            {
                if (Events.Count > 0)
                {
                    return TimeOnly.FromDateTime(Events.First().DateTime).ToShortTimeString();
                }
                else
                    return string.Empty;
            }
        }


        /// <summary>
        /// Trip End Time
        /// </summary>
        public string EndTime
        {
            get
            {
                if (Events.Count > 0)
                {
                    return TimeOnly.FromDateTime(Events.Last().DateTime).ToShortTimeString();
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the total distance of the trip, calculated using the Haversine formula, in miles.
        /// </summary>
        /// <remarks>The distance is calculated based on the first and last non-zero GPS coordinates in
        /// the trip's event list. The Haversine formula is used to compute the great-circle distance between two points
        /// on the Earth's surface.</remarks>
        public string TripHaversineDistance
        {
            get
            {
                if (Events.Count > 0)
                {
                    var start = Events.First(e => !e.GpsPhoneCoordinates.IsZero()).GpsPhoneCoordinates;
                    var end = Events.Last(e => !e.GpsPhoneCoordinates.IsZero()).GpsPhoneCoordinates;
                    var distance = Haversine.Distance(start.Longitude.ToDecimalDegrees(), start.Latitude.ToDecimalDegrees(), end.Longitude.ToDecimalDegrees(), end.Latitude.ToDecimalDegrees());
                    var KILOMETERS_TO_MILES = 0.621371;
                    return $"{distance * KILOMETERS_TO_MILES:N2} miles (Haversine)";
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the total trip distance in miles, calculated from the odometer readings of the first and last events.
        /// </summary>
        public string TripMilesDistance
        {
            get
            {
                if (Events.Count > 0)
                {
                    var start = Events.First().Odometer;
                    var end = Events.Last().Odometer;
                    return $"{end-start}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the Vehicle Identification Number (VIN) associated with the first event in the collection.
        /// </summary>
        public string VIN
        {
            get
            {
                if (Events.Count > 0)
                {
                    return Events.First().VIN;
                }
                else
                    return string.Empty;
            }
        }

        [ObservableProperty]
        private string fileName = string.Empty;

        [ObservableProperty]
        private TripLog? selectedEvent;

        public const string FeatureRecordKeyName = "tripdata";

        public const string FeatureHeadingToNextPoint = "heading";

        /// <summary>
        /// Initializes a new instance of the <see cref="TripDataViewModel"/> class, which manages trip-related data,
        /// charts, and configuration settings.
        /// </summary>
        /// <remarks>This constructor initializes the view model with default chart configurations, menu
        /// items, and event handlers.  It also sets up a collection of predefined charts, such as temperature, power
        /// usage, and vehicle data charts,  and subscribes to collection change events to update relevant properties
        /// when the event collection is reset.</remarks>
        /// <param name="logger">The logger instance used for logging diagnostic and operational information.</param>
        /// <param name="startupConfigOptions">The options monitor for accessing the application's startup configuration settings.</param>
        /// <param name="colorConfig">The options monitor for accessing color configuration settings used in charts and UI elements.</param>
        /// <param name="lsConfig">The options monitor for accessing LeafSpy import configuration settings.</param>
        /// <param name="chartConfig">The options monitor for accessing chart configuration settings.</param>
        public TripDataViewModel(
                    ILogger<TripDataViewModel> logger,
                    IOptionsMonitor<StartupConfiguration> startupConfigOptions,
                    IOptionsMonitor<ColorConfiguration> colorConfig,
                    IOptionsMonitor<LeafspyImportConfiguration> lsConfig,
                    IOptionsMonitor<ChartConfiguration> chartConfig)
        {
            _logger = logger;
            _configuration = startupConfigOptions.CurrentValue;
            _colorConfig = colorConfig.CurrentValue;
            _leafSpyImportConfig = lsConfig.CurrentValue;

            ChartMenuItems = [];
            MapLayersMenuItems = [];
            Charts = [];

            Reset();

            //Temperature
            Charts.Add(new TemperatureChartViewModel(colorConfig, chartConfig));
            Charts.Add(new TirePressureChartViewModel(colorConfig, chartConfig));

            //Power / Battery
            Charts.Add(new PowerUsageChartViewModel(colorConfig, chartConfig));
            Charts.Add(new SocChartViewModel(colorConfig, chartConfig));
            Charts.Add(new SohChartViewModel(colorConfig, chartConfig));
            Charts.Add(new RegenWhChartViewModel(colorConfig, chartConfig));
            Charts.Add(new GidsChartViewModel(colorConfig, chartConfig));
            Charts.Add(new HVoltChartViewModel(colorConfig, chartConfig));
            Charts.Add(new HVPackVoltsAmpsChartViewModel(colorConfig, chartConfig));

            //Positional data
            Charts.Add(new SpeedChartViewModel(colorConfig, chartConfig));
            Charts.Add(new ElevationChartViewModel(colorConfig, chartConfig));
            Charts.Add(new GpsAccuracyChartViewModel(colorConfig, chartConfig));

            //Vehicle data
            charts.Add(new MotorTorqueRpmChartViewModel(colorConfig, chartConfig));
            Charts.Add(new GearPositionChartViewModel(colorConfig, chartConfig));
            Charts.Add(new V12BatteryChartViewModel(colorConfig, chartConfig));

            //Charts.Add(new CellPairHeatMapViewData(colorConfig, lsConfig)); //Not functional yet
            BuildChartMenuItems();

            Events.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    OnPropertyChanged(nameof(StartDate));
                    OnPropertyChanged(nameof(StartTime));
                    OnPropertyChanged(nameof(EndTime));
                    OnPropertyChanged(nameof(TripHaversineDistance));
                    OnPropertyChanged(nameof(TripMilesDistance));
                    OnPropertyChanged(nameof(VIN));
                    SelectedEvent = null;
                }
            };

            _logger.LogDebug("TripDataViewModel created.");
        }


        [RelayCommand]
        protected void ChartMenuItemClick(MenuItemViewModel item)
        {
            ActiveChart = Charts.First(e => e.Name == item.Header);
            BuildChartMenuItems();
        }

        /// <summary>
        /// Builds and populates the collection of menu items for the available charts.
        /// </summary>
        /// <remarks>This method clears the existing menu items and creates a new menu item for each chart
        /// in the  <c>Charts</c> collection. Each menu item reflects the chart's name and indicates whether it is  the
        /// currently active chart.</remarks>
        public void BuildChartMenuItems()
        {
            ChartMenuItems.Clear();
            foreach (var item in Charts){
                ChartMenuItems.Add(new MenuItemViewModel(item.Name ?? item.GetType().Name, ActiveChart == item, ChartMenuItemClickCommand, null));
            }
        }

        /// <summary>
        /// Resets the state of the ViewModel to its default values.
        /// </summary>
        /// <remarks>This method clears all data, resets properties to their initial states, and updates
        /// associated components. After calling this method, the object will be in a state as if no data has been
        /// loaded.</remarks>
        public void Reset()
        {
            Points.Features = [];
            Points.Name = "Trip";
            Points.FeaturesWereModified();

            GpsAccLayer.Features = [];
            GpsAccLayer.Name = "Gps Accuracy";
            GpsAccLayer.FeaturesWereModified();

            Events.Clear();
            FileName = string.Empty;
            SelectedEvent = null;
            Title = "TripView: No Data Loaded";
            foreach (var item in Charts)
            {
                item.Reset();
            }
        }

        /// <summary>
        /// Handles changes to the selected trip log event and updates the associated map features accordingly.
        /// </summary>
        /// <remarks>This method updates the styles of map features to reflect the newly selected trip log
        /// event.  If a feature corresponds to the new event, it is styled with an image representing the car. 
        /// Features corresponding to the old event have the car image style removed.  Additionally, the map is notified
        /// of feature modifications and data changes, and a message is sent to center the map on a specific point if
        /// applicable.</remarks>
        /// <param name="oldValue">The previously selected <see cref="TripLog"/> instance, or <see langword="null"/> if no event was previously
        /// selected.</param>
        /// <param name="newValue">The newly selected <see cref="TripLog"/> instance, or <see langword="null"/> if no event is currently
        /// selected.</param>
        partial void OnSelectedEventChanged(TripLog? oldValue, TripLog? newValue)
        {
            MPoint? thePoint = null;
            var carBase64ImageContent = $"base64-content://{CarImageAsBase64}";
            foreach (var item in Points.Features)
            {
                //If this point doesn't contain a trip log event, then skip it
                if (item[FeatureRecordKeyName] is TripLog log)
                {
                    if (log.DateTime == newValue?.DateTime)
                    {
                        thePoint = item.Extent?.Centroid.ToPoint().ToMPoint();
                        item.Styles.Add(new ImageStyle
                        {
                            Image = carBase64ImageContent,
                            SymbolScale = 0.05,
                            SymbolRotation = item[FeatureHeadingToNextPoint] != null ? item[FeatureHeadingToNextPoint].As<double>() : 0,
                        });
                    }
                    else
                    {
                        var stylesToRemove = item.Styles
                            .OfType<ImageStyle>()
                            .Where(im => im?.Image?.Source == carBase64ImageContent)
                            .ToList();

                        foreach (var style in stylesToRemove)
                        {
                            item.Styles.Remove(style);
                        }
                    }
                }
            }
            Points.FeaturesWereModified();
            Points.DataHasChanged();
            if (thePoint != null)
            {
                _ = WeakReferenceMessenger.Default.Send(new CenterOnPointMessage(thePoint));
            }
        }

        /// <summary>
        /// Experimental Command
        /// </summary>
        [RelayCommand]
        private void JudgementCheck()
        {
            var CellsWithFailedJudgementCheck = new Dictionary<int, int>(); /* cell ID, Count */
            foreach (var e in Events)
            {
                if (e.JudgmentValue != 0)
                {
                    float judgementValue = (float)e.JudgmentValue / 1000;
                    foreach (var cp in e.GetAllCellPairs())
                    {
                        float cellValAbs = Math.Abs(cp.Item2);
                        if (cellValAbs <= judgementValue)
                        {
                            if (CellsWithFailedJudgementCheck.ContainsKey(cp.Item1))
                                CellsWithFailedJudgementCheck[cp.Item1] += 1;
                            else
                                CellsWithFailedJudgementCheck.Add(cp.Item1, 1);
                        }
                    }
                }
            }
            if(CellsWithFailedJudgementCheck.Count > 0)
            {
                var cells = string.Join("\n\t", CellsWithFailedJudgementCheck.Select(kvp => $"Cell Pair(Count):{kvp.Key}:{kvp.Value}"));
                System.Windows.MessageBox.Show($"Failed Judgement Check. Cell Count: {CellsWithFailedJudgementCheck.Keys.Count}\n{cells}");
            }
        }

        /// <summary>
        /// Loads a LeafSpy log file and processes its data.
        /// </summary>
        /// <remarks>If the specified file exists, the method resets the current state, updates the title
        /// to reflect the file name,  and attempts to load the CSV data from the file. If the file does not exist, the
        /// method does nothing.</remarks>
        /// <param name="filename">The full path to the LeafSpy log file to load. Must be a valid file path.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task LoadLeafSpyLogFile(string filename)
        {
            _logger.LogDebug("Selected file: {FileName}", filename);
            if (System.IO.File.Exists(filename))
            {
                Reset();
                Title = $"TripView: {filename}";
                try
                {
                    await LoadCSVData(filename);
                    FileName = filename;
                }
                catch (CsvHelper.HeaderValidationException ex)
                {
                    _logger.LogWarning(ex, "failed to load '{FileName}'", FileName);
                    Reset();
                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously loads and processes trip data from a CSV file.
        /// </summary>
        /// <remarks>This method parses the specified CSV file, processes the trip data, and updates
        /// various layers and properties related to the trip, such as map features, GPS accuracy, and trip statistics.
        /// It also raises property change notifications for UI updates and sends a message with the first and last
        /// valid GPS coordinates of the trip.  The method skips records with invalid GPS coordinates and applies styles
        /// to map features, such as start and end points, route points, and GPS accuracy areas. Additionally, it
        /// initializes charts with the loaded data and sets the active chart to the first one if not already set.  The
        /// GPS accuracy layer is hidden by default after processing.</remarks>
        /// <param name="fileName">The full path to the CSV file containing trip data.</param>
        /// <returns></returns>
        private async Task LoadCSVData(string fileName)
        {
            await Task.Run(() =>
            {
                LeafSpySingleTripParser parser = new(_leafSpyImportConfig);
                parser.Open(fileName);
                var records = parser.Read().ToList();
                int skippedCount = 0;

                var features = new List<IFeature>();

                var gpsAccFeatures = new List<IFeature>();
              
                var pointColor = Utilities.GetColorFromString(_colorConfig.MapRouteColor, SKColors.MediumPurple);
                PointFeature? lastPoint = null;
                foreach (var record in records)
                {
                    if (record.GpsPhoneCoordinates.IsZero())
                    {
                        skippedCount++;
                        continue;
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() => Events.Add(record));

                    //Maybe use a Linestring? https://stackoverflow.com/questions/65033623/display-linestring-and-trackpoints-with-mapsui

                    var vehPoint = new PointFeature(record.GpsPhoneCoordinates.ToMPoint());
                    vehPoint[FeatureRecordKeyName] = record;

                    if(lastPoint != null)
                    {
                        lastPoint[FeatureHeadingToNextPoint] = Utilities.CalculateHeading(
                            lastPoint[FeatureRecordKeyName].As<TripLog>().GpsPhoneCoordinates, 
                            record.GpsPhoneCoordinates);
                    }

                    if (record == Events.First())
                    {
                        var startStyle = new ImageStyle
                        {
                            Image = $"base64-content://{RouteStartImageAsBase64}",
                            SymbolScale = 0.05,
                            SymbolRotation = records.Count > 1 ? Utilities.CalculateHeading(record.GpsPhoneCoordinates, records[1].GpsPhoneCoordinates) : 0,
                        };
                        vehPoint.Styles.Add(startStyle);
                    } 
                    else
                    {
                        var pointStyle = new SymbolStyle {
                            SymbolScale = 0.4,
                            Fill = new Mapsui.Styles.Brush(pointColor.ToMapsui()),
                            Opacity = 0.4f
                        };
                        vehPoint.Styles.Add(pointStyle);
                    }
                    features.Add(vehPoint);

                    //
                    //Add GPS Accuracy layer
                    //
                    var accInMeters = record.GPSStatus.GetAccuracy();
                    if (accInMeters == 0)
                        continue;

                    var geoFactory = new NetTopologySuite.Geometries.GeometryFactory();
                    var feature = new Mapsui.Nts.GeometryFeature() { Geometry = geoFactory.CreatePoint(record.GpsPhoneCoordinates.ToMPoint().ToCoordinate()).Buffer(accInMeters) };
                    feature[FeatureRecordKeyName] = record;
                    feature.Styles.Add(new VectorStyle()
                    {
                        Fill = new Mapsui.Styles.Brush(Utilities.GetColorFromString(_colorConfig.GpsAccuracyColor, SKColors.LightBlue).ToMapsui())
                    });
                    gpsAccFeatures.Add(feature);
                    lastPoint = vehPoint;
                }

                var lastFeature = features.Last().As<PointFeature>();
                lastFeature.Styles.Clear();
                lastFeature.Styles.Add(new ImageStyle
                {
                    Image = $"base64-content://{RouteEndImageAsBase64}",
                    SymbolScale = 0.025,
                    SymbolRotation = features.Count > 1 ? Utilities.CalculateHeading(
                        features[^2][FeatureRecordKeyName].As<TripLog>().GpsPhoneCoordinates, 
                        lastFeature[FeatureRecordKeyName].As<TripLog>().GpsPhoneCoordinates) : 0,
                });

                foreach (var chart in Charts)
                {
                    chart.LoadData(Events, _configuration.MinutesBetweenTrips);
                }

                //Note: Moved this from the ctor to LoadCSVData to prevent an issue with XAxes being squished
                //ActiveChart = Charts.First(); //Set the active model to the first one as a sane default. we should read this from the config.
                ActiveChart ??= Charts.First();

                this.Points.Name = $"Trip-{System.IO.Path.GetFileNameWithoutExtension(fileName)}";
                this.Points.Style = null;
                this.Points.Features = features;
                this.Points.Enabled = true;

                this.GpsAccLayer.Name = $"Gps Accuracy-{System.IO.Path.GetFileNameWithoutExtension(fileName)}";
                this.GpsAccLayer.Style = null;
                this.GpsAccLayer.Features = gpsAccFeatures;
                this.GpsAccLayer.Enabled = false; //Hide this layer by default

                SelectedEvent = Events.First();

                OnPropertyChanged(nameof(StartDate));
                OnPropertyChanged(nameof(StartTime));
                OnPropertyChanged(nameof(EndTime));
                OnPropertyChanged(nameof(TripHaversineDistance));
                OnPropertyChanged(nameof(TripMilesDistance));
                OnPropertyChanged(nameof(VIN));

                var firstValid = records.First(c => !c.GpsPhoneCoordinates.IsZero()).GpsPhoneCoordinates;
                var lastValid = records.Last(c => !c.GpsPhoneCoordinates.IsZero()).GpsPhoneCoordinates;
                _ = WeakReferenceMessenger.Default.Send(new NewDataLoaded(new Tuple<MPoint, MPoint>(firstValid.ToMPoint(), lastValid.ToMPoint())));
                _logger.LogDebug("Skipped Processing {skippedCount} of {records.Count} records.", skippedCount, records.Count);
            });
        }
    }
}