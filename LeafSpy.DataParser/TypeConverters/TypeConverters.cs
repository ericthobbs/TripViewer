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
using System;
using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace LeafSpy.DataParser.TypeConverters
{

    internal class FloatConverter10K : DefaultTypeConverter
    {
        private readonly int divisor = 10000;
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return float.Parse(text ?? "0") / divisor;
        }
    }

    internal class FloatConverter1K : DefaultTypeConverter
    {
        private readonly int divisor = 1000;
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return float.Parse(text ?? "0") / divisor;
        }
    }

    internal class FloatOrNoneConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text == null || text == "none" || text == "na")
                return 0.0f;
            return float.Parse(text ?? "0");
        }
    }

    internal class MultiplyConverterBase : DefaultTypeConverter
    {
        public int Multiplier { get; set; } = 1;

        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return int.Parse(text ?? "0") * Multiplier;
        }
    }
    
    internal class MultiplyBy50Converter : MultiplyConverterBase
    {
        public MultiplyBy50Converter() 
        {
            Multiplier = 50;
        }
    }

    internal class MultiplyBy100Converter : MultiplyConverterBase
    {
        public MultiplyBy100Converter()
        {
            Multiplier = 100;
        }
    }

    internal class MultiplyBy250Converter : MultiplyConverterBase
    {
        public MultiplyBy250Converter()
        {
            Multiplier = 250;
        }
    }
}