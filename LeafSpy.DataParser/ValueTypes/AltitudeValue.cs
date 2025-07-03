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
    public class AltitudeValue : BaseValue
    {
        public DistanceUnit SourceDistanceUnit { get; private set; }
        public const float FeetToMeters = 0.3048f;

        public AltitudeValue(DistanceUnit unit, string rawValue) : base(rawValue)
        {
            SourceDistanceUnit = unit;
        }

        public float ToMeters()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == DistanceUnit.METER)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.METER);
        }

        public float ToFeet()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == DistanceUnit.FEET)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.FEET);
        }

        public float ConvertTo(DistanceUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == unit)
                return float.Parse(RawValue);
            
            switch(unit)
            {
                case DistanceUnit.FEET:
                    return float.Parse(RawValue) / FeetToMeters;
                case DistanceUnit.METER:
                    return float.Parse(RawValue) * FeetToMeters;
                default:
                    throw new InvalidEnumArgumentException(nameof(unit));
            }
        }
    }
}
