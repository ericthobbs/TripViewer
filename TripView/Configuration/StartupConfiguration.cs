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
    /// Represents the configuration settings for initializing and managing the behavior of TripView.
    /// </summary>
    public class StartupConfiguration
    {
        /// <summary>
        /// Initial map starting position Latitude (in degrees)
        /// </summary>
        public double InitialPositionLatitude { get; set; } = 34.01596;
        /// <summary>
        /// Initial map starting position Longitude (in degrees)
        /// </summary>
        public double InitialPositionLongitude { get; set; } = -118.28498;

        /// <summary>
        /// Initial map zoom level (0-9)
        /// </summary>
        public int InitialZoomLevel { get; set; } = 9;

        /// <summary>
        /// How long in seconds to take to zoom in to the specified zoom level
        /// </summary>
        public int ZoomTimeInSeconds { get; set; } = 2;

        /// <summary>
        /// Open Street Maps tile server url. If you want to self-host OSM, change this to point to your instance.
        /// </summary>
        public string OpenStreetMapUrl { get; set; } = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

        /// <summary>
        /// Max time in minutes to consider a point to be part of the same trip.
        /// </summary>
        public int MinutesBetweenTrips { get; set; } = 5;


        /// <summary>
        /// Set to false to store OSM tiles on the filesystem instead of an SQLite database.
        /// </summary>
        public bool UseSqlAsCache { get; set; } = true;
    }
}