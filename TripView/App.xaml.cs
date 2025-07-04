﻿/**
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
using System.Reflection;
using System.Windows;
using TripView.Configuration;
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
                    services.AddSingleton<About>();
                    services.AddSingleton<AboutViewModel>();
                    services.AddTransient<EventViewerWindow>();

                    services.AddTransient<TripDataViewModel>();

                    //Add all chart view models dynamically via reflection
                    services.AddAllDerivedFromAsTransients<BaseChartViewModel>();
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
}