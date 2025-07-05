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
using System.Collections.ObjectModel;
using LeafSpy.DataParser;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Options;
using SkiaSharp;
using TripView.Configuration;

namespace TripView.ViewModels.Charts
{
    public partial class TemperatureChartViewModel : BaseChartViewModel
    {
        public TemperatureChartViewModel(
            IOptionsMonitor<ColorConfiguration> colorConfiguration, 
            IOptionsMonitor<ChartConfiguration> chartConfig) : base(colorConfiguration, chartConfig)
        {
            Reset();
        }

        public override void Reset()
        {
            Series.Clear();
            XAxes.Clear();
            YAxes.Clear();

            XAxes.Add(new DateTimeAxis(TimeSpan.FromSeconds(5), value => value.ToString("hh:mm:ss"))
            {
                Name = "Time",
                UnitWidth = TimeSpan.FromSeconds(5).Ticks,
                MinStep = TimeSpan.FromSeconds(5).Ticks,
                CrosshairSnapEnabled = true,
                CrosshairPaint = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartCrosshairColor, ChartDefaults.CrosshairColor), 1),
                LabelsRotation = _chartConfiguration.CurrentValue.TimeAxisLabelRotation,
            });

            YAxes.Add(new Axis()
            {
                Labeler = (value) => $"{value:N2} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = $"Temperature (\u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()})",
                CrosshairSnapEnabled = true,
                CrosshairPaint = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartCrosshairColor, ChartDefaults.CrosshairColor), 1),
            });
            Name = $"{YAxes[0].Name} x {XAxes[0].Name}";
        }

        public override void LoadData(ObservableCollection<TripLog> Events, int minMinutesBetweenTrip)
        {
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT1F.ToFahrenheit(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - T1F",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartPrimaryColor, ChartDefaults.Series1Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
            });
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT2F.ToFahrenheit(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - T2F",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartSecondaryColor, ChartDefaults.Series2Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
            });
            /*
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT3F.ToFahrenheit(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - TF3",
                Stroke = new SolidColorPaint(SKColors.BlueViolet),
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
            });*/
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT4F.ToFahrenheit(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - T4F",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartTertiaryColor, ChartDefaults.Series4Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.FAHRENHEIT,
            });

            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT1C.ToCelsius(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - T1C",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartQuinaryColor, ChartDefaults.Series5Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.CELSIUS,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.CELSIUS,
            });
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT2C.ToCelsius(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - T2C",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartSenaryColor, ChartDefaults.Series6Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.CELSIUS,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.CELSIUS,
            });
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.PackT4C.ToCelsius(), minMinutesBetweenTrip),
                DataLabelsFormatter = (point) => $"{point.Coordinate} \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Name = "Battery Temp - T4C",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartSeptenaryColor, ChartDefaults.Series7Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                IsVisible = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.CELSIUS,
                IsVisibleAtLegend = _chartConfiguration.CurrentValue.TemperatureUnit == TemperatureUnit.CELSIUS,
            });

            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.Ambient.ConvertTo(_chartConfiguration.CurrentValue.TemperatureUnit), minMinutesBetweenTrip),
                Name = $"Ambient outside Temp \u00B0{_chartConfiguration.CurrentValue.TemperatureUnit.ToShortAbbrevation()}",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartOctonaryColor, ChartDefaults.Series8Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
            });
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.MotorTemp, minMinutesBetweenTrip),
                Name = $"Motor Temp \u00B0F",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartNonaryColor, ChartDefaults.Series9Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
            });
        }
    }
}