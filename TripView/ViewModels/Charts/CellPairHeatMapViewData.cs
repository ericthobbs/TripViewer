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
using Microsoft.Extensions.Options;
using SkiaSharp;
using System.Collections.ObjectModel;
using TripView.Configuration;

namespace TripView.ViewModels.Charts
{
    public class CellPairHeatMapViewData : BaseChartViewModel
    {
        public CellPairHeatMapViewData(
            IOptionsMonitor<ColorConfiguration> colorConfiguration,
            IOptionsMonitor<ChartConfiguration> chartConfig) : base(colorConfiguration, chartConfig)
        {
            Name = "Cell Pair Voltage x Time";
            Reset();
        }

        public override void Reset()
        {
            Series.Clear();
            XAxes.Clear();
            YAxes.Clear();

            XAxes.Add(
                new DateTimeAxis(TimeSpan.FromSeconds(5), value => value.ToString("hh:mm:ss"))
                {
                    Name = "Time",
                    LabelsRotation = 15,
                    TextSize = 10,
                    UnitWidth = TimeSpan.FromSeconds(5).Ticks,
                    MinStep = TimeSpan.FromSeconds(5).Ticks,

                    //UnitWidth = TimeSpan.FromSeconds(5).Ticks,
                    //MinStep = TimeSpan.FromSeconds(5).Ticks,
                    //CrosshairSnapEnabled = true,
                    //CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
                });

            YAxes.Add(new Axis()
            {
                //Labeler = (value) => $"{value:N2} Cell Index",
                Name = "Cell Pair",
                //CrosshairSnapEnabled = true,
                //CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
                

                //UnitWidth = TimeSpan.FromSeconds(5).Ticks,
                //MinStep = TimeSpan.FromSeconds(5).Ticks,
                
                MinStep = 1,
                MinLimit = 1,
                MaxLimit = 96,
                LabelsRotation = 0,
                TextSize = 10,
            });
        }

        public override void LoadData(ObservableCollection<TripLog> Events, int minMinutesBetweenTrip)
        {
            Series.Add(new HeatSeries<WeightedPoint>
            {
                Values = GetAllCellPairs(Events).ToList(),
                Name = "Cell Pair",
                HeatMap = new[]
                {
                    SKColors.Red.AsLvcColor(),
                    SKColors.Yellow.AsLvcColor(),
                    SKColors.Orange.AsLvcColor(),
                    SKColors.Blue.AsLvcColor(),
                    SKColors.Green.AsLvcColor(),
                },
            });
        }

        //Testing Logic. This may not be correct
        protected static IList<List<TripLog>> SplitTrips(List<TripLog> events, int minMinutesBetweenTrip)
        {
            var subTrips = new List<List<TripLog>>();
            var tsMaxdiff = TimeSpan.FromMinutes(minMinutesBetweenTrip);
            var startRange = 0;
            for (int i = 1; i < events.Count; i++)
            {
                if ((events[i].DateTime - events[i - 1].DateTime) > tsMaxdiff)
                {
                    var e = events[startRange..i];
                    startRange = i;
                    subTrips.AddRange(e);
                    i++;
                }
            }
            if (subTrips.Count == 0)
                subTrips.Add(events);

            return subTrips;
        }

        private IEnumerable<WeightedPoint> GetAllCellPairs(IEnumerable<TripLog> tripLogs)
        {
            var tripsMaster = SplitTrips(tripLogs.ToList(), 5);
            var trips = tripsMaster.First();
            foreach (var tripLog in trips) {
                foreach(var pair in tripLog.GetAllCellPairs())
                {
                    yield return new WeightedPoint(tripLog.DateTime.Ticks, pair.Item1, (float)pair.Item2 / 1000);
                    if (tripLog != trips.LastOrDefault())
                    {
                        yield return new WeightedPoint(tripLog.DateTime.Ticks+5, pair.Item1, null);
                    }
                }
            }
        }
    }
}