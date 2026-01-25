using IsleServerLauncher.Services;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IsleServerLauncher
{
    public partial class MainWindow : Window
    {
        private void mnuSavePreset_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveSettings(true)) return;

            string? presetName = PromptPresetName("Save Preset", txtServerName.Text);
            if (presetName == null) return;

            if (_presetManager.SavePreset(presetName, overwrite: false, out string error))
            {
                ShowToast("Preset saved");
                return;
            }

            if (error == "Preset already exists.")
            {
                var result = MessageBox.Show(
                    "Preset already exists. Overwrite it?",
                    "Confirm Overwrite",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes &&
                    _presetManager.SavePreset(presetName, overwrite: true, out error))
                {
                    ShowToast("Preset saved");
                    return;
                }
            }

            MessageBox.Show($"Failed to save preset: {error}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void mnuLoadPreset_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfDirty()) return;

            string? presetName = SelectPresetFromList("Load Preset");
            if (presetName == null) return;

            if (_presetManager.LoadPreset(presetName, out string error))
            {
                LoadConfiguration();
                ShowToast("Preset loaded");
                return;
            }

            MessageBox.Show($"Failed to load preset: {error}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void mnuExportPreset_Click(object sender, RoutedEventArgs e)
        {
            string? presetName = SelectPresetFromList("Export Preset");
            if (presetName == null) return;

            var dialog = new SaveFileDialog
            {
                Title = "Export Preset",
                Filter = "Isle Preset (*.islepreset)|*.islepreset|All files (*.*)|*.*",
                DefaultExt = ".islepreset",
                FileName = presetName + ".islepreset"
            };

            if (dialog.ShowDialog() == true)
            {
                if (_presetManager.ExportPreset(presetName, dialog.FileName, overwrite: true, out string error))
                {
                    ShowToast("Preset exported");
                    return;
                }

                MessageBox.Show($"Failed to export preset: {error}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mnuImportPreset_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfDirty()) return;

            var dialog = new OpenFileDialog
            {
                Title = "Import Preset",
                Filter = "Isle Preset (*.islepreset)|*.islepreset|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true) return;

            string defaultName = Path.GetFileNameWithoutExtension(dialog.FileName);
            string? presetName = PromptPresetName("Import Preset", defaultName);
            if (presetName == null) return;

            if (_presetManager.ImportPreset(dialog.FileName, presetName, overwrite: false, out string error))
            {
                if (_presetManager.LoadPreset(presetName, out string loadError))
                {
                    LoadConfiguration();
                    ShowToast("Preset imported and loaded");
                    return;
                }

                MessageBox.Show($"Imported preset but failed to load: {loadError}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (error == "Preset already exists.")
            {
                var result = MessageBox.Show(
                    "Preset already exists. Overwrite it?",
                    "Confirm Overwrite",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes &&
                    _presetManager.ImportPreset(dialog.FileName, presetName, overwrite: true, out error))
                {
                    if (_presetManager.LoadPreset(presetName, out string loadError))
                    {
                        LoadConfiguration();
                        ShowToast("Preset imported and loaded");
                        return;
                    }

                    MessageBox.Show($"Imported preset but failed to load: {loadError}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            MessageBox.Show($"Failed to import preset: {error}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void mnuManagePresets_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window
            {
                Title = "Manage Presets",
                Width = 420,
                Height = 360,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var list = new ListBox { Margin = new Thickness(10) };
            var btnRename = new Button { Content = "Rename", Width = 90, Height = 30, Margin = new Thickness(10) };
            var btnDelete = new Button { Content = "Delete", Width = 90, Height = 30, Margin = new Thickness(10) };
            var btnClose = new Button { Content = "Close", Width = 90, Height = 30, Margin = new Thickness(10) };

            void RefreshList()
            {
                list.Items.Clear();
                foreach (var preset in _presetManager.GetPresetFolders()
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .OrderBy(name => name))
                {
                    list.Items.Add(preset);
                }
            }

            RefreshList();

            btnRename.Click += (_, __) =>
            {
                if (list.SelectedItem == null) return;
                string oldName = list.SelectedItem.ToString() ?? "";
                string? newName = PromptPresetName("Rename Preset", oldName);
                if (newName == null || string.Equals(oldName, newName, StringComparison.Ordinal)) return;

                if (_presetManager.RenamePreset(oldName, newName, overwrite: false, out string error))
                {
                    RefreshList();
                    return;
                }

                if (error == "Preset already exists.")
                {
                    var result = MessageBox.Show(
                        "Preset already exists. Overwrite it?",
                        "Confirm Overwrite",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes &&
                        _presetManager.RenamePreset(oldName, newName, overwrite: true, out error))
                    {
                        RefreshList();
                        return;
                    }
                }

                MessageBox.Show($"Failed to rename preset: {error}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            btnDelete.Click += (_, __) =>
            {
                if (list.SelectedItem == null) return;
                string name = list.SelectedItem.ToString() ?? "";
                var result = MessageBox.Show(
                    $"Delete preset \"{name}\"?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                if (_presetManager.DeletePreset(name, out string error))
                {
                    RefreshList();
                    return;
                }

                MessageBox.Show($"Failed to delete preset: {error}", "Preset Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            btnClose.Click += (_, __) => dialog.Close();

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            buttons.Children.Add(btnRename);
            buttons.Children.Add(btnDelete);
            buttons.Children.Add(btnClose);

            var panel = new DockPanel();
            DockPanel.SetDock(buttons, Dock.Bottom);
            panel.Children.Add(buttons);
            panel.Children.Add(list);

            dialog.Content = panel;
            dialog.ShowDialog();
        }

        private void mnuOpenPresetsFolder_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(_presetManager.PresetsFolder);
            Process.Start("explorer.exe", _presetManager.PresetsFolder);
        }

        private bool ConfirmSaveIfDirty()
        {
            if (!_isDirty) return true;

            var result = MessageBox.Show(
                "You have unsaved changes. Save before loading a preset?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Cancel) return false;
            if (result == MessageBoxResult.Yes) return SaveSettings(true);

            return true;
        }

        private string? SelectPresetFromList(string title)
        {
            string[] presets = _presetManager.GetPresetFolders()
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!)
                .OrderBy(name => name)
                .ToArray();

            if (presets.Length == 0)
            {
                MessageBox.Show("No presets found.", title, MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            var dialog = new Window
            {
                Title = title,
                Width = 380,
                Height = 320,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var list = new ListBox { Margin = new Thickness(10) };
            foreach (var preset in presets) list.Items.Add(preset);

            var btnOk = new Button { Content = "OK", Width = 90, Height = 30, Margin = new Thickness(10) };
            var btnCancel = new Button { Content = "Cancel", Width = 90, Height = 30, Margin = new Thickness(10) };

            btnOk.Click += (_, __) => dialog.DialogResult = true;
            btnCancel.Click += (_, __) => dialog.DialogResult = false;

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            buttons.Children.Add(btnOk);
            buttons.Children.Add(btnCancel);

            var panel = new DockPanel();
            DockPanel.SetDock(buttons, Dock.Bottom);
            panel.Children.Add(buttons);
            panel.Children.Add(list);

            dialog.Content = panel;

            if (dialog.ShowDialog() == true && list.SelectedItem != null)
            {
                return list.SelectedItem.ToString();
            }

            return null;
        }

        private string? PromptPresetName(string title, string defaultName)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 360,
                Height = 160,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var textBox = new TextBox
            {
                Margin = new Thickness(10, 10, 10, 0),
                Text = defaultName ?? ""
            };

            var btnOk = new Button { Content = "OK", Width = 90, Height = 30, Margin = new Thickness(10) };
            var btnCancel = new Button { Content = "Cancel", Width = 90, Height = 30, Margin = new Thickness(10) };
            btnOk.IsDefault = true;
            btnCancel.IsCancel = true;

            btnOk.Click += (_, __) => dialog.DialogResult = true;
            btnCancel.Click += (_, __) => dialog.DialogResult = false;

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            buttons.Children.Add(btnOk);
            buttons.Children.Add(btnCancel);

            var panel = new DockPanel();
            DockPanel.SetDock(buttons, Dock.Bottom);
            panel.Children.Add(buttons);
            panel.Children.Add(textBox);
            dialog.Content = panel;

            if (dialog.ShowDialog() == true)
            {
                string name = textBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Preset name is required.", title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                return name;
            }

            return null;
        }
    }
}
