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
using CommunityToolkit.Mvvm.ComponentModel;
using TripView.Configuration;

namespace TripView.ViewModels
{
    public partial class StartupConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private double initialPositionLatitude;

        [ObservableProperty]
        private double initialPositionLongitude;

        [ObservableProperty]
        private int initialZoomLevel;

        [ObservableProperty]
        private int zoomTimeInSeconds;

        [ObservableProperty]
        public string? openStreetMapUrl;

        [ObservableProperty]
        public int minutesBetweenTrips;

        [ObservableProperty]
        public bool useSqlAsCache;

        public StartupConfigurationViewModel(StartupConfiguration config)
        {
            Read(config);
        }

        public void Read(StartupConfiguration config) 
        {
            InitialPositionLatitude = config.InitialPositionLatitude;
            InitialPositionLongitude = config.InitialPositionLongitude;
            InitialZoomLevel = config.InitialZoomLevel;
            ZoomTimeInSeconds = config.ZoomTimeInSeconds;
            OpenStreetMapUrl = config.OpenStreetMapUrl;
            MinutesBetweenTrips = config.MinutesBetweenTrips;
            UseSqlAsCache = config.UseSqlAsCache;
        }

        public StartupConfiguration ToStartupConfiguration()
        {
            return new StartupConfiguration()
            {
                InitialPositionLatitude = InitialPositionLatitude,
                InitialPositionLongitude = InitialPositionLongitude,
                InitialZoomLevel = InitialZoomLevel,
                ZoomTimeInSeconds = ZoomTimeInSeconds,
                OpenStreetMapUrl = OpenStreetMapUrl ?? string.Empty,
                MinutesBetweenTrips = MinutesBetweenTrips,
                UseSqlAsCache = UseSqlAsCache
            };
        }
    }
}
