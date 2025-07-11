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
using System.Reflection;
using System.Windows;

namespace TripView
{

    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutViewModel Libraries { get; set; } = new();

        public AboutWindow()
        {
            InitializeComponent();
            DataContext = Libraries;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal static string GetAssemblyVersion(string assemblyName)
        {
            try
            {
                Assembly? ass = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name?.Contains(assemblyName, StringComparison.CurrentCultureIgnoreCase) == true);
                if (ass != null)
                {
                    var version = ass.GetName().Version;
                    return version?.ToString() ?? "Unknown";
                }
                return "Unknown";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
