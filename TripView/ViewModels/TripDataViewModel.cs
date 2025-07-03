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
using TripView.ViewModels.Charts;
using TripView.ViewModels.Messages;


namespace TripView.ViewModels
{
    public partial class LeafSpyImportConfigViewModel : ObservableObject 
    {
        private readonly ILogger<LeafSpyImportConfigViewModel> _logger;

        [ObservableProperty]
        public LeafspyImportConfiguration leafspyImportConfiguration;

        public LeafSpyImportConfigViewModel(ILogger<LeafSpyImportConfigViewModel> logger, IOptionsMonitor<LeafspyImportConfiguration> lsConfig)
        {
            _logger = logger;
            LeafspyImportConfiguration = lsConfig.CurrentValue;
        }
    }
    public partial class TripDataViewModel : ObservableObject
    {
        private readonly ILogger<TripDataViewModel> _logger;
        private readonly StartupConfiguration _configuration;
        private readonly ColorConfiguration _colorConfig;
        public readonly LeafspyImportConfiguration _leafSpyImportConfig; //FIX THIS (public needed for KML Export)

        [ObservableProperty]
        private ObservableCollection<BaseChartViewModel> charts;

        [ObservableProperty]
        private BaseChartViewModel activeChart;

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

        public const string FeatureRecordKeyName = "tripdata";

        [ObservableProperty]
        private string? carImageAsBase64;

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

        public string TripMilesDistance
        {
            get
            {
                if (Events.Count > 0)
                {
                    var start = Events.First().Odokm;
                    var end = Events.Last().Odokm;
                    var KILOMETERS_TO_MILES = 0.621371;
                    return $"{(end-start) * KILOMETERS_TO_MILES:N2} miles (Odo)";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

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

            Charts.Add(new TemperatureChartViewModel(colorConfig, chartConfig));
            Charts.Add(new ElevationChartViewModel(colorConfig, chartConfig));
            Charts.Add(new SpeedChartViewModel(colorConfig, chartConfig));
            Charts.Add(new SocChartViewModel(colorConfig, chartConfig));
            Charts.Add(new AccPowerUsageChartViewModel(colorConfig, chartConfig));
            Charts.Add(new GpsAccuracyChartViewModel(colorConfig, chartConfig));
            Charts.Add(new TirePressureChartViewModel(colorConfig, chartConfig));
            Charts.Add(new GidsChartViewModel(colorConfig, chartConfig));
            Charts.Add(new HVoltChartViewModel(colorConfig, chartConfig));
            Charts.Add(new GearPositionChartViewModel(colorConfig, chartConfig));
            //Charts.Add(new CellPairHeatMapViewData(colorConfig, lsConfig));
            ActiveChart = Charts.First(); //Set the active model to the first one as a sane default. we should read this from the config.
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
            BuildChartMenuItems(); //TODO: Fix this.
        }

        public void BuildChartMenuItems()
        {
            ChartMenuItems.Clear();
            foreach (var item in Charts){
                ChartMenuItems.Add(new MenuItemViewModel(item.Name ?? item.GetType().Name, ActiveChart == item, ChartMenuItemClickCommand, null));
            }
        }

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

        partial void OnSelectedEventChanged(TripLog? oldValue, TripLog? newValue)
        {
            MPoint? thePoint = null;
            foreach (var item in Points.Features)
            {
                if (item.Styles.First() is SymbolStyle s)
                {
                    if (item[FeatureRecordKeyName] is TripLog log)
                    {
                        if (log.DateTime == newValue?.DateTime)
                        {
                            if(item.Extent != null)
                            {
                                thePoint = new MPoint(item.Extent.Centroid.X, item.Extent.Centroid.Y);
                            }
                            s.Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.FireBrick);
                            s.SymbolType = SymbolType.Triangle;
                            s.SymbolScale = 1.0;
                        }
                        else
                        {
                            s.Fill = new Mapsui.Styles.Brush(Utilities.GetColorFromString(_colorConfig.MapRouteColor, SkiaSharp.SKColors.Purple).ToMapsui());
                            s.SymbolType = SymbolType.Ellipse;
                            s.SymbolScale = 0.5;
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
                var firstRecordProcessed = false;
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
                    if (!firstRecordProcessed)
                    {
                        var carStyle = new ImageStyle
                        {
                            Image = $"base64-content://{CarImageAsBase64}",
                            SymbolScale = 0.05,
                            SymbolRotation = records.Count > 1 ? Utilities.CalculateHeading(record.GpsPhoneCoordinates, records[1].GpsPhoneCoordinates) : 0,
                        };
                        vehPoint.Styles.Add(carStyle);
                        firstRecordProcessed = true;
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
                    //Add GPS Uncert
                    //
                    var acc = record.GPSStatus.GetAccuracy();
                    if (acc == 0)
                        continue;

                    var geoFactory = new NetTopologySuite.Geometries.GeometryFactory();
                    var circle = geoFactory.CreatePoint(record.GpsPhoneCoordinates.ToMPoint().ToCoordinate()).Buffer(acc);

                    var feature = new Mapsui.Nts.GeometryFeature() { Geometry = circle };
                    feature[FeatureRecordKeyName] = record;
                    feature.Styles.Add(new VectorStyle()
                    {
                        Fill = new Mapsui.Styles.Brush(Utilities.GetColorFromString(_colorConfig.GpsAccuracyColor, Mapsui.Styles.Color.FromArgb(64, 0, 120, 215).ToSkia()).ToMapsui())
                    });
                    gpsAccFeatures.Add(feature);
                }

                foreach (var chart in Charts)
                {
                    chart.LoadData(Events, _configuration.MinutesBetweenTrips);
                }

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