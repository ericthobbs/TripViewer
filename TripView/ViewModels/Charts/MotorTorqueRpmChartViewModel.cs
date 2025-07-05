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
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace TripView.ViewModels.Charts
{
    public partial class MotorTorqueRpmChartViewModel : BaseChartViewModel
    {
        public MotorTorqueRpmChartViewModel(
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

            YAxes.Add(new Axis
            {
                Name = "Torque (Nm)",
                Position = LiveChartsCore.Measure.AxisPosition.Start
            });
            YAxes.Add(new Axis
            {
                Name = "Motor RPM",
                Position = LiveChartsCore.Measure.AxisPosition.End
            });
            Name = $"{YAxes[0].Name}/{YAxes[1].Name} x {XAxes[0].Name}";
        }

        public override void LoadData(ObservableCollection<TripLog> Events, int minMinutesBetweenTrip)
        {
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.RPM, minMinutesBetweenTrip),
                Name = "Motor RPM",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartPrimaryColor, ChartDefaults.Series1Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
            });
            Series.Add(new LineSeries<DateTimePoint>
            {
                Values = BuildDateTimePoints(Events, e => e.TorqueNm, minMinutesBetweenTrip),
                Name = "Torque (Nm)",
                Stroke = new SolidColorPaint(Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartSecondaryColor, ChartDefaults.Series2Color)) { StrokeThickness = _colorConfiguration.CurrentValue.ChartLineThickness },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                ScalesYAt = 1
            });
        }
    }
}