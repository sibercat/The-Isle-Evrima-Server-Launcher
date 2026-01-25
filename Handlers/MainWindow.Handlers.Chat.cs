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
    // Chat monitor handlers

    {
        // CHAT MONITOR HANDLERS
        // ==========================================

        private void chkEnableChatMonitor_Checked(object sender, RoutedEventArgs e)
        {
            if (_isLoadingConfig) return;

            var result = MessageBox.Show(
                "⚠️ Performance Warning\n\n" +
                "Chat monitoring polls the server log file continuously and may impact server performance.\n\n" +
                "Recommendations:\n" +
                "• Increase refresh interval to 5-10 seconds if experiencing lag\n" +
                "• Monitor server performance after enabling\n\n" +
                "Enable Chat Monitor?",
                "Performance Warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                chkEnableChatMonitor.IsChecked = false;
                return;
            }

            try
            {
                LoadRecentChatHistory(100);

                string logPath = System.IO.Path.Combine(_serverFolder, "TheIsle", "Saved", "Logs", "TheIsle.log");
                if (System.IO.File.Exists(logPath))
                {
                    var fileInfo = new System.IO.FileInfo(logPath);
                    _lastChatPosition = fileInfo.Length;
                    _logger.Info($"Chat monitor enabled. Starting from current log position: {_lastChatPosition}");
                }

                if (_chatMonitorTimer != null)
                {
                    _chatMonitorTimer.Start();
                }

                _logger.Info("Chat monitor enabled by user");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error enabling chat monitor: {ex.Message}", ex);
                MessageBox.Show($"Failed to enable chat monitor:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                chkEnableChatMonitor.IsChecked = false;
            }
        }

        private void chkEnableChatMonitor_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_chatMonitorTimer != null)
                {
                    _chatMonitorTimer.Stop();
                }

                rtbChatMonitor.Document.Blocks.Clear();

                var paragraph = new Paragraph { Margin = new Thickness(0) };
                paragraph.Inlines.Add(new Run("Chat Monitor is disabled. Enable above to start monitoring.")
                {
                    Foreground = Brushes.Gray,
                    FontStyle = FontStyles.Italic
                });
                rtbChatMonitor.Document.Blocks.Add(paragraph);

                _logger.Info("Chat monitor disabled by user");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error disabling chat monitor: {ex.Message}", ex);
            }
        }

        private void LoadRecentChatHistory(int maxMessages)
        {
            string logPath = System.IO.Path.Combine(_serverFolder, "TheIsle", "Saved", "Logs", "TheIsle.log");
            if (!System.IO.File.Exists(logPath)) return;

            try
            {
                var chatLines = new List<string>();

                using (var fs = new System.IO.FileStream(logPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                using (var reader = new System.IO.StreamReader(fs))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("LogTheIsleChatData:"))
                        {
                            chatLines.Add(line);
                        }
                    }
                }

                var recentMessages = chatLines.Skip(Math.Max(0, chatLines.Count - maxMessages)).ToList();

                foreach (var line in recentMessages)
                {
                    ParseAndDisplayChatLine(line, addToWebhookQueue: false);
                }

                _logger.Info($"Loaded {recentMessages.Count} recent chat messages for context");
            }
            catch (Exception ex)
            {
                _logger.Warning($"Failed to load recent chat history: {ex.Message}");
            }
        }

        private void ParseAndDisplayChatLine(string line, bool addToWebhookQueue = true)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                line, @"\[(Spatial|Global|Admin)\].*?\] ([^\[]+) \[[\d]+\]: (.+)");

            if (!match.Success) return;

            string chatType = match.Groups[1].Value;
            string playerName = match.Groups[2].Value.Trim();
            string message = match.Groups[3].Value;

            if (chatType == "Global" && chkMonitorGlobal.IsChecked != true) return;
            if (chatType == "Spatial" && chkMonitorSpatial.IsChecked != true) return;
            if (chatType == "Admin" && chkMonitorAdmin.IsChecked != true) return;

            Dispatcher.Invoke(() =>
            {
                var typeColor = chatType switch
                {
                    "Admin" => Brushes.OrangeRed,
                    "Global" => Brushes.CornflowerBlue,
                    "Spatial" => Brushes.MediumSeaGreen,
                    _ => Brushes.Gray
                };

                var paragraph = new Paragraph { Margin = new Thickness(0) };

                paragraph.Inlines.Add(new Run($"[{DateTime.Now:HH:mm:ss}] ") { Foreground = Brushes.Gray });

                string typeLabel = chatType == "Spatial" ? "Local" : chatType;
                paragraph.Inlines.Add(new Run($"[{typeLabel}] ") { Foreground = typeColor, FontWeight = FontWeights.Bold });

                paragraph.Inlines.Add(new Run($"{playerName}: "));

                paragraph.Inlines.Add(new Run(message));

                rtbChatMonitor.Document.Blocks.Add(paragraph);
                rtbChatMonitor.ScrollToEnd();

                if (addToWebhookQueue && chkEnableChatWebhook.IsChecked == true && !string.IsNullOrWhiteSpace(txtChatWebhookUrl.Text))
                {
                    lock (_chatBatchLock)
                    {
                        _pendingChatMessages.Add($"**[{typeLabel}]** {playerName}: {message}");
                    }
                }
            });
        }

        private void btnPauseChat_Click(object sender, RoutedEventArgs e)
        {
            _chatMonitorPaused = !_chatMonitorPaused;
            btnPauseChat.Content = _chatMonitorPaused ? "▶️ Resume" : "Pause";
        }

        private void btnClearChat_Click(object sender, RoutedEventArgs e)
        {
            rtbChatMonitor.Document.Blocks.Clear();
        }

        private void cmbChatRefreshInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chatMonitorTimer == null || cmbChatRefreshInterval.SelectedIndex < 0) return;

            int seconds = cmbChatRefreshInterval.SelectedIndex switch
            {
                0 => 1,
                1 => 2,
                2 => 5,
                3 => 10,
                _ => 1
            };

            _chatMonitorTimer.Interval = TimeSpan.FromSeconds(seconds);
            _logger.Info($"Chat monitor refresh interval changed to {seconds} second(s)");
        }

        private async void btnTestChatWebhook_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtChatWebhookUrl.Text)) return;
            btnTestChatWebhook.IsEnabled = false;
            try
            {
                await _discordWebhookService.SendChatTestNotificationAsync(txtChatWebhookUrl.Text, txtServerName.Text);
                ShowToast("✓ Test Chat Message Sent");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnTestChatWebhook.IsEnabled = true;
            }
        }

        private void ShowDataDialog(string title, string data)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 600,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["WindowBackgroundBrush"]
            };

            var txtData = new TextBox
            {
                Text = data,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12,
                Margin = new Thickness(10),
                IsReadOnly = true,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["InputBackgroundBrush"],
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["InputTextBrush"]
            };

            dialog.Content = txtData;
            dialog.ShowDialog();
        }
    }
}
