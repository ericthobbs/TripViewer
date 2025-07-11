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
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows;

namespace TripView.Controls
{
    /// <summary>
    /// Interaction logic for SkiaColorPickerDialog.xaml
    /// </summary>
    public partial class SkiaColorPickerDialog : Window
    {
        public bool _canUpdate = true;
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(SKColor),
                typeof(SkiaColorPickerDialog),
                new PropertyMetadata(SKColors.Empty, OnSelectedColorChanged));

        public SKColor SelectedColor
        {
            get => (SKColor)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly DependencyProperty AlphaChannelProperty =
            DependencyProperty.Register(
                nameof(AlphaChannel),
                typeof(int),
                typeof(SkiaColorPickerDialog),
                new PropertyMetadata(0, OnChannelChanged));

        public int AlphaChannel
        {
            get => (int)GetValue(AlphaChannelProperty);
            set => SetValue(AlphaChannelProperty, value);
        }

        public static readonly DependencyProperty RedChannelProperty =
            DependencyProperty.Register(
                nameof(RedChannel),
                typeof(int),
                typeof(SkiaColorPickerDialog),
                new PropertyMetadata(0, OnChannelChanged));

        public int RedChannel
        {
            get => (int)GetValue(RedChannelProperty);
            set => SetValue(RedChannelProperty, value);
        }

        public static readonly DependencyProperty GreenChannelProperty =
            DependencyProperty.Register(
                nameof(GreenChannel),
                typeof(int),
                typeof(SkiaColorPickerDialog),
                new PropertyMetadata(0, OnChannelChanged));

        public int GreenChannel
        {
            get => (int)GetValue(GreenChannelProperty);
            set => SetValue(GreenChannelProperty, value);
        }

        public static readonly DependencyProperty BlueChannelProperty =
            DependencyProperty.Register(
                nameof(BlueChannel),
                typeof(int),
                typeof(SkiaColorPickerDialog),
                new PropertyMetadata(0, OnChannelChanged));

        public int BlueChannel
        {
            get => (int)GetValue(BlueChannelProperty);
            set => SetValue(BlueChannelProperty, value);
        }

        public SkiaColorPickerDialog()
        {
            SelectedColor = SKColors.Gold;
            InitializeComponent();
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SkiaColorPickerDialog;
            SKColor oldValue = (SKColor)e.OldValue;
            SKColor newValue = (SKColor)e.NewValue;

            control?.OnSelectedColorChanged(oldValue, newValue);
        }

        private void OnSelectedColorChanged(SKColor oldValue, SKColor newValue)
        {
            _canUpdate = false;
            GreenChannel = SelectedColor.Green;
            BlueChannel = SelectedColor.Blue;
            RedChannel = SelectedColor.Red;
            AlphaChannel = SelectedColor.Alpha;
            _canUpdate = true;
            PreviewColorElement?.InvalidateVisual();
        }

        private static void OnChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SkiaColorPickerDialog;
            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;

            control?.OnChannelChanged(oldValue, newValue);
        }

        private void OnChannelChanged(int oldValue, int newValue)
        {
            if (!_canUpdate)
                return;

            SelectedColor = new SKColor(
                Convert.ToByte(RedChannel), Convert.ToByte(GreenChannel), Convert.ToByte(BlueChannel), Convert.ToByte(AlphaChannel));
        }

        private void PreviewColorElement_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            using var paint = new SKPaint() { Color = SelectedColor };
            canvas.DrawPaint(paint);
        }

        [RelayCommand]
        private void Ok()
        {
            DialogResult = true;
            Close();
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
