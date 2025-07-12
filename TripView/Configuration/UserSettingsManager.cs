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

    /// <summary>
    /// Provides functionality for managing user settings, including saving, deleting, and accessing configuration data
    /// stored in a JSON file. This data is used to override appsettings.json
    /// </summary>
    /// <remarks>The user settings are stored in a JSON file located in the application's local data folder.
    /// This class supports saving multiple configuration sections, such as color settings, chart settings, startup
    /// settings, and LeafSpy import settings. It also provides methods for deleting the settings file.</remarks>
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

        /// <summary>
        /// Gets the full file path to the user settings file.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsManager"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used to record diagnostic and operational information.</param>
        public UserSettingsManager(ILogger<UserSettingsManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Saves the provided configuration settings to a user settings file.
        /// </summary>
        /// <remarks>This method serializes the provided configuration objects into JSON format and writes
        /// them to a user settings file. Any configuration object that is null will be excluded from the saved file. If
        /// an error occurs during the save operation, the method logs the error and returns <see
        /// langword="false"/>.</remarks>
        /// <param name="colorConfiguration">The color configuration settings to be saved. Cannot be null.</param>
        /// <param name="chartConfiguration">The chart configuration settings to be saved. Cannot be null.</param>
        /// <param name="startupConfiguration">The startup configuration settings to be saved. Cannot be null.</param>
        /// <param name="importConfiguration">The Leafspy import configuration settings to be saved. Cannot be null.</param>
        /// <returns><see langword="true"/> if the settings were successfully saved; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Deletes the user settings file from the file system.
        /// </summary>
        /// <remarks>This method attempts to delete the user settings file specified by <see
        /// cref="UserSettingsFile"/>. If the deletion fails due to an exception, the failure is logged and the method
        /// returns <see langword="false"/>.</remarks>
        /// <returns><see langword="true"/> if the user settings file was successfully deleted; otherwise, <see
        /// langword="false"/>.</returns>
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
