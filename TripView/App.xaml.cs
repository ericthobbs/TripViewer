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
using CommandLine;
using LeafSpy.DataParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Windows;
using TripView.Configuration;
using TripView.Extensions;
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
            base.OnStartup(e);

            var userConfigPath = UserSettingsManager.UserSettingsFile;
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

            var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(e.Args);
            CommandLineOptions? options = null;
            parseResult
                .WithParsed(opts => { options = opts; })
                .WithNotParsed(errors => {

                    if (errors.IsVersion())
                    {
                        var aboutWindow = new AboutWindow();
                        aboutWindow.ShowDialog();
                    }
                    else
                    {
                        //This is just a placeholder window for the moment.
                        var helpWindow = new HelpWindow(CommandLine.Text.HelpText.AutoBuild(parseResult, h => h, e => e));
                        helpWindow.ShowDialog();
                    }
                    Current.Shutdown();
                });

            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile(userConfigPath, optional: true, reloadOnChange: true);
                    config.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(e.Args);
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
                    services.Configure<StartupConfiguration>(context.Configuration.GetSection(UserSettingsManager.StartupConfigurationSectionName));
                    services.Configure<LeafspyImportConfiguration>(context.Configuration.GetSection(UserSettingsManager.LeafSpyConfigurationSectionName));
                    services.Configure<ColorConfiguration>(context.Configuration.GetSection(UserSettingsManager.ColorConfigurationSectionName));
                    services.Configure<ChartConfiguration>(context.Configuration.GetSection(UserSettingsManager.ChartConfigurationSectionName));
                    services.AddSingleton<UserSettingsManager>();
                    services.AddSingleton<RecentFilesManager>();

                    services.AddSingleton(options ?? new CommandLineOptions());

                    //Add all chart view models dynamically via reflection
                    services.AddAllDerivedFromAsTransients<BaseChartViewModel>();

                    //About Window
                    services.AddTransient<AboutWindow>();
                    services.AddSingleton<AboutViewModel>();

                    //Settings Window
                    services.AddTransient<UserSettingsWindow>();
                    services.AddSingleton<SettingsViewModel>();

                    //Main Window
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<EventViewerWindow>();
                    services.AddTransient<TripDataViewModel>();

                })
                .Build();

            _host.Start();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
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