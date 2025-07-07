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
using LeafSpy.DataParser;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TripView.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the human readable enum value from a enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        public static string ToAbbrevationOverTime(this DistanceUnit unit)
        {
            switch (unit)
            {
                case DistanceUnit.METER:
                    return "kmh";
                case DistanceUnit.FEET:
                    return "mph";
                default:
                    return string.Empty;
            }
        }

        public static string ToShortAbbrevation(this TemperatureUnit unit)
        {
            switch (unit)
            {
                case TemperatureUnit.FAHRENHEIT:
                    return "F";
                case TemperatureUnit.CELSIUS:
                    return "C";
                default:
                    return string.Empty;
            }
        }
    }
}
