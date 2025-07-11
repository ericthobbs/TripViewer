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
using Microsoft.Xaml.Behaviors;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace TripView.Behaviors
{
    public class NumericRange
    {
        public double Min { get; }
        public double Max { get; }

        public NumericRange(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public bool IsInRange(double value) => value >= Min && value <= Max;
    }

    public class ValidateNumericInputWithRangeBehavior : Behavior<System.Windows.Controls.TextBox>
    {
        private string DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        public static readonly System.Windows.DependencyProperty CommandProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Command),
                typeof(System.Windows.Input.ICommand),
                typeof(ValidateNumericInputWithRangeBehavior),
                new System.Windows.PropertyMetadata(null)
            );

        public System.Windows.Input.ICommand Command
        {
            get => (System.Windows.Input.ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(NumericRange),
                typeof(ValidateNumericInputWithRangeBehavior),
                new PropertyMetadata(null));

        public NumericRange CommandParameter
        {
            get => (NumericRange)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
        }

        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string preview = GetPreviewText(AssociatedObject, e.Text);

            if (!AllowNumericOnly(preview) || !IsWithinRange(preview) )
            {
                e.Handled = true;
                return;
            }

            if (Command?.CanExecute(e) == true)
            {
                Command.Execute(e);
            }
        }

        private string GetPreviewText(System.Windows.Controls.TextBox textBox, string input)
        {
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;
            string before = textBox.Text.Substring(0, start);
            string after = textBox.Text.Substring(start + length);
            return before + input + after;
        }

        private bool AllowNumericOnly(string fulltext)
        {
            foreach (var c in fulltext)
            {
                if (!char.IsDigit(c) && c != '-' && c.ToString() != DecimalSeparator)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsWithinRange(string text)
        {
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out var value))
                return false;
            
            return CommandParameter.IsInRange(value);
        }
    }
}
