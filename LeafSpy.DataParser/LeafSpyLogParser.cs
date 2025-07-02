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
using LeafSpy.DataParser.TypeConverters;
using LeafSpy.DataParser.ClassMaps;

namespace LeafSpy.DataParser
{
    public class LeafSpyBaseCsvParser<T> : IDisposable
    {
        public string LogFileName { get; private set; } = String.Empty;

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

    public class LeafSpySingleTripParser : LeafSpyBaseCsvParser<TripLog>
    {
        public LeafSpySingleTripParser(LeafspyImportConfiguration cfg) : base(cfg) { }

        public override void Open(string fileName)
        {
            base.Open(fileName);
            
            if (csvReader == null)
                return;//TODO
            csvReader.Context.TypeConverterCache.AddConverter<GpsCordConverter>(new GpsCordConverter());
            csvReader.Context.TypeConverterCache.AddConverter<EpochConverter>(new EpochConverter());
            csvReader.Context.TypeConverterCache.AddConverter<GidConverter>(new GidConverter());
            csvReader.Context.TypeConverterCache.AddConverter<FloatConverter10K>(new FloatConverter10K());
            csvReader.Context.TypeConverterCache.AddConverter<FloatConverter1K>(new FloatConverter1K());
            csvReader.Context.TypeConverterCache.AddConverter<EnumConverter<GearPosition>>(new EnumConverter<GearPosition>());
            csvReader.Context.TypeConverterCache.AddConverter<EnumConverter<PlugState>>(new EnumConverter<PlugState>());
            csvReader.Context.TypeConverterCache.AddConverter<EnumConverter<ChargeMode>>(new EnumConverter<ChargeMode>());
            csvReader.Context.TypeConverterCache.AddConverter<EnumConverter<ReadState>>(new EnumConverter<ReadState>());
            csvReader.Context.TypeConverterCache.AddConverter<HexEnumConverter<FrontWiperStatus>>(new HexEnumConverter<FrontWiperStatus>());
            csvReader.Context.TypeConverterCache.AddConverter<GPSStatusConverter>(new GPSStatusConverter());
            csvReader.Context.TypeConverterCache.AddConverter<TempOffsetConverter>(new TempOffsetConverter());
            csvReader.Context.TypeConverterCache.AddConverter<MultiplyBy50Converter>(new MultiplyBy50Converter());
            csvReader.Context.TypeConverterCache.AddConverter<MultiplyBy100Converter>(new MultiplyBy100Converter());
            csvReader.Context.TypeConverterCache.AddConverter<MultiplyBy250Converter>(new MultiplyBy250Converter());
            csvReader.Context.TypeConverterCache.AddConverter<PressureValueConverter>(new PressureValueConverter(AirPressureUnit.PSI));
            csvReader.Context.TypeConverterCache.AddConverter<TemperatureValueConverter>(new TemperatureValueConverter(TemperatureUnit.FAHRENHEIT));

            csvReader.Context.RegisterClassMap(new TripMap(leafspyImportConfiguration));
        }
    }

    public class LeafSpySingleChargeLogParser : LeafSpyBaseCsvParser<ChargeLog>
    {
        public LeafSpySingleChargeLogParser(LeafspyImportConfiguration cfg) : base(cfg) { }

        public override void Open(string fileName)
        {
            base.Open(fileName);

            if (csvReader == null)
                return;//TODO

            csvReader.Context.TypeConverterCache.AddConverter<FloatConverter10K>(new FloatConverter10K());
            csvReader.Context.TypeConverterCache.AddConverter<EpochConverter>(new EpochConverter());
            csvReader.Context.RegisterClassMap<ChargeMap>();
        }
    }

    public class LeafSpyECUVersionLogParser : LeafSpyBaseCsvParser<ECUVersions>
    {
        public LeafSpyECUVersionLogParser(LeafspyImportConfiguration cfg) : base(cfg) { }

        public override void Open(string fileName)
        {
            base.Open(fileName);

            if (csvReader == null)
                return;//TODO

            csvReader.Context.RegisterClassMap<ECUVersionMap>();
        }
    }

    public class LeafSpyTripHistoryLogParser : LeafSpyBaseCsvParser<TripHistory>
    {
        public LeafSpyTripHistoryLogParser(LeafspyImportConfiguration cfg) : base(cfg) { }

        public override void Open(string fileName)
        {
            base.Open(fileName);

            if (csvReader == null)
                return;//TODO

            csvReader.Context.RegisterClassMap<TripHistoryMap>();
        }
    }
}
