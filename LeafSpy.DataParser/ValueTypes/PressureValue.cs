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
using System.Diagnostics;

namespace LeafSpy.DataParser.ValueTypes
{
    [DebuggerDisplay("{ToPsi()} PSI")]
    public class PressureValue : BaseValue
    {
        public AirPressureUnit SourcePressureUnit;

        public PressureValue(AirPressureUnit unit, string rawValue) : base(rawValue) 
        {
            SourcePressureUnit = unit;
        }

        public float ToPsi()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == AirPressureUnit.PSI)
                return float.Parse(RawValue);

            switch(SourcePressureUnit)
            {
                case AirPressureUnit.KPA:
                    return float.Parse(RawValue) / 6.89476f;
                case AirPressureUnit.BAR:
                    return float.Parse(RawValue) * 14.5038f;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourcePressureUnit));
            }
        }

        public float ToBar()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == AirPressureUnit.BAR)
                return float.Parse(RawValue);

            switch(SourcePressureUnit)
            {
                case AirPressureUnit.PSI:
                    return float.Parse(RawValue) / 14.5038f;
                case AirPressureUnit.KPA:
                    return float.Parse(RawValue) / 100;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourcePressureUnit));
            }
        }

        public float ToKpa()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == AirPressureUnit.KPA)
                return float.Parse(RawValue);

            switch (SourcePressureUnit)
            {
                case AirPressureUnit.PSI:
                    return float.Parse(RawValue) * 6.89476f;
                case AirPressureUnit.BAR:
                    return float.Parse(RawValue) * 100;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourcePressureUnit));
            }
        }

        public float ConvertTo(AirPressureUnit unit)
        {
            if(string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == unit)
                return float.Parse(RawValue);

            switch(unit)
            {
                case AirPressureUnit.PSI:
                    return ToPsi();
                case AirPressureUnit.KPA:
                    return ToKpa();
                case AirPressureUnit.BAR:
                    return ToBar();
                default:
                    throw new InvalidEnumArgumentException(nameof(unit));
            }
        }
    }
}
