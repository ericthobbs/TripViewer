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
using LeafSpy.DataParser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TripView.Configuration;

namespace TripView.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, IDisposable
    {
        private readonly IOptionsMonitor<ColorConfiguration> _colorConfiguration;
        private readonly IOptionsMonitor<ChartConfiguration> _chartConfiguration;
        private readonly IOptionsMonitor<LeafspyImportConfiguration> _importConfiguration;
        private readonly IOptionsMonitor<StartupConfiguration> _startupConfiguration;
        private readonly ILogger<SettingsViewModel> _logger;

        private readonly IDisposable? _colorConfigChangeListener;
        private readonly IDisposable? _chartConfigurationListener;
        private readonly IDisposable? _importConfigurationListener;
        private readonly IDisposable? _startupConfigurationListener;
        private bool disposedValue;

        [ObservableProperty]
        private ColorConfigurationViewModel colorConfig;

        [ObservableProperty]
        private StartupConfigurationViewModel startupConfig;

        [ObservableProperty]
        private ChartConfigurationViewModel chartConfig;

        [ObservableProperty]
        private ImportConfigurationViewModel importConfig;

        public SettingsViewModel(
            IOptionsMonitor<ColorConfiguration> colorConfiguration,
            IOptionsMonitor<ChartConfiguration> chartConfiguration,
            IOptionsMonitor<LeafspyImportConfiguration> leafspyConfiguration,
            IOptionsMonitor<StartupConfiguration> startupConfiguration,
            ILogger<SettingsViewModel> logger) {
            _colorConfiguration = colorConfiguration;
            _chartConfiguration = chartConfiguration;
            _importConfiguration = leafspyConfiguration;
            _startupConfiguration = startupConfiguration;
            _logger = logger;

            _colorConfigChangeListener = _colorConfiguration.OnChange(OnColorConfigChange);
            _chartConfigurationListener = _chartConfiguration.OnChange(OnChartConfigChange);
            _importConfigurationListener = _importConfiguration.OnChange(OnLeafSpyConfigChange);
            _startupConfigurationListener = _startupConfiguration.OnChange(OnStartupConfigChange);

            ColorConfig = new ColorConfigurationViewModel(_colorConfiguration.CurrentValue);
            StartupConfig = new StartupConfigurationViewModel(_startupConfiguration.CurrentValue);
            ChartConfig = new ChartConfigurationViewModel(_chartConfiguration.CurrentValue);
            ImportConfig = new ImportConfigurationViewModel(_importConfiguration.CurrentValue);
        }

        private void OnStartupConfigChange(StartupConfiguration config)
        {
            _logger.LogDebug("Change to StartupConfiguration detected.");
            StartupConfig.Read(_startupConfiguration.CurrentValue);
            
        }

        private void OnColorConfigChange(ColorConfiguration config)
        {
            _logger.LogDebug("Change to ColorConfiguration detected.");
            ColorConfig.Read(config);
        }

        private void OnChartConfigChange(ChartConfiguration config)
        {
            _logger.LogDebug("Change to ChartConfiguration detected.");
            ChartConfig.Read(config);
        }

        private void OnLeafSpyConfigChange(LeafspyImportConfiguration config)
        {
            _logger.LogDebug("Change to LeafspyImportConfiguration detected.");
            ImportConfig.Read(config);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _colorConfigChangeListener?.Dispose();
                    _chartConfigurationListener?.Dispose();
                    _importConfigurationListener?.Dispose();
                    _startupConfigurationListener?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SettingsViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region XAML
        [Obsolete("XAML Only. DO NOT USE.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public SettingsViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        #endregion

    }
}
