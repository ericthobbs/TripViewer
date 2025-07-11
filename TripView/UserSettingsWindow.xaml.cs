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
using System.Windows;
using TripView.Behaviors;
using TripView.Configuration;
using TripView.ViewModels;

namespace TripView
{
    /// <summary>
    /// Interaction logic for UserSettingsWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class UserSettingsWindow : Window
    {
        private readonly UserSettingsManager _userSettings;

        [ObservableProperty]
        private SettingsViewModel viewModel;

        [ObservableProperty]
        private NumericRange latitudeRange = new(-90f,+90f);

        [ObservableProperty]
        private NumericRange longitudeRange = new(-180f, +180f);

        [ObservableProperty]
        private NumericRange zoomLevelRange = new(0, 19);

        [ObservableProperty]
        private NumericRange zoomSecondsRange = new(0, 10);

        [ObservableProperty]
        private NumericRange minutesBetweenRange = new(0, 1440);

        [ObservableProperty]
        private NumericRange labelRotationRange = new(-90, 90);

        [ObservableProperty]
        private NumericRange lineSizeRange = new(0, 100);

        public UserSettingsWindow(UserSettingsManager userSettingsManager, SettingsViewModel vm)
        {
            DataContext = ViewModel = vm;
            _userSettings = userSettingsManager;
            InitializeComponent();
        }

        [RelayCommand]
        private void OkButton()
        {
            _userSettings.Save(
                ViewModel.ColorConfig.ToColorConfiguration(), 
                ViewModel.ChartConfig.ToChartConfiguration(), 
                ViewModel.StartupConfig.ToStartupConfiguration(), 
                ViewModel.ImportConfig.ToLeafSpyImportConfiguration());
            Close();
        }

        [RelayCommand]
        private void RestoreDefaults()
        {
            var result = System.Windows.MessageBox.Show(
                $"Are you sure you wish to restore TripView to its default settings.\n\n"+
                $"This will remove {System.IO.Path.GetFileName(UserSettingsManager.UserSettingsFile)}.\n"+
                $"This operation cannot be undone.",
                "Reset TripView",MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                _userSettings.DeleteUserSettings();
                Close();
            }
        }

        [RelayCommand]
        private void CancelButton()
        {
            Close();
        }
    }
}
