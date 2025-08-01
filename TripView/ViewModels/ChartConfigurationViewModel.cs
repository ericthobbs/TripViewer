﻿/**
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
using LeafSpy.DataParser;
using TripView.Configuration;

namespace TripView.ViewModels
{
    public partial class ChartConfigurationViewModel : ObservableObject
    {

        [ObservableProperty]
        private int chartLineThickness;

        [ObservableProperty]
        private AirPressureUnit airPressureUnit;

        [ObservableProperty]
        private DistanceUnit distanceUnit;

        [ObservableProperty]
        private DistanceUnit elevationUnit;

        [ObservableProperty]
        private TemperatureUnit temperatureUnit;

        [ObservableProperty]
        private int timeAxisLabelRotation;

        public ChartConfigurationViewModel(ChartConfiguration config) {
            Read(config);
        }

        public void Read(ChartConfiguration config)
        {
            ChartLineThickness = config.ChartLineThickness;
            AirPressureUnit = config.AirPressureUnit;
            DistanceUnit = config.DistanceUnit;
            ElevationUnit = config.ElevationUnit;
            TemperatureUnit = config.TemperatureUnit;
            TimeAxisLabelRotation = config.TimeAxisLabelRotation;
        }

        public ChartConfiguration ToChartConfiguration()
        {
            return new ChartConfiguration()
            {
                ChartLineThickness = ChartLineThickness,
                AirPressureUnit = AirPressureUnit,
                DistanceUnit = DistanceUnit,
                ElevationUnit = ElevationUnit,
                TemperatureUnit = TemperatureUnit,
                TimeAxisLabelRotation = TimeAxisLabelRotation,
            };
        }
    }
}
