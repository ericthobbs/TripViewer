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
    public class SpeedValue : BaseValue
    {
        public DistanceUnit SourceSpeedUnit { get; private set; }
        public const float MphToMh = 1609.34f;
        public const float MphToKmh = 1.609344f;
        public const float MphToFph = 5280f;
        public SpeedValue(DistanceUnit unit, string rawValue) : base(rawValue)
        {
            SourceSpeedUnit = unit;
        }

        public float ToMilesPerHour()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceSpeedUnit == DistanceUnit.MILES)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.MILES);
        }

        public float ToKilometersPerHour()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceSpeedUnit == DistanceUnit.KILOMETERS)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.KILOMETERS);
        }

        public float ConvertTo(DistanceUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceSpeedUnit == unit)
                return float.Parse(RawValue);

            float value = float.Parse(RawValue);

            //normalize unit to be in mph
            float valueInMph = SourceSpeedUnit switch
            {
                DistanceUnit.FEET => value / MphToFph,
                DistanceUnit.MILES => value,
                DistanceUnit.METER => value / MphToMh,
                DistanceUnit.KILOMETERS => value / MphToKmh,
                _ => throw new InvalidEnumArgumentException(nameof(SourceSpeedUnit))
            };

            return unit switch
            {
                DistanceUnit.FEET => valueInMph / MphToFph,
                DistanceUnit.MILES => valueInMph,
                DistanceUnit.METER => valueInMph * MphToMh,
                DistanceUnit.KILOMETERS => valueInMph * MphToKmh,
                _ => throw new InvalidEnumArgumentException(nameof(unit))
            };
        }
    }
}
