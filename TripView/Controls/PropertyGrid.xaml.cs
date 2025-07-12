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
using System.Windows;

namespace TripView.Controls
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGrid : System.Windows.Controls.UserControl
    {
        private readonly System.Windows.Forms.PropertyGrid _propertyGrid;
        private int PrevScrollPos = -1;
        public PropertyGrid()
        {
            InitializeComponent();
            _propertyGrid = new System.Windows.Forms.PropertyGrid();
            _propertyGrid.Dock = DockStyle.Fill;
            Host.Child = _propertyGrid;
            Host.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Host.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        }

        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(PropertyGrid),
                new PropertyMetadata(null, OnSelectedObjectChanged));

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyGrid grid)
            {
                //https://stackoverflow.com/questions/54914511/propertygrid-scroll-position-not-changing-when-set
                var vScroll = grid._propertyGrid.Controls.OfType<Control>().Where(ctl => ctl.AccessibilityObject.Role == AccessibleRole.Table).First().Controls.OfType<VScrollBar>().First();
                var val = vScroll.Value;
                grid._propertyGrid.SelectedObject = e.NewValue;
                vScroll.Value = grid.PrevScrollPos == -1 ? 0 : grid.PrevScrollPos;
                grid.PrevScrollPos = val;
            }
        }
    }
}
