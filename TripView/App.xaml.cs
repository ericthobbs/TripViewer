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
using LeafSpy.DataParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Reflection;
using System.Windows;
using TripView.ViewModels;
using TripView.ViewModels.Charts;

namespace TripView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            var userConfigPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Eric Hobbs", "TripView", "user-settings.json");
            var directory = System.IO.Path.GetDirectoryName(userConfigPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                try
                {
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"*** Cannot create directory for settings: {ex.Message}");
                }
            }

            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile(userConfigPath, optional: true, reloadOnChange: true);
                    config.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((context, services) =>
                {
                    // Register your services and view models here
                    services.Configure<StartupConfiguration>(context.Configuration.GetSection("Startup"));
                    services.Configure<LeafspyImportConfiguration>(context.Configuration.GetSection("LeafspyImport"));
                    services.Configure<ColorConfiguration>(context.Configuration.GetSection("Colors"));
                    services.Configure<ChartConfiguration>(context.Configuration.GetSection("ChartConfiguration"));
                    services.AddSingleton<MainWindow>();

                    services.AddTransient<TripDataViewModel>();

                    services.AddTransient<TemperatureChartViewModel>();
                    services.AddTransient<ElevationChartViewModel>();
                    services.AddTransient<SpeedChartViewModel>();
                    services.AddTransient<AccPowerUsageChartViewModel>();
                    services.AddTransient<SocChartViewModel>();
                    services.AddTransient<GpsAccuracyChartViewModel>();
                    services.AddTransient<TirePressureChartViewModel>();
                    services.AddTransient<GidsChartViewModel>();
                    services.AddTransient<HVoltChartViewModel>();
                    services.AddTransient<GearPositionChartViewModel>();
                    services.AddTransient<MotorTorqueRpmChartViewModel>();
                    services.AddTransient<V12BatteryChartViewModel>();
                    services.AddTransient<HVPackVoltsAmpsChartViewModel>();
                    services.AddTransient<CellPairHeatMapViewData>();
                })
                .Build();

            _host.Start();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }

    /// <summary>
    /// Represents the configuration settings for initializing and managing the behavior of TripView.
    /// </summary>
    public class StartupConfiguration
    {
        /// <summary>
        /// Initial map starting position Latitude (in degrees)
        /// </summary>
        public double InitialPositionLatitude { get; set; } = 34.01596;
        /// <summary>
        /// Initial map starting position Longitude (in degrees)
        /// </summary>
        public double InitialPositionLongitude { get; set; } = -118.28498;

        /// <summary>
        /// Initial map zoom level (0-9)
        /// </summary>
        public int InitialZoomLevel { get; set; } = 9;

        /// <summary>
        /// How long in seconds to take to zoom in to the specified zoom level
        /// </summary>
        public int ZoomTimeInSeconds { get; set; } = 2;

        /// <summary>
        /// Open Street Maps tile server url. If you want to self-host OSM, change this to point to your instance.
        /// </summary>
        public string OpenStreetMapUrl { get; set; } = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

        /// <summary>
        /// Max time in minutes to consider a point to be part of the same trip.
        /// </summary>
        public int MinutesBetweenTrips { get; set; } = 5;


        /// <summary>
        /// Set to false to store OSM tiles on the filesystem instead of an SQLite database.
        /// </summary>
        public bool UseSqlAsCache { get; set; } = true;
    }

    public class ColorConfiguration
    {
        public string? MapRouteColor { get; set; }
        public string? ChartCrosshairColor { get; set; }
        public int ChartLineThickness { get; set; } = 3;
        public string? ChartBackgroundColor { get; set; } = SKColors.White.ToString();
        public string? ChartPrimaryColor { get; set; }
        public string? ChartSecondaryColor { get; set; }
        public string? ChartTertiaryColor { get; set; }
        public string? ChartQuaternaryColor { get; set; }
        public string? ChartQuinaryColor { get; set; }
        public string? ChartSenaryColor { get; set; }
        public string? ChartSeptenaryColor { get; set; }
        public string? ChartOctonaryColor { get; set; }
        public string? ChartNonaryColor { get; set; }
        public string? ChartDenaryColor { get; set; } //10
        public string? GpsAccuracyColor { get; set; }
    }

    public class ChartConfiguration
    {
        public AirPressureUnit AirPressureUnit { get; set; } = AirPressureUnit.PSI;
        public DistanceUnit DistanceUnit { get; set; } = DistanceUnit.FEET;
        public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.FAHRENHEIT;
        public int TimeAxisLabelRotation { get; set; } = 15;
    }
}