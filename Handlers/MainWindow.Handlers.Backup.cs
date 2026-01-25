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
    // Backup handlers

    {
        // BACKUP HANDLERS
        // ==========================================

        private async void btnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            btnCreateBackup.IsEnabled = false;
            btnCreateBackup.Content = "Creating...";
            try
            {
                string path = await _backupService.CreateBackupAsync();
                ShowToast("✓ Backup Created Successfully");
                _backupService.CleanupOldBackups(10);
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error"); }
            finally { btnCreateBackup.IsEnabled = true; btnCreateBackup.Content = "Create Backup"; }
        }

        private async void btnRestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            if (_serverManager.CurrentState != ServerState.Stopped && _serverManager.CurrentState != ServerState.NotInstalled)
            {
                MessageBox.Show("Server must be stopped.", "Error");
                return;
            }

            var backups = _backupService.GetAvailableBackups();
            if (backups.Length == 0) { MessageBox.Show("No backups found.", "Error"); return; }

            var dialog = new Window { Title = "Select Backup", Width = 400, Height = 300, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            var list = new ListBox { Margin = new Thickness(5) };
            foreach (var b in backups) list.Items.Add(System.IO.Path.GetFileName(b));

            var btnRestore = new Button { Content = "Restore", Height = 30, Margin = new Thickness(5) };
            btnRestore.Click += (s, args) => dialog.DialogResult = true;

            var panel = new DockPanel();
            panel.Children.Add(btnRestore); DockPanel.SetDock(btnRestore, Dock.Bottom);
            panel.Children.Add(list);
            dialog.Content = panel;

            if (dialog.ShowDialog() == true && list.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Overwrite server data?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _backupService.RestoreBackupAsync(backups[list.SelectedIndex]);
                        ShowToast("✓ Backup Restored Successfully");
                    }
                    catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error"); }
                }
            }
        }

        private void btnOpenBackupFolder_Click(object sender, RoutedEventArgs e)
        {
            string folder = System.IO.Path.Combine(_serverFolder, "Backups");
            if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
            Process.Start("explorer.exe", folder);
        }

        // ==========================================
    }
}
