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
using System.Diagnostics;

namespace LeafSpy.DataParser.ValueTypes
{
    [DebuggerDisplay("{ToDecimalDegrees()}")]
    public class GPSCoord : BaseValue
    {
        public GPSCoord(string raw) : base(raw) { } //DDD mmm.mmmmm Degrees Decimal Minutes (DDM)

        public double ToDecimalDegrees()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            int sep = RawValue.IndexOf(' ');
            if (sep == -1)
                return 0;

            // Parse degrees (can be negative)
            int degrees = int.Parse(RawValue[..sep].Trim());

            // Parse minutes (trim space after the separator)
            double minutes = double.Parse(RawValue[(sep + 1)..].Trim());

            // Handle sign correctly
            double decimalDegrees = Math.Abs(degrees) + minutes / 60.0;
            return degrees < 0 ? -decimalDegrees : decimalDegrees;
        }

        public double ToRadians()
        {
            return ToDecimalDegrees() * (Math.PI / 180.0);
        }

        /// <summary>
        /// Calculates the distance in meters to another GPS coordinate using the Haversine formula.
        /// </summary>
        /// <param name="other">GPS Coord to compare to</param>
        /// <returns>distance in meters between both coordinates</returns>
        public static double DistanceInMeters(GPSCoord lat, GPSCoord @long, GPSCoord other_lat, GPSCoord other_long)
        {
            double lat1 = lat.ToRadians();
            double lon1 = @long.ToRadians();
            double lat2 = other_lat.ToRadians();
            double lon2 = other_long.ToRadians();
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            const int RadiusOfEarthInMeters = 6371000; // Radius of the Earth in meters (Mean Earth Radius)
            return RadiusOfEarthInMeters * c; // Distance in meters
        }
    }
}
