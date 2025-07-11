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
using SkiaSharp;
using TripView.Configuration;
using TripView.ViewModels.Charts;

namespace TripView.ViewModels
{
    public partial class ColorConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private SKColor chartCrosshairColor;

        [ObservableProperty]
        private int chartLineThickness;

        [ObservableProperty]
        private SKColor mapRouteColor;

        [ObservableProperty]
        private SKColor gpsAccuracyColor;

        [ObservableProperty]
        private SKColor chartBackgroundColor;

        [ObservableProperty]
        private SKColor chartPrimaryColor;

        [ObservableProperty]
        private SKColor chartSecondaryColor;

        [ObservableProperty]
        private SKColor chartTertiaryColor;

        [ObservableProperty]
        private SKColor chartQuaternaryColor;

        [ObservableProperty]
        private SKColor chartQuinaryColor;

        [ObservableProperty]
        private SKColor chartSenaryColor;

        [ObservableProperty]
        private SKColor chartSeptenaryColor;

        [ObservableProperty]
        private SKColor chartOctonaryColor;

        [ObservableProperty]
        private SKColor chartNonaryColor;

        [ObservableProperty]
        private SKColor chartDenaryColor; //10

        public ColorConfigurationViewModel(ColorConfiguration config) {
            Read(config);
        }

        public void Read(ColorConfiguration config) {
            ChartCrosshairColor = ConfigurationUtilities.GetColorFromString(config.ChartCrosshairColor, Charts.ChartDefaults.CrosshairColor);
            ChartLineThickness = 1;
            MapRouteColor = ConfigurationUtilities.GetColorFromString(config.MapRouteColor, SKColors.MediumPurple);
            GpsAccuracyColor = ConfigurationUtilities.GetColorFromString(config.GpsAccuracyColor, SKColors.LightBlue);
            ChartBackgroundColor = ConfigurationUtilities.GetColorFromString(config.ChartBackgroundColor, SKColors.White);
            ChartPrimaryColor = ConfigurationUtilities.GetColorFromString(config.ChartPrimaryColor, ChartDefaults.Series1Color);
            ChartSecondaryColor = ConfigurationUtilities.GetColorFromString(config.ChartSecondaryColor, ChartDefaults.Series2Color);
            ChartTertiaryColor = ConfigurationUtilities.GetColorFromString(config.ChartTertiaryColor, ChartDefaults.Series3Color);
            ChartQuaternaryColor = ConfigurationUtilities.GetColorFromString(config.ChartQuaternaryColor, ChartDefaults.Series4Color);
            ChartQuinaryColor = ConfigurationUtilities.GetColorFromString(config.ChartQuinaryColor, ChartDefaults.Series5Color);
            ChartSenaryColor = ConfigurationUtilities.GetColorFromString(config.ChartSenaryColor, ChartDefaults.Series6Color);
            ChartSeptenaryColor = ConfigurationUtilities.GetColorFromString(config.ChartSeptenaryColor, ChartDefaults.Series7Color);
            ChartOctonaryColor = ConfigurationUtilities.GetColorFromString(config.ChartOctonaryColor, ChartDefaults.Series8Color);
            ChartNonaryColor = ConfigurationUtilities.GetColorFromString(config.ChartNonaryColor, ChartDefaults.Series9Color);
            ChartDenaryColor = ConfigurationUtilities.GetColorFromString(config.ChartDenaryColor, ChartDefaults.Series10Color);
        }

        public ColorConfiguration ToColorConfiguration()
        {
            return new ColorConfiguration() { 
                ChartCrosshairColor = ChartCrosshairColor.ToString(),
                ChartLineThickness = ChartLineThickness,
                MapRouteColor = MapRouteColor.ToString(),
                GpsAccuracyColor = GpsAccuracyColor.ToString(),
                ChartBackgroundColor = ChartBackgroundColor.ToString(),
                ChartPrimaryColor = ChartPrimaryColor.ToString(),
                ChartSecondaryColor = ChartSecondaryColor.ToString(),
                ChartTertiaryColor = ChartTertiaryColor.ToString(),
                ChartQuaternaryColor = ChartQuaternaryColor.ToString(),
                ChartQuinaryColor = ChartQuinaryColor.ToString(),
                ChartSenaryColor = ChartSenaryColor.ToString(),
                ChartSeptenaryColor = ChartSeptenaryColor.ToString(),
                ChartOctonaryColor = ChartOctonaryColor.ToString(),
                ChartNonaryColor = ChartNonaryColor.ToString(),
                ChartDenaryColor = ChartDenaryColor.ToString(),
            };
        }
    }
}
