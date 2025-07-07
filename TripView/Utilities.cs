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
using LeafSpy.DataParser.ValueTypes;
using System.IO;
using TripView.Extensions;

namespace TripView
{

    /// <summary>
    /// Misc utility methods.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Converts a color string into a Skia SKColor
        /// </summary>
        /// <param name="mapRouteColor">the ARGB hex color code string</param>
        /// <param name="fallback">Fallback color in case the color code is invalid.</param>
        /// <returns>the color value from the provided string or the fallback color if conversion fails.</returns>
        public static SkiaSharp.SKColor GetColorFromString(string? mapRouteColor, SkiaSharp.SKColor fallback)
        {
            if(mapRouteColor == null) 
            { 
                return fallback; 
            }
            try
            {
                var pointColor = fallback;
                if (!SkiaSharp.SKColor.TryParse(mapRouteColor, out pointColor))
                {
                    pointColor = fallback;
                }
                return pointColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to GetColorFromString ({mapRouteColor}): {ex.Message}");
                return fallback;
            }
        }

        /// <summary>
        /// Converts the contents of a <see cref="Stream"/> to a Base64-encoded string.
        /// </summary>
        /// <remarks>This method reads all data from the provided stream and encodes it as a Base64
        /// string. Ensure the stream is positioned at the desired starting point before calling this method. The stream
        /// is not disposed by this method; the caller is responsible for managing its lifecycle.</remarks>
        /// <param name="stream">The input stream containing the data to encode. Cannot be <see langword="null"/>.</param>
        /// <returns>A Base64-encoded string representing the contents of the input stream.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is <see langword="null"/>.</exception>
        public static string StreamToBase64(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }


        /// <summary>
        /// Calculate the heading in degrees from one point to another
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">Destination point</param>
        /// <returns>Heading in Degrees from start to destination</returns>
        public static double CalculateHeading(GPSCoordinates start, GPSCoordinates end)
        {
            return CalculateHeading(start.Latitude.ToDecimalDegrees(), start.Longitude.ToDecimalDegrees(),
                                    end.Latitude.ToDecimalDegrees(), end.Longitude.ToDecimalDegrees());
        }

        /// <summary>
        /// Calculate the heading in degrees from one point to another.
        /// </summary>
        /// <param name="lat1">starting latitude (degrees)</param>
        /// <param name="lon1">starting longitude (degrees)</param>
        /// <param name="lat2">destination latitude (degrees)</param>
        /// <param name="lon2">destination longitude (degrees)</param>
        /// <returns>Heading in Degrees from start to destination</returns>
        /// <seealso cref="https://www.movable-type.co.uk/scripts/latlong.html"/>
        private static double CalculateHeading(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = (lon2 - lon1).ToRadians();
            lat1 = lat1.ToRadians();
            lat2 = lat2.ToRadians();

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            var heading = Math.Atan2(y, x);
            return (heading.ToDegrees() + 360) % 360;
        }
    }
}
