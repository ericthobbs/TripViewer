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
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using System.Reflection;
using TripView.ViewModels.Messages;

namespace TripView
{
    public static class Extensions
    {
        public static MPoint ToMPoint(this GPSCoordinates coords)
        {
            return SphericalMercator.FromLonLat(coords.Longitude.ToDecimalDegrees(), coords.Latitude.ToDecimalDegrees()).ToMPoint();
        }

        public static double ToRadians(this double degrees) => degrees * Math.PI / 180;
        public static double ToDegrees(this double radians) => radians * 180 / Math.PI;

        public static IEnumerable<Tuple<int, float>> GetAllCellPairs(this TripLog tripEvent)
        {
            var type = typeof(TripLog);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    Property = p,
                    HasIndex = int.TryParse(p.Name.AsSpan(2), out int index) && p.Name.StartsWith("CP"),
                    Index = int.TryParse(p.Name.AsSpan(2), out int idx) ? idx : -1
                })
                .Where(x => x.HasIndex && x.Index >= 1 && x.Index <= 96 && x.Property.PropertyType == typeof(float))
                .OrderBy(x => x.Index)
                .Select(x => (x.Index, x.Property));

            foreach (var (index, prop) in properties)
            {
                yield return new Tuple<int,float>(index,(float)(prop.GetValue(tripEvent) ?? 0));
            }
        }

        public static SkiaSharp.SKEncodedImageFormat ToSkiaImageFormat(this SaveAsFormat format)
        {
            switch(format)
            {
                case SaveAsFormat.PNG:
                default:
                    return SkiaSharp.SKEncodedImageFormat.Png;
            }
        }

        public static string ToExtension(this SaveAsFormat format)
        {
            switch(format)
            {
                case SaveAsFormat.PNG:
                    return "*.png";
                default:
                    return "*.*";
            }
        }
    }
}