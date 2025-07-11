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

namespace TripView.Configuration
{
    /// <summary>
    /// Colors used in the UI (map and chart)
    /// These are the SkiaSharp hex color codes ARGB format
    /// </summary>
    public class ColorConfiguration
    {
        /// <summary>
        /// Color of the route on the map
        /// </summary>
        public string? MapRouteColor { get; set; }

        /// <summary>
        /// Color of GPS accuracy on the map
        /// </summary>
        public string? GpsAccuracyColor { get; set; }

        /// <summary>
        /// Chart crosshair color
        /// </summary>
        public string? ChartCrosshairColor { get; set; }

        /// <summary>
        /// Chart background color
        /// </summary>
        public string? ChartBackgroundColor { get; set; }

        /// <summary>
        /// First series color
        /// </summary>
        public string? ChartPrimaryColor { get; set; }

        /// <summary>
        /// Second series color
        /// </summary>
        public string? ChartSecondaryColor { get; set; }

        /// <summary>
        /// Third series color
        /// </summary>
        public string? ChartTertiaryColor { get; set; }

        /// <summary>
        /// Fourth series color
        /// </summary>
        public string? ChartQuaternaryColor { get; set; }

        /// <summary>
        /// Fifth series color
        /// </summary>
        public string? ChartQuinaryColor { get; set; }

        /// <summary>
        /// Sixth series color
        /// </summary>
        public string? ChartSenaryColor { get; set; }

        /// <summary>
        /// Seventh series color
        /// </summary>
        public string? ChartSeptenaryColor { get; set; }

        /// <summary>
        /// Eighth Series color
        /// </summary>
        public string? ChartOctonaryColor { get; set; }

        /// <summary>
        /// Ninth series color
        /// </summary>
        public string? ChartNonaryColor { get; set; }

        /// <summary>
        /// Tenth series color
        /// </summary>
        public string? ChartDenaryColor { get; set; } //10
    }
}