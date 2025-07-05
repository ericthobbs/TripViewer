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
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LeafSpy.DataParser;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Options;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TripView.Configuration;
using TripView.ViewModels.Messages;

namespace TripView.ViewModels.Charts
{
    public abstract partial class BaseChartViewModel : ObservableObject
    {
        internal static class ChartDefaults
        {
            public static SKColor CrosshairColor = SKColors.DarkGray;
            public static SKColor Series1Color = SKColors.DarkOrange;
            public static SKColor Series2Color = SKColors.SteelBlue;
            public static SKColor Series3Color = SKColors.MediumSeaGreen;
            public static SKColor Series4Color = SKColors.Goldenrod;
            public static SKColor Series5Color = SKColors.SlateBlue;
            public static SKColor Series6Color = SKColors.Crimson;
            public static SKColor Series7Color = SKColors.DarkOrange;
            public static SKColor Series8Color = SKColors.MediumBlue;
            public static SKColor Series9Color = SKColors.LimeGreen;
            public static SKColor Series10Color = SKColors.Magenta;
        }

        [ObservableProperty]
        private ObservableCollection<ISeries> series;

        [ObservableProperty]
        private ObservableCollection<ICartesianAxis> xAxes;

        [ObservableProperty]
        private ObservableCollection<ICartesianAxis> yAxes;

        [ObservableProperty]
        private ObservableCollection<MenuItemViewModel>? contextMenuItems;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private SKColor? backgroundColor;

        protected readonly IOptionsMonitor<ColorConfiguration> _colorConfiguration;
        protected readonly IOptionsMonitor<ChartConfiguration> _chartConfiguration;

        [RelayCommand]
        public void PointerPressed(PointerCommandArgs args)
        {
            if (args.OriginalEventArgs is MouseButtonEventArgs orig)
            {
                if (orig.ChangedButton == MouseButton.Right && orig.ButtonState == MouseButtonState.Pressed)
                    return;
            }
            var foundPoints = args.Chart.GetPointsAt(args.PointerPosition);

            try
            {
                args.Chart.InvokeOnUIThread(() => //Placing this in an invoke to try to avoid an exception that occurs inside of the control
                {
                    foreach (var point in foundPoints.ToList())
                    {
                        if (point.Context.DataSource is DateTimePoint p)
                        {
                            _ = WeakReferenceMessenger.Default.Send(new ChartPointPressed(p.DateTime));
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                //Getting an Collection modified error sometimes when loading data and the mouse is moved over the active chart control.
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return;
            }
        }

        protected BaseChartViewModel(
            IOptionsMonitor<ColorConfiguration> colorConfiguration, IOptionsMonitor<ChartConfiguration> chartConfig) {
            _colorConfiguration = colorConfiguration;
            _chartConfiguration = chartConfig;
            Series = [];
            XAxes = [];
            YAxes = [];
            ContextMenuItems = [];
            BackgroundColor = Utilities.GetColorFromString(_colorConfiguration.CurrentValue.ChartBackgroundColor, SKColors.White);

            _colorConfiguration.OnChange( config => {
                BackgroundColor = Utilities.GetColorFromString(config.ChartBackgroundColor, SKColors.White);
                if(Series != null && Series.Count > 0)
                {
                    if(Series != null && Series.Count > 0)
                    {
                        if (Series[0] is LineSeries<DateTimePoint> lsPrimary)
                        {
                            lsPrimary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartPrimaryColor, ChartDefaults.Series1Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 1 && Series[1] is LineSeries<DateTimePoint> lsSecondary)
                        {
                            lsSecondary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartSecondaryColor, ChartDefaults.Series2Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 2 && Series[2] is LineSeries<DateTimePoint> lsTertiary)
                        {
                            lsTertiary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartTertiaryColor, ChartDefaults.Series3Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 3 && Series[3] is LineSeries<DateTimePoint> lsQuaternary)
                        {
                            lsQuaternary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartQuaternaryColor, ChartDefaults.Series4Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 4 && Series[4] is LineSeries<DateTimePoint> lsQuinary)
                        {
                            lsQuinary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartQuinaryColor, ChartDefaults.Series5Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 5 && Series[5] is LineSeries<DateTimePoint> lsSenary)
                        {
                            lsSenary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartSenaryColor, ChartDefaults.Series6Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 6 && Series[6] is LineSeries<DateTimePoint> lsSeptenary)
                        {
                            lsSeptenary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartSeptenaryColor, ChartDefaults.Series7Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 7 && Series[7] is LineSeries<DateTimePoint> lsOctonary)
                        {
                            lsOctonary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartOctonaryColor, ChartDefaults.Series8Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 8 && Series[8] is LineSeries<DateTimePoint> lsNonary)
                        {
                            lsNonary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartNonaryColor, ChartDefaults.Series9Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                        if (Series.Count >= 9 && Series[9] is LineSeries<DateTimePoint> lsDenary)
                        {
                            lsDenary.Stroke = new SolidColorPaint(Utilities.GetColorFromString(config.ChartDenaryColor, ChartDefaults.Series10Color)) { StrokeThickness = config.ChartLineThickness };
                        }
                    }
                }
            });
        }

        public abstract void Reset();

        public abstract void LoadData(ObservableCollection<TripLog> Events, int minMinutesBetweenTrip);

        [RelayCommand]
        protected void MenuItemClick(MenuItemViewModel item)
        {
            if (Series != null && Series.Count > 0)
            {
                var seriesItem = Series.First(e => e.Name == item.Header);
                if (seriesItem != null && item.IsChecked.HasValue)
                {
                    seriesItem.IsVisible = !item.IsChecked.Value;
                    seriesItem.IsVisibleAtLegend = !item.IsChecked.Value;
                }
            }
        }

        [RelayCommand]
        protected static void SaveAsImageContextMenuClick(MenuItemViewModel item)
        {
            _ = WeakReferenceMessenger.Default.Send(new SaveChartAsImageMessage(new ImageSettings() { SaveAs = SaveAs.File, SaveAsFormat = SaveAsFormat.PNG, Width = 0, Height = 0 }));
        }

        [RelayCommand]
        protected static void CopyToClipboardContextMenuClick(MenuItemViewModel item)
        {
            _ = WeakReferenceMessenger.Default.Send(new SaveChartAsImageMessage(new ImageSettings() { SaveAs = SaveAs.Image, SaveAsFormat = SaveAsFormat.PNG, Width = 0, Height = 0 }));
        }      

        public virtual void BuildContextMenuItems()
        {
            if(ContextMenuItems != null)
            {
                ContextMenuItems.Clear();
                foreach (var item in Series)
                {
                    System.Windows.Media.SolidColorBrush? textBrush = null;
                    if (item is LineSeries<DateTimePoint> ls && ls.Stroke is SolidColorPaint solidColorPaint)
                    {
                        textBrush = new System.Windows.Media.SolidColorBrush(solidColorPaint.Color.ToColor());
                    }

                    ContextMenuItems.Add(new MenuItemViewModel(item.Name ?? "Unknown", item.IsVisible, MenuItemClickCommand, textBrush) );
                }
                ContextMenuItems.Add(new MenuItemViewModel("Save as Image...", false, SaveAsImageContextMenuClickCommand, null));
                ContextMenuItems.Add(new MenuItemViewModel("Copy to Clipboard...", false, CopyToClipboardContextMenuClickCommand, null));
            }
        }

        protected static List<DateTimePoint> BuildDateTimePoints(
            IEnumerable<TripLog> events,
            Func<TripLog, double> valueSelector,
            int minMinutesBetweenTrip)
        {
            var points = events.Select(e => new DateTimePoint(e.DateTime, valueSelector(e))).ToList();
            var tsMaxdiff = TimeSpan.FromMinutes(minMinutesBetweenTrip);
            for (int i = 1; i < points.Count; i++)
            {
                if ((points[i].DateTime - points[i - 1].DateTime) > tsMaxdiff)
                {
                    points.Insert(i, new DateTimePoint(points[i].DateTime.AddSeconds(5), null));
                    i++;
                }
            }
            return points;
        }

        protected static List<DateTimePoint> BuildDeltaDateTimePoints(
            IEnumerable<TripLog> events,
            Func<TripLog, double> valueSelector,
            int minMinutesBetweenTrip)
        {
            var eventList = events.ToList();
            var points = new List<DateTimePoint>();

            var tsMaxdiff = TimeSpan.FromMinutes(minMinutesBetweenTrip);

            for (int i = 1; i < eventList.Count; i++)
            {
                var current = eventList[i];
                var previous = eventList[i - 1];
                var delta = valueSelector(current) - valueSelector(previous);
                if ((current.DateTime - previous.DateTime) > tsMaxdiff)
                {
                    points.Add(new DateTimePoint(current.DateTime.AddSeconds(-5), null));
                    i++;
                }

                points.Add(new DateTimePoint(current.DateTime, delta));
            }
            return points;
        }
    }
}