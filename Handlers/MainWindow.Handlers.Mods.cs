using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Forms = System.Windows.Forms;

namespace IsleServerLauncher
{
    public partial class MainWindow : Window
    // Mods handlers
    {
        private string GetAutoInjectScriptPath()
        {
            return Path.Combine(_serverFolder, "TheIsle", "Binaries", "Win64", "StartServerWithMod.bat");
        }

        internal void chkAutoInjectAfterRestart_Checked(object sender, RoutedEventArgs e)
        {
            if (_isLoadingConfig) return;
            if (string.IsNullOrWhiteSpace(txtAutoInjectDelaySeconds.Text))
            {
                txtAutoInjectDelaySeconds.Text = "5";
            }
        }

        internal void chkAutoInjectAfterRestart_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isLoadingConfig) return;
        }

        internal void txtAutoInjectDelaySeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoadingConfig) return;
        }

        internal void btnBrowseModLoader_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Executable (*.exe)|*.exe|All Files (*.*)|*.*",
                Title = "Select Mod Loader (IsleModLoader.exe)"
            };

            if (dialog.ShowDialog() == true)
            {
                txtModLoaderPath.Text = dialog.FileName;
            }
        }

        internal void btnBrowseModDll_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "DLL (*.dll)|*.dll|All Files (*.*)|*.*",
                Title = "Select Mod DLL (IsleServerMod.dll)"
            };

            if (dialog.ShowDialog() == true)
            {
                txtModDllPath.Text = dialog.FileName;
            }
        }

        internal void btnBrowseModConfig_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new Forms.FolderBrowserDialog
            {
                Description = "Select Mod Config Folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            })
            {
                if (dialog.ShowDialog() == Forms.DialogResult.OK)
                {
                    txtModConfigDir.Text = dialog.SelectedPath;
                }
            }
        }

        internal void btnAutoFillModPaths_Click(object sender, RoutedEventArgs e)
        {
            string binDir = Path.Combine(_serverFolder, "TheIsle", "Binaries", "Win64");
            txtModLoaderPath.Text = Path.Combine(binDir, "IsleModLoader.exe");
            txtModDllPath.Text = Path.Combine(binDir, "IsleServerMod.dll");
            txtModConfigDir.Text = Path.Combine(binDir, "config");
        }

        internal void btnOpenModConfig_Click(object sender, RoutedEventArgs e)
        {
            string dir = txtModConfigDir.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                MessageBox.Show("Mod config folder not found. Please set a valid path.", "Missing Folder",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = dir,
                UseShellExecute = true
            });
        }

        internal void btnInjectMod_Click(object sender, RoutedEventArgs e)
        {
            if (rdoInjectBat.IsChecked == true)
            {
                TryRunModBatInjection(showUi: true);
                return;
            }

            TryInjectModWithLoader(showUi: true);
        }

        private bool TryRunModBatInjection(bool showUi)
        {
            string scriptPath = GetAutoInjectScriptPath();
            if (!File.Exists(scriptPath))
            {
                string message = "StartServerWithMod.bat not found. Please verify your server files.";
                if (showUi)
                {
                    MessageBox.Show(message, "Missing Script", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                AppendModLog(message);
                return false;
            }

            try
            {
                AppendModLog("Starting StartServerWithMod.bat...");
                var psi = new ProcessStartInfo
                {
                    FileName = scriptPath,
                    WorkingDirectory = Path.GetDirectoryName(scriptPath) ?? "",
                    UseShellExecute = true
                };
                Process.Start(psi);
                AppendModLog("StartServerWithMod.bat launched.");
                return true;
            }
            catch (Exception ex)
            {
                AppendModLog($"Failed to launch StartServerWithMod.bat: {ex.Message}");
                if (showUi)
                {
                    MessageBox.Show($"Failed to launch StartServerWithMod.bat:\n\n{ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
        }

        private bool TryInjectModWithLoader(bool showUi)
        {
            string loaderPath = txtModLoaderPath.Text?.Trim() ?? "";
            string dllPath = txtModDllPath.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(loaderPath) || !File.Exists(loaderPath))
            {
                string message = "Mod loader not found. Please set the loader path.";
                if (showUi)
                {
                    MessageBox.Show(message, "Missing Loader", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                AppendModLog(message);
                return false;
            }

            if (string.IsNullOrWhiteSpace(dllPath) || !File.Exists(dllPath))
            {
                string message = "Mod DLL not found. Please set the DLL path.";
                if (showUi)
                {
                    MessageBox.Show(message, "Missing DLL", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                AppendModLog(message);
                return false;
            }

            if (!IsServerRunning())
            {
                string message = "Server must be running to inject the mod.";
                if (showUi)
                {
                    MessageBox.Show(message, "Server Not Running", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                AppendModLog(message);
                return false;
            }

            try
            {
                AppendModLog("Starting mod injection...");

                var psi = new ProcessStartInfo
                {
                    FileName = loaderPath,
                    Arguments = $"\"TheIsleServer-Win64-Shipping.exe\" \"{dllPath}\"",
                    WorkingDirectory = Path.GetDirectoryName(loaderPath) ?? "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(psi))
                {
                    if (proc == null)
                    {
                        AppendModLog("Failed to start loader process.");
                        return false;
                    }

                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit(15000);

                    if (!string.IsNullOrWhiteSpace(output))
                        AppendModLog(output.Trim());
                    if (!string.IsNullOrWhiteSpace(error))
                        AppendModLog(error.Trim());

                    AppendModLog(proc.ExitCode == 0 ? "Injection completed." : $"Injection finished with exit code {proc.ExitCode}.");
                    return proc.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                AppendModLog($"Injection failed: {ex.Message}");
                if (showUi)
                {
                    MessageBox.Show($"Injection failed:\n\n{ex.Message}", "Injection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
        }

        private bool IsServerRunning()
        {
            return Process.GetProcessesByName("TheIsleServer-Win64-Shipping").Length > 0;
        }

        private void AppendModLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtModLog.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            txtModLog.ScrollToEnd();
        }
    }
}
