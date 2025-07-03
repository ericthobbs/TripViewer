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
using System.ComponentModel;

namespace LeafSpy.DataParser.ValueTypes
{
    public class TemperatureValue : BaseValue
    {
        public TemperatureUnit SourceTemperatureUnit { get; private set; }

        public TemperatureValue(TemperatureUnit unit, string rawValue) : base(rawValue)
        {
            SourceTemperatureUnit = unit;
        }

        public float ToCelsius()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceTemperatureUnit == TemperatureUnit.CELSIUS)
                return float.Parse(RawValue);
            return ConvertTo(TemperatureUnit.CELSIUS);
        }

        public float ToFahrenheit()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceTemperatureUnit == TemperatureUnit.FAHRENHEIT)
                return float.Parse(RawValue);
            return ConvertTo(TemperatureUnit.FAHRENHEIT);
        }

        public float ConvertTo(TemperatureUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceTemperatureUnit == unit)
                return float.Parse(RawValue);

            switch(unit)
            {
                case TemperatureUnit.CELSIUS:
                    return (float.Parse(RawValue) - 32) / 1.8f;
                case TemperatureUnit.FAHRENHEIT:
                    return float.Parse(RawValue) * 1.8f + 32;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourceTemperatureUnit));
            }
        }
    }
}
