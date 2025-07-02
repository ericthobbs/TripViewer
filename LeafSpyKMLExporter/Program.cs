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
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LeafSpyKMLExporter
{
    /// <summary>
    /// Test Program. WIP. Not for external use at this time.
    /// </summary>
    internal partial class Program
    {
        const string FilePathCommon = @"C:\adb-temp\com.Turbo3.Leaf_Spy_Pro\files\";
        const string TripLogDirName = @"LOG_FILES";

        [GeneratedRegex(pattern: @"^Log_(?<VinSuffix>\w{8})_(?<Date>\d{6})_(?<DeviceId>\w+)$", options: RegexOptions.Compiled)]
        private static partial Regex s_filenameRegex();

        /// <summary>
        /// This may be removed in the future and only LeafSpyKmlExporter may exist.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            /*
            using var host = Host.CreateApplicationBuilder(args)
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            foreach (var filename in Directory.EnumerateFiles(Path.Combine(FilePathCommon, TripLogDirName), "Log_*.csv"))
            {
                logger.LogInformation("Processing file: {Filename}", filename);

                var baseName = Path.GetFileNameWithoutExtension(filename);
                var match = s_filenameRegex().Match(baseName);

                if (!match.Success)
                {
                    logger.LogWarning("Filename did not match pattern: {BaseName}", baseName);
                    continue;
                }

                var dateStr = match.Groups["Date"].Value;
                var outputFile = $"output-{dateStr}.kml";

                LeafSpyKmlExporter.ExportToKml(new(), filename, outputFile, dateStr); //new() should be replaced with a IOption instance....

                logger.LogInformation("Exported: {OutputFile}", outputFile);
            }
            */
        }
    }
}
