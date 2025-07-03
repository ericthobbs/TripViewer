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
using LeafSpy.DataParser.TypeConverters;
using LeafSpy.DataParser.ClassMaps;

namespace LeafSpy.DataParser.Parsers
{
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

            csvReader.Context.RegisterClassMap(new CsvToTripLogMap(leafspyImportConfiguration));
        }
    }
}
