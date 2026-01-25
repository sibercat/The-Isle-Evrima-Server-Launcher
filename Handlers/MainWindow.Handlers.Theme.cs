using IsleServerLauncher.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;

namespace IsleServerLauncher
{
    public partial class MainWindow : Window
    // Theme handlers

    {
        // THEME HANDLERS
        // ==========================================

        private void mnuLightMode_Click(object sender, RoutedEventArgs e)
        {
            // Logic moved to MainWindow.xaml.cs to ensure single source of truth for _currentTheme
            // Calling base handler if needed or ensuring event is hooked up in XAML to the method in MainWindow.xaml.cs
            // Since this is a partial class, the method in MainWindow.xaml.cs will handle it if defined there.
            // If defined here, use:

            if (_currentTheme == "Light") return;
            _currentTheme = "Light";
            _themeManager.ApplyTheme("Light");
            mnuLightMode.IsChecked = true;
            mnuDarkMode.IsChecked = false;
            SaveSettings(true);
        }

        private void mnuDarkMode_Click(object sender, RoutedEventArgs e)
        {
            // Logic moved to MainWindow.xaml.cs to ensure single source of truth for _currentTheme
            // But if the XAML points here:

            if (_currentTheme == "Dark") return;
            _currentTheme = "Dark";
            _themeManager.ApplyTheme("Dark");
            mnuDarkMode.IsChecked = true;
            mnuLightMode.IsChecked = false;
            SaveSettings(true);
        }

        // ==========================================
    }
}
