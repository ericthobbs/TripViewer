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
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TripView.Configuration
{
    /// <summary>
    /// Recent Files Manager - Handles updates to the backing storage and 
    /// prunes the list when it overfills by removing the oldest item
    /// </summary>
    public class RecentFilesManager : IEnumerable<string>
    {

        /// <summary>
        /// Really Basic Iterator
        /// </summary>
        private class RecentFileIterator : IEnumerator<string>
        {
            private readonly IList<string> _files;
            private int _index = -1;

            public object Current
            {
                get
                {
                    if (_index < 0 || _index >= _files.Count)
                        throw new InvalidOperationException();
                    return _files[_index];
                }
            }

            object IEnumerator.Current => Current;

            string IEnumerator<string>.Current => (string)Current;

            public RecentFileIterator(IList<string> files)
            {
                _files = files;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _files.Count;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {

            }
        }

        private readonly ILogger<RecentFilesManager> _logger;

        private RecentlyUsedFilesConfiguration _recentConfig = new();

        public int MaxRecentFiles { get; private set; } = 10;

        /// <summary>
        /// Path to the recent files state file.
        /// </summary>
        private static string RecentFilesSettingsFile
        {
            get
            {
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Eric Hobbs", "TripView", "Recent-files.json");
            }
        }

        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Manages a list of recently accessed files, providing functionality to load, track, and retrieve them.
        /// </summary>
        /// <remarks>This class initializes the recent files list by loading data from a persistent source
        /// during construction. It requires an <see cref="ILogger{TCategoryName}"/> instance for logging
        /// operations.</remarks>
        /// <param name="logger">An instance of <see cref="ILogger{RecentFilesManager}"/> used to log operations and errors.</param>
        public RecentFilesManager(ILogger<RecentFilesManager> logger)
        {
            _logger = logger;
            Load();
        }

        /// <summary>
        /// Loads the configuration for recently used files from a JSON file.
        /// </summary>
        /// <remarks>This method attempts to read the configuration from the file specified by <see
        /// cref="RecentFilesSettingsFile"/>. If the file cannot be read or deserialized, an error is logged and the
        /// configuration is initialized to a default state.</remarks>
        public void Load()
        {
            try
            {
                var json = System.IO.File.ReadAllText(RecentFilesSettingsFile);
                _recentConfig = JsonSerializer.Deserialize<RecentlyUsedFilesConfiguration>(json) ?? new RecentlyUsedFilesConfiguration();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cant read {RecentFilesSettingsFile}.", RecentFilesSettingsFile);
            }
        }

        /// <summary>
        /// Saves the recent configuration to a file in JSON format.
        /// </summary>
        /// <remarks>This method serializes the recent configuration object and writes it to the specified
        /// file. If an error occurs during the save operation, the error is logged.</remarks>
        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_recentConfig, _serializerOptions);
                System.IO.File.WriteAllText(RecentFilesSettingsFile, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cant write {RecentFilesSettingsFile}.", RecentFilesSettingsFile);
            }
        }

        /// <summary>
        /// Adds a file to the list of recent files, ensuring the list does not exceed the maximum allowed.
        /// </summary>
        /// <remarks>If the file already exists in the recent files list, it is moved to the top of the
        /// list.  If the list exceeds the maximum number of recent files, the oldest file is removed.</remarks>
        /// <param name="file">The path of the file to add to the recent files list. Cannot be null or empty.</param>
        public void AddRecentFile(string file)
        {
            if (_recentConfig.Files.Count >= MaxRecentFiles)
            {
                _recentConfig.Files.Remove(_recentConfig.Files.First());
            }
            if (_recentConfig.Files.Contains(file))
            {
                _recentConfig.Files.Remove(file);
                _recentConfig.Files.Insert(0, file);
            } else
            {
                _recentConfig.Files.Add(file);
            }
                Save();
        }

        /// <summary>
        /// Removes the specified file from the list of recent files.
        /// </summary>
        /// <remarks>If the file exists in the list of recent files, it is removed and the updated list is
        /// saved. If the file does not exist in the list, no changes are made.</remarks>
        /// <param name="file">The path of the file to remove from the recent files list. Cannot be null or empty.</param>
        public void RemoveRecentFile(string file)
        {
            if(_recentConfig.Files.Remove(file))
            {
                Save();
            }
        }

        /// <summary>
        /// Clears the list of recent configuration files.
        /// </summary>
        /// <remarks>This method removes all entries from the list of recent configuration files and saves
        /// the updated state.</remarks>
        public void Clear()
        {
            _recentConfig.Files.Clear();
            Save();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new RecentFileIterator(_recentConfig.Files);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RecentFileIterator(_recentConfig.Files);
        }
    }
}
