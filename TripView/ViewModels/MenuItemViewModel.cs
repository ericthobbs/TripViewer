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
using System.Collections.ObjectModel;

namespace TripView.ViewModels
{
    public interface IMenuItem
    {

    }

    public partial class MenuItemViewModel : ObservableObject, IMenuItem
    {
        [ObservableProperty]
        private string header;

        [ObservableProperty]
        private bool? isChecked;

        [ObservableProperty]
        private IRelayCommand? command;

        [ObservableProperty]
        private bool isEnabled;

        [ObservableProperty]
        private System.Windows.Media.Brush? foreground;

        [ObservableProperty]
        private ObservableCollection<IMenuItem>? items = [];

        public MenuItemViewModel(string header, bool? isChecked, IRelayCommand? command, System.Windows.Media.Brush? foreground, bool isEnabled = true)
        {
            Header = header;
            IsEnabled = isEnabled;
            IsChecked = isChecked;
            Command = command;
            Foreground = foreground;
        }
    }

    public class SeperatorItemViewModel : IMenuItem
    {

    }
}