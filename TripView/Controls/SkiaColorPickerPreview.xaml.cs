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
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows;
using System.Windows.Input;

namespace TripView.Controls
{
    /// <summary>
    /// Interaction logic for SkiaColorPicker.xaml
    /// </summary>
    public partial class SkiaColorPickerPreview : System.Windows.Controls.UserControl
    {

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(SKColor),
                typeof(SkiaColorPickerPreview),
                new FrameworkPropertyMetadata(SKColor.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));

        public SKColor SelectedColor
        {
            get => (SKColor)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(
                nameof(Caption),
                typeof(string),
                typeof(SkiaColorPickerPreview),
                new PropertyMetadata(string.Empty));

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty CaptionWidthProperty =
            DependencyProperty.Register(
                nameof(CaptionWidth),
                typeof(int),
                typeof(SkiaColorPickerPreview),
                new PropertyMetadata(125));

        public string CaptionWidth
        {
            get => (string)GetValue(CaptionWidthProperty);
            set => SetValue(CaptionWidthProperty, value);
        }

        public SkiaColorPickerPreview()
        {
            InitializeComponent();
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SkiaColorPickerPreview;
            SKColor oldValue = (SKColor)e.OldValue;
            SKColor newValue = (SKColor)e.NewValue;

            control?.OnSelectedColorChanged(oldValue, newValue);
        }

        private void OnSelectedColorChanged(SKColor oldValue, SKColor newValue)
        {
            PreviewColorElement.InvalidateVisual();
        }

        private void PreviewColorElement_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            using var paint = new SKPaint() { Color = SelectedColor };
            canvas.DrawPaint(paint);
        }

        private void PreviewColorElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new SkiaColorPickerDialog();
            dlg.Owner = Window.GetWindow(this);
            dlg.Title = $"Select Color for {Caption}";
            dlg.SelectedColor = SelectedColor;
            if (dlg.ShowDialog() == true)
            {
                SelectedColor = dlg.SelectedColor;
            }
        }
    }
}
