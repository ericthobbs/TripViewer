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
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace LeafSpy.DataParser.Parsers
{
    public class LeafSpyBaseCsvParser<T> : IDisposable
    {
        public string LogFileName { get; private set; } = string.Empty;

        protected readonly CsvConfiguration config;
        protected readonly LeafspyImportConfiguration leafspyImportConfiguration;
        protected CsvReader? csvReader;
        private bool disposedValue;

        public LeafSpyBaseCsvParser(LeafspyImportConfiguration importConfiguration)
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture) { 
                Delimiter = importConfiguration.CsvDelimiter
            };
            leafspyImportConfiguration = importConfiguration;
        }

        public virtual void Open(string fileName)
        {
            LogFileName = fileName;
            csvReader = new CsvReader(new StreamReader(File.Open(LogFileName, FileMode.Open, FileAccess.Read, FileShare.Read)), config);
        }

        public IEnumerable<T> Read()
        {
            if (csvReader != null)
                return csvReader.GetRecords<T>();
            return [];
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
