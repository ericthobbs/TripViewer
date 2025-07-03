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
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LeafSpy.DataParser;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Extensions.Cache;
using Mapsui.Limiting;
using Mapsui.Projections;
using Mapsui.Rendering.Skia;
using Mapsui.Tiling.Layers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TripView.ViewModels;
using TripView.ViewModels.Messages;
namespace TripView
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IRecipient<NewDataLoaded>, IRecipient<ChartPointPressed>, IRecipient<CenterOnPointMessage>, IRecipient<SaveChartAsImageMessage>
    {
        private readonly string _cacheDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Eric Hobbs", "TripView");

        public TripDataViewModel CurrentData { get; set;}

        private readonly StartupConfiguration _startupConfig;
        private readonly ILogger<MainWindow> _logger;
        private EventViewerWindow? _logEventViewer;

        public static readonly DependencyProperty MapLayersMenuItemsProperty =
        DependencyProperty.Register(
            nameof(MapLayersMenuItems),
            typeof(ObservableCollection<MenuItemViewModel>),
            typeof(MainWindow),
            new PropertyMetadata(new ObservableCollection<MenuItemViewModel>()));

        public ObservableCollection<MenuItemViewModel> MapLayersMenuItems
        {
            get => (ObservableCollection<MenuItemViewModel>)GetValue(MapLayersMenuItemsProperty);
            set => SetValue(MapLayersMenuItemsProperty, value);
        }

        public static readonly DependencyProperty MapWidgetsMenuItemsProperty =
        DependencyProperty.Register(
            nameof(MapWidgetsMenuItems),
            typeof(ObservableCollection<MenuItemViewModel>),
            typeof(MainWindow),
            new PropertyMetadata(new ObservableCollection<MenuItemViewModel>()));

        public ObservableCollection<MenuItemViewModel> MapWidgetsMenuItems
        {
            get => (ObservableCollection<MenuItemViewModel>)GetValue(MapWidgetsMenuItemsProperty);
            set => SetValue(MapWidgetsMenuItemsProperty, value);
        }

        public static readonly RoutedUICommand ShowEventWindowCommand = new("_Show Event Window...", "ShowEventWindowCommand", typeof(MainWindow));
        public static readonly RoutedUICommand ExportToKmlFileCommand = new("E_xport to KML File...", "ExportToKmlCommand", typeof(MainWindow));
        public static readonly RoutedUICommand ExitCommand = new("Exit TripView", "ExitCommand", typeof(MainWindow));
        public static readonly RoutedUICommand ShowAboutCommand = new("_About...", "ShowAboutCommand", typeof(MainWindow));
        public static readonly RoutedUICommand SaveMapAsImageCommand = new("Save as Image...", "SaveMapAsImageCommand", typeof(MainWindow));

        public MainWindow(TripDataViewModel vm, ILogger<MainWindow> logger, IOptions<StartupConfiguration> startupConfigOptions)
        {
            _startupConfig = startupConfigOptions.Value;
            _logger = logger;
            DataContext = CurrentData = vm;

            var resource = System.Windows.Application.Current.Resources["GenericEVWhite"];
            if (resource is BitmapImage bi)
            {
                var resourceInfo = System.Windows.Application.GetResourceStream(bi.UriSource);
                CurrentData.CarImageAsBase64 = Utilities.StreamToBase64(resourceInfo.Stream);
            }
            InitializeComponent();

            Mapsui.Logging.Logger.LogDelegate += (level, message, ex) => {
                var mapsuiPrefix = "[MAPSUI]";
                if (level == Mapsui.Logging.LogLevel.Error)
                    _logger.LogError(ex, "{mapsuiPrefix} {message}", mapsuiPrefix, message);
                else if (level == Mapsui.Logging.LogLevel.Warning)
                    _logger.LogWarning(ex, "{mapsuiPrefix} {message}", mapsuiPrefix, message);
                else if (level == Mapsui.Logging.LogLevel.Information)
                    _logger.LogInformation(ex, "{mapsuiPrefix} {message}", mapsuiPrefix, message);
                else if (level == Mapsui.Logging.LogLevel.Debug)
                    _logger.LogDebug(ex, "{mapsuiPrefix} {message}", mapsuiPrefix, message);
                else if (level == Mapsui.Logging.LogLevel.Trace)
                    _logger.LogTrace(ex, "{mapsuiPrefix} {message}", mapsuiPrefix, message);
            };

            TripMap.MapTapped += TripMap_MapTapped;

            //TripMap.Map.Navigator.ViewportChanged += (s, e) =>
            //{
                //_logger.LogDebug("map viewport changed: {e.PropertyName}", e.PropertyName);
            //};

            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        #region Command Handlers

        public void ShowEventWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_logEventViewer == null || !_logEventViewer.IsLoaded)
            {
                _logEventViewer = new();
                _logEventViewer.DataContext = DataContext;
                _logEventViewer.Owner = System.Windows.Application.Current.MainWindow;
            }
            if (_logEventViewer.IsVisible)
                _logEventViewer.Hide();
            else
                _logEventViewer.Show();
        }

        private void ShowEventWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentData.Events.Count > 0;
        }

        private void ExportToKmlFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentData.Events.Count == 0)
            {
                System.Windows.MessageBox.Show(this, $"You must open a file before you can export it.", "Export to KML");
                e.Handled = true;
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save CSV as KML...",
                Filter = "KML file (*.kml)|*.kml|All Files (*.*)|*.*",
                DefaultExt = ".kml"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                _logger.LogDebug("Selected file: {selectedFile}", selectedFile);
                if (System.IO.File.Exists(selectedFile))
                {
                    _logger.LogInformation("Selected file already exists. Overwriting.");
                }
                try
                {
                    LeafSpyKMLExporter.LeafSpyKmlExporter.ExportToKml(CurrentData._leafSpyImportConfig, CurrentData.FileName, selectedFile, CurrentData.Events.First().DateTime.ToString("yyMMdd"));
                    System.Windows.MessageBox.Show(this, $"Success. Saved to {selectedFile}.", "Export to KML");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(this, $"Failed to export KML to {selectedFile}\nError:\n{ex.Message}", "Export to KML file");
                }
            }
        }

        private void ExportToKmlFileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentData.Events.Count > 0;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowAboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void ShowAboutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void AppCommandOpen_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ActiveChart.IsEnabled = false; //Works around a crash while the collection is being modified

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a file",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false,
                ReadOnlyChecked = true,
                ShowReadOnly = true,
                DefaultExt = ".csv"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                _logger.LogDebug("Selected file: {FileName}", selectedFile);
                if (System.IO.File.Exists(selectedFile))
                {
                    try
                    {
                        await CurrentData.LoadLeafSpyLogFile(selectedFile);
                    }
                    catch (CsvHelper.HeaderValidationException ex) //This should be handled better by the VM, but for now lets catch it
                    {
                        _logger.LogError(ex, "CSV Header Validation Error: {Message}", ex.Message);
                        System.Windows.MessageBox.Show($"Unable to process the file, please choose another.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AppCommandHelp_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.badpointer.net/",
                UseShellExecute = true,
            });
        }

        private void SaveMapAsImageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveMapAsImageCommand_Executed(object sender, RoutedEventArgs e)
        {
            var bitmap = new MapRenderer().RenderToBitmapStream(TripMap.Map.Navigator.Viewport, TripMap.Map.Layers, TripMap.Map.BackColor, renderFormat: Mapsui.Rendering.RenderFormat.Png);

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Map as Image...",
                Filter = "PNG Image files (*.png)|*.png|All Files (*.*)|*.*",
                DefaultExt = ".png"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                _logger.LogDebug("Selected file: {selectedFile}", selectedFile);
                if (System.IO.File.Exists(selectedFile))
                {
                    _logger.LogInformation("Selected file already exists. Overwriting.");
                }
                try
                {
                    using var file = System.IO.File.OpenWrite(selectedFile);
                    bitmap.Seek(0, System.IO.SeekOrigin.Begin);
                    bitmap.CopyTo(file);
                    file.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(this, $"Failed to save Image to {selectedFile}\nError:\n{ex.Message}", "Save Map Image");
                }
            }
        }

        #endregion

        private void BuildMapLayersMenu()
        {
            MapLayersMenuItems.Clear();
            foreach(var layer in TripMap.Map.Layers)
            {
                MapLayersMenuItems.Add(new MenuItemViewModel(layer.Name, layer.Enabled, new RelayCommand<MenuItemViewModel>((ctx) => {
                    if (ctx == null) {
                        _logger.LogError("No menu item passed to BuildMapLayersMenu.");
                        return;
                    }

                    var layer = TripMap.Map.Layers.First(l => l.Name == ctx.Header);
                    if(layer == null)
                    {
                        _logger.LogError("{ctx.Header} layer not found.", ctx.Header);
                        return;
                    }

                    if (ctx.IsChecked.HasValue)
                    {
                        layer.Enabled = !ctx.IsChecked.Value;
                        ctx.IsChecked = !ctx.IsChecked.Value;
                        TripMap.Map.Refresh();
                    }
                }) , null));
            }
        }

        private void BuildWidgetsMenu()
        {
            MapWidgetsMenuItems.Clear();
            foreach(var widget in TripMap.Map.Widgets)
            {
                string widgetName = widget.GetType().Name;
                if(widgetName.EndsWith("Widget"))
                {
                    var idx = widgetName.IndexOf("Widget");
                    widgetName = widgetName[..idx];
                }

                MapWidgetsMenuItems.Add(new MenuItemViewModel(widgetName, widget.Enabled, new RelayCommand<MenuItemViewModel>((ctx) => {
                    if (ctx == null)
                    {
                        _logger.LogError("No menu item passed to BuildWidgetsMenu.");
                        return;
                    }

                    var w = TripMap.Map.Widgets.FirstOrDefault(w => w.ToString()?.Contains(ctx.Header) == true);
                    if(w == null)
                    {
                        _logger.LogError("Widget {ctx.Header} not found.", ctx.Header);
                        return;
                    }

                    if(ctx.IsChecked.HasValue)
                    {
                        w.Enabled = !ctx.IsChecked.Value;
                        ctx.IsChecked = !ctx.IsChecked.Value;
                        TripMap.Map.Refresh();
                    }
                } ), null));
            }
        }

        private void TripMap_MapTapped(object? sender, MapEventArgs e)
        {
            var feature = e.GetMapInfo(e.Map.Layers.Where(l => l.Name == CurrentData.Points.Name)).Feature;

            if (feature != null && feature[TripDataViewModel.FeatureRecordKeyName] is TripLog)
            {
                CurrentData.SelectedEvent = feature[TripDataViewModel.FeatureRecordKeyName] as TripLog;
            }
            else
            {
                if (CurrentData.SelectedEvent != null)
                {
                    CurrentData.SelectedEvent = null;
                }
            }
        }

        public void Receive(NewDataLoaded message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                var minX = Math.Min(message.Value.Item1.X, message.Value.Item2.X);
                var minY = Math.Min(message.Value.Item1.Y, message.Value.Item2.Y);
                var maxX = Math.Max(message.Value.Item1.X, message.Value.Item2.X);
                var maxY = Math.Max(message.Value.Item1.Y, message.Value.Item2.Y);

                TripMap.Map.Navigator.ZoomToBox(new MRect(minX, minY, maxX, maxY), MBoxFit.FitWidth, _startupConfig.ZoomTimeInSeconds * 1000);
                ActiveChart.IsEnabled = true; //Work around a collection modified exception inside of the LiveCharts2 control.
                BuildMapLayersMenu();
            });
        }

        public void Receive(ChartPointPressed message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                CurrentData.SelectedEvent = CurrentData.Events.Where(e => e.DateTime == message.Value).FirstOrDefault();
            });
        }

        public void Receive(CenterOnPointMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                TripMap.Map.Navigator.CenterOn(message.Value);
                TripMap.Refresh();
            });
        }

        public void Receive(SaveChartAsImageMessage message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                if (message.Value.SaveAs == SaveAs.File)
                {
                    var skChart = new SKCartesianChart(ActiveChart) { 
                        Width = message.Value.Width == 0 ? (int)ActiveChart.Width : message.Value.Width, 
                        Height = message.Value.Height == 0 ? (int)ActiveChart.Height : message.Value.Width, 
                    };
                    var saveDlg = new SaveFileDialog()
                    {
                        Title = "Save Chart as Image...",
                        Filter = "PNG Image files (*.png)|*.png|JPEG Image files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                        DefaultExt = message.Value.SaveAsFormat.ToExtension()
                    };

                    if(saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        skChart.SaveImage(saveDlg.FileName, message.Value.SaveAsFormat.ToSkiaImageFormat());
                    }
                }
                else
                {
                    using var ms = new System.IO.MemoryStream();
                    var skChart = new SKCartesianChart(ActiveChart) {
                        Width = message.Value.Width == 0 ? (int)ActiveChart.Width : message.Value.Width,
                        Height = message.Value.Height == 0 ? (int)ActiveChart.Height : message.Value.Width,
                    };
                    skChart.SaveImage(ms, message.Value.SaveAsFormat.ToSkiaImageFormat());

                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = ms;
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    bmp.Freeze();
                    System.Windows.Clipboard.SetImage(bmp);
                }
            });
        }

        private void ConfigureForOpenStreetMaps(Map map)
        {
            IPersistentCache<byte[]> cache;
            var cacheRoot = System.IO.Path.Combine(_cacheDir, "OpenStreetMaps");
            if (_startupConfig.UseSqlAsCache)
            {
                cache = new SqlitePersistentCache(cacheRoot);
            }
            else
            {
                cache = new FileCache(cacheRoot, "png");
            }
            var tileSource = new HttpTileSource(
                new GlobalSphericalMercator(),
                _startupConfig.OpenStreetMapUrl,
                name: "OpenStreetMap",
                persistentCache: cache ?? null,
                attribution: new BruTile.Attribution("© OpenStreetMap contributors", "https://openstreetmap.org/copyright"));

            var tileLayer = new TileLayer(tileSource) { Name = "OpenStreetMap" };
            map.Layers.Add(tileLayer);
        }

        private void TripMap_Loaded(object sender, RoutedEventArgs e)
        {
            var map = TripMap.Map;

            ConfigureForOpenStreetMaps(map);

            map.Navigator.Limiter = new ViewportLimiterKeepWithinExtent();

            map.Navigator.CenterOnAndZoomTo(
                SphericalMercator.FromLonLat(
                _startupConfig.InitialPositionLongitude,
                _startupConfig.InitialPositionLatitude).ToMPoint(), 
                map.Navigator.Resolutions[_startupConfig.InitialZoomLevel], _startupConfig.ZoomTimeInSeconds * 1000);

            map.Layers.Add(CurrentData.Points);
            map.Layers.Add(CurrentData.GpsAccLayer);

            //Add only the widgets we want.
            map.Widgets.Clear();
            map.Widgets.Add(new Mapsui.Widgets.ButtonWidgets.ZoomInOutWidget());
            map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map));
            map.Widgets.Add(new Mapsui.Widgets.InfoWidgets.MouseCoordinatesWidget());
#if DEBUG
            map.Widgets.Add(new Mapsui.Widgets.InfoWidgets.MapInfoWidget(map, l => l.Name == CurrentData.Points.Name));
            map.Widgets.Add(new Mapsui.Widgets.InfoWidgets.PerformanceWidget(map.Performance));
#endif
            BuildMapLayersMenu();
            BuildWidgetsMenu();
        }



        private void Chart_ContextMenuItemOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            CurrentData?.ActiveChart?.BuildContextMenuItems();
        }
    }
}