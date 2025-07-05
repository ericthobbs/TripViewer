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
    public class OdoValue : BaseValue
    {
        public DistanceUnit SourceDistanceUnit { get; private set; }
        public const float KmToMiles = 0.621371f;

        public OdoValue(DistanceUnit unit, string rawValue) : base(rawValue)
        {
            SourceDistanceUnit = unit;
        }

        private OdoValue(DistanceUnit unit, float value) : base(value.ToString())
        {
            SourceDistanceUnit = unit;
        }

        public static OdoValue operator-(OdoValue x, OdoValue y)
        {
            return new OdoValue(x.SourceDistanceUnit, x.ConvertTo(x.SourceDistanceUnit) - y.ConvertTo(x.SourceDistanceUnit));
        }

        public float ToMiles()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == DistanceUnit.FEET)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.FEET);
        }

        public float ToKilometers()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == DistanceUnit.METER)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.METER);
        }

        public float ConvertTo(DistanceUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == unit)
                return float.Parse(RawValue);

            switch (unit)
            {
                case DistanceUnit.FEET:
                    return float.Parse(RawValue) * KmToMiles;
                case DistanceUnit.METER:
                    return float.Parse(RawValue) / KmToMiles;
                default:
                    throw new InvalidEnumArgumentException(nameof(unit));
            }
        }

        /// <summary>
        /// Returns the Odometer distance in miles. This is temporary until a better solution can be found for the PropertyGrid control.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{ConvertTo(DistanceUnit.FEET):N2} miles";
        }
    }
}
