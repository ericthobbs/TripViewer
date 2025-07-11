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
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TripView.Configuration
{

    public class UserSettingsManager
    {
        public const string ColorConfigurationSectionName = "Colors";
        public const string ChartConfigurationSectionName = "Chart";
        public const string LeafSpyConfigurationSectionName = "LeafspyImport";
        public const string StartupConfigurationSectionName = "Startup";

        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string UserSettingsFile
        {
            get
            {
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Eric Hobbs", "TripView", "user-settings.json");
            }
        }

        private readonly ILogger<UserSettingsManager> _logger;

        public UserSettingsManager(ILogger<UserSettingsManager> logger)
        {
            _logger = logger;
        }

        public bool Save(
            ColorConfiguration colorConfiguration,
            ChartConfiguration chartConfiguration,
            StartupConfiguration startupConfiguration,
            LeafspyImportConfiguration importConfiguration)
        {
            try
            {
                var dict = new Dictionary<string, object>
                {
                    { StartupConfigurationSectionName, startupConfiguration },
                    { LeafSpyConfigurationSectionName, importConfiguration },
                    { ColorConfigurationSectionName, colorConfiguration },
                    { ChartConfigurationSectionName, chartConfiguration },
                };

                foreach (var key in dict.Where(kvp => kvp.Value is null).Select(kvp => kvp.Key).ToList())
                {
                    dict.Remove(key);
                }

                var json = JsonSerializer.Serialize(dict, _serializerOptions);
                System.IO.File.WriteAllText(UserSettingsFile, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write user settings to {UserSettingsFile}", UserSettingsFile);
                return false;
            }

            _logger.LogDebug("Wrote settings to {UserSettingsFile}", UserSettingsFile);
            return true;
        }

        public bool DeleteUserSettings()
        {
            try
            {
                System.IO.File.Delete(UserSettingsFile);
                _logger.LogDebug("Deleted file {UserSettingsFile}", UserSettingsFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user settings file {UserSettingsFile}", UserSettingsFile);
                return false;
            }
            return true;
        }
    }
}
