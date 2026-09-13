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
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace IsleServerLauncher
{
    public partial class MainWindow : Window
    // System setup handlers

    {
        private const string GitHubLatestReleaseApi = "https://api.github.com/repos/sibercat/The-Isle-Evrima-Server-Launcher/releases/latest";
        private const string GitHubReleasesPage = "https://github.com/sibercat/The-Isle-Evrima-Server-Launcher/releases";
        private static readonly HttpClient _httpClient = new HttpClient();
        // SYSTEM SETUP HANDLERS
        // ==========================================

        private async void btnServerSetup_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This will configure your Windows Server for The Isle:\n\n• Open firewall ports\n• Install VC++\n• Disable IE Security\n\nAdministrator privileges required. Continue?",
                "Server Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                if (!_systemSetup.IsAdministrator()) { MessageBox.Show("Run as Admin required.", "Error"); return; }
                await _systemSetup.RunServerSetupAsync();
                ShowToast("✓ Server Setup Complete");
            }
            catch (Exception ex) { MessageBox.Show($"Setup failed: {ex.Message}", "Error"); }
        }

        private async void btnNetworkFix_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This optimizes drivers for VPS/Cloud Servers (VirtIO fix).\n\nRequires restart. Continue?",
                "Network Optimization", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                if (!_systemSetup.IsAdministrator()) { MessageBox.Show("Run as Admin required.", "Error"); return; }
                await _systemSetup.RunNetworkOptimizationAsync();
                ShowToast("✓ Network Optimization Complete");
                MessageBox.Show("Please restart your VPS/Server for changes to take effect.", "Restart Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Optimization failed: {ex.Message}", "Error"); }
        }

        private async void btnFixSSL_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Check whether this machine trusts Epic Online Services' certificates?\n\n" +
                "The launcher connects to Epic's servers, which lets Windows add a missing trusted root " +
                "certificate on its own. If that isn't enough, you will be asked before anything else is installed.",
                "Fix SSL Certificate Error", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            if (!_systemSetup.IsAdministrator()) { MessageBox.Show("Run as Admin required.", "Error"); return; }

            var menuItem = sender as MenuItem;
            if (menuItem != null) menuItem.IsEnabled = false;

            try
            {
                ShowToast("Checking Epic certificates...");
                _logger.Info("=== SSL certificate check started ===");

                var rootsBefore = _systemSetup.GetMachineRootCertificates();
                var results = await _systemSetup.CheckEpicCertificatesAsync();
                RootInstallResult? install = null;
                bool fallbackDeclined = false;

                // The check is also the fix: Windows fetches a missing root while building the chain,
                // which normally lands inside that same handshake. If the download was slower than
                // the handshake, the first result can still say "not trusted" on a machine that is
                // now fine, so re-check once when the store actually grew rather than asking for a
                // machine-wide root install nobody needs.
                if (results.Any(r => r.Connected && !r.Trusted) &&
                    _systemSetup.GetMachineRootCertificates().Count != rootsBefore.Count)
                {
                    _logger.Info("Root store changed during the check; re-checking before offering the root install.");
                    results = await _systemSetup.CheckEpicCertificatesAsync();
                }

                // Installing roots can only help a chain that arrived and wasn't trusted. A connection
                // that never got that far is a network problem, and the report says so instead.
                var untrusted = results.Where(r => r.Connected && !r.Trusted).Select(r => r.Host).ToList();
                if (untrusted.Count > 0)
                {
                    var fallback = MessageBox.Show(
                        $"Windows still doesn't trust the certificate from {string.Join(" and ", untrusted)}.\n\n" +
                        "Install all of Microsoft's trusted root certificates from Windows Update? These are the " +
                        "roots Windows already trusts; this adds them to the machine's Trusted Root store so the " +
                        "game server can find them.\n\n" +
                        "Choose No to see the details without installing anything.",
                        "Install Trusted Root Certificates", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (fallback == MessageBoxResult.Yes)
                    {
                        ShowToast("Installing trusted root certificates...");
                        install = await _systemSetup.InstallMicrosoftTrustedRootsAsync();
                        results = await _systemSetup.CheckEpicCertificatesAsync();
                    }
                    else
                    {
                        fallbackDeclined = true;
                        _logger.Info("User declined installing Microsoft trusted roots.");
                    }
                }

                var rootsAfter = _systemSetup.GetMachineRootCertificates();
                var verdict = SystemSetupService.GetVerdict(results);
                _logger.Info($"=== SSL certificate check finished: {verdict}, " +
                             $"machine root store {rootsBefore.Count} -> {rootsAfter.Count} ===");

                string toast = verdict switch
                {
                    CertificateVerdict.Trusted => "✓ Epic certificates are trusted",
                    CertificateVerdict.NotTrusted => "Epic certificates are still not trusted",
                    _ => "Could not reach Epic - see the report"
                };
                ShowToast(toast, isError: verdict != CertificateVerdict.Trusted);
                ShowGuide("SSL Certificate Check", SystemSetupService.BuildCertificateReport(results, rootsBefore, rootsAfter, install, fallbackDeclined));
            }
            catch (Exception ex)
            {
                _logger.Error($"SSL certificate check failed: {ex.Message}", ex);
                MessageBox.Show($"Certificate check failed: {ex.Message}", "Error");
            }
            finally
            {
                if (menuItem != null) menuItem.IsEnabled = true;
            }
        }

        private void btnTroubleshooting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1. Client stuck connecting? Run InstallAntiCheat.bat in game folder.\n2. SSL Errors? Use the Help menu SSL fix.", "Troubleshooting");
        }

        /// <summary>
        /// Shows a long-form guide in a scrollable window. MessageBox truncates awkwardly and
        /// can't be scrolled, which makes it useless for anything step-by-step.
        /// </summary>
        private void ShowGuide(string title, string body)
        {
            var text = new TextBox
            {
                Text = body,
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Foreground = (Brush)FindResource("PrimaryTextBrush"),
                // Fully qualified: System.Drawing is also in scope here and has its own FontFamily.
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12.5,
                Margin = new Thickness(14),
                // Selectable so an admin can copy a command straight out of the guide.
                IsReadOnlyCaretVisible = true
            };

            var window = new Window
            {
                Title = title,
                Width = 760,
                Height = 660,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                Content = new ScrollViewer
                {
                    Content = text,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
                }
            };

            window.ShowDialog();
        }

        private void btnConnectionGuide_Click(object sender, RoutedEventArgs e)
        {
            ShowGuide("Can't Connect To Your Own Server?", ConnectionGuideText);
        }

        /// <summary>
        /// Written from a real diagnosis. Evrima has no direct-connect and no console, so players
        /// can only use the server browser and must accept whatever address EOS advertises - which
        /// makes every network problem produce the same "Connection TIMED OUT" and sends admins
        /// chasing the firewall when the cause is usually upstream of their router entirely.
        /// </summary>
        private const string ConnectionGuideText =
@"CAN'T CONNECT TO YOUR OWN SERVER?

If you see ""UNetConnection::Tick: Connection TIMED OUT"" after about 20
seconds, the packets are not reaching your server. That is nearly always a
network problem, not a server problem. Work through these in order.


STEP 1 - IS THE SERVER ACTUALLY LISTENING?

In PowerShell:

    Get-NetUDPEndpoint | Where-Object LocalPort -eq 7777

You should see 0.0.0.0:7777. If nothing appears, the server did not start or
another program already holds the port - a second game server, for example.
Only one program can use port 7777 at a time.


STEP 2 - ARE YOU TESTING FROM THE SAME NETWORK AS THE SERVER?

If you connect to your own public IP from inside your own network, most
routers will not send that traffic back to you. This is called hairpin NAT
or NAT loopback, and it only affects you - real players are unaffected.

Other games let you work around it by typing a local address like
192.168.x.x. The Isle: Evrima has no direct-connect box and no console, so
you cannot. The server browser gives you the public address and that is the
only address you get.

Best test: have someone outside your network try to join. If they can, your
server is fine and only your own connection is affected.


STEP 3 - CHECK YOUR ROUTER'S WAN IP  (the one most people miss)

Open your router's admin page and find the WAN / Internet IP address.

If it starts with 100.64 through 100.127 - for example 100.82.129.139 -
you are behind CARRIER-GRADE NAT (CGNAT). That address is not public. It
belongs to your ISP, and their equipment drops incoming connections before
they ever reach your router.

If you are behind CGNAT:

  * NO amount of port forwarding will work. Not on any router.
  * Nobody can join your server, no matter how it is configured.
  * The ""public IP"" you see in game belongs to your ISP and is shared
    with other customers.

Fix: phone your ISP and ask to be taken off CGNAT, or for a public IPv4
address. It is often free or a small monthly fee. This is by far the
easiest solution, and your existing port forwarding will then just work.


WHAT DOES NOT WORK: INBOUND-ONLY TUNNELS

Services such as playit.gg give you a public address that forwards traffic
inward to your machine. That works for games where you hand players an
address directly. It does NOT work for The Isle.

The reason: your server announces itself to Epic Online Services, and EOS
records whichever IP address it sees your server connect FROM. An
inbound-only tunnel does not change outgoing traffic, so EOS still sees
your CGNAT address and still tells players to connect there. The tunnel
sits unused and players still cannot join. This has been tested.


WHAT DOES WORK: A VPS PLUS WIREGUARD

Rent a small server with a real public IP, run WireGuard on it, and route
your game machine's traffic - BOTH directions - through it. Now EOS sees
the VPS address and advertises that, and inbound traffic on port 7777 comes
back down the tunnel to you.

The difference from the tunnel services above is that outgoing traffic goes
through the VPS too. That is the part that matters.

Rough outline on the VPS (Ubuntu):

    sudo apt install wireguard-tools
    # wg0: Address 10.66.66.1/24, ListenPort 51820
    # then, where ens5 is the VPS network interface:
    iptables -t nat -A POSTROUTING -s 10.66.66.0/24 -o ens5 -j MASQUERADE
    iptables -t nat -A PREROUTING -i ens5 -p udp --dport 7777 \
             -j DNAT --to-destination 10.66.66.2:7777
    iptables -t nat -A POSTROUTING -d 10.66.66.2 -p udp --dport 7777 \
             -j MASQUERADE

Install WireGuard on the machine running the server, set AllowedIPs to
0.0.0.0/0, and open UDP 51820 and UDP 7777 in the VPS firewall. Confirm it
worked by visiting any ""what is my IP"" site - it must show the VPS address.

The last MASQUERADE rule is what lets you connect to your own server, so it
solves the hairpin problem in Step 2 at the same time.

Note this sends all of that machine's traffic through the VPS while the
tunnel is active, which uses bandwidth. Turn it off when you are done.


WHICH PORTS DO I ACTUALLY NEED?

  UDP 7777   The game. This is the only one that matters.
  UDP 10000  Login queue - ONLY if you set bQueueEnabled=true.
             With the queue off, nothing listens on it.
  TCP 8888   RCON. Do NOT expose this to the internet. Use it over a
             private tunnel, or not at all.
  UDP 27015  Steam query. Evrima lists through EOS, so this is not needed.


STILL STUCK?

Collect these before asking for help - they identify the problem quickly:

  1. Your router's WAN IP  (tells us instantly if it is CGNAT)
  2. Output of:  Get-NetUDPEndpoint | Where-Object LocalPort -eq 7777
  3. Whether anyone OUTSIDE your network can connect
  4. The full error text from the client
";

        internal async void btnCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item) item.IsEnabled = false;

            try
            {
                await CheckForUpdatesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update check failed:\n\n{ex.Message}", "Update Check", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sender is MenuItem itemRestore) itemRestore.IsEnabled = true;
            }
        }

        internal void mnuCheckUpdatesOnStartup_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoadingConfig) return;
            SaveSettings(true);
        }

        private async Task<(Version? latestVersion, string latestTag, string latestUrl)> FetchLatestReleaseInfoAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, GitHubLatestReleaseApi);
            request.Headers.UserAgent.ParseAdd("IsleServerLauncher/1.0");
            request.Headers.Accept.ParseAdd("application/vnd.github+json");

            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var response = await _httpClient.SendAsync(request, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Update check failed: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            string latestTag = doc.RootElement.TryGetProperty("tag_name", out var tagEl) ? (tagEl.GetString() ?? "") : "";
            string latestUrl = doc.RootElement.TryGetProperty("html_url", out var urlEl) ? (urlEl.GetString() ?? GitHubReleasesPage) : GitHubReleasesPage;

            return (ParseVersion(latestTag), latestTag, latestUrl);
        }

        /// <summary>
        /// Silent startup check: only surfaces anything when a new version exists,
        /// and only once per released version. Failures just go to the log.
        /// </summary>
        private async Task CheckForUpdatesOnStartupAsync()
        {
            try
            {
                // Let the window finish loading before doing network work
                await Task.Delay(TimeSpan.FromSeconds(5));

                if (!mnuCheckUpdatesOnStartup.IsChecked) return;

                var (latestVersion, _, latestUrl) = await FetchLatestReleaseInfoAsync();
                var currentVersion = GetCurrentVersion();
                if (currentVersion == null || latestVersion == null || latestVersion <= currentVersion)
                {
                    _logger.Info($"Startup update check: up to date (current v{currentVersion}, latest v{latestVersion}).");
                    return;
                }

                // Only notify once per released version
                string noticeFile = Path.Combine(_serverFolder, "last_update_notice.txt");
                try
                {
                    if (File.Exists(noticeFile) &&
                        string.Equals(File.ReadAllText(noticeFile).Trim(), latestVersion.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                    File.WriteAllText(noticeFile, latestVersion.ToString());
                }
                catch { /* notice tracking is best-effort; worst case the dialog shows again */ }

                _logger.Info($"Startup update check: new version v{latestVersion} is available.");

                var result = MessageBox.Show(
                    $"A new version of the launcher is available.\n\nCurrent: v{currentVersion}\nLatest: v{latestVersion}\n\nOpen the download page?\n\n(You won't be asked again for this version. You can always use Help > Check for Updates.)",
                    "Update Available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo { FileName = latestUrl, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                // Silent by design - never bother the user at startup over a failed check
                _logger.Info($"Startup update check skipped: {ex.Message}");
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            var (latestVersion, latestTag, latestUrl) = await FetchLatestReleaseInfoAsync();
            var currentVersion = GetCurrentVersion();

            if (currentVersion != null && latestVersion != null)
            {
                if (latestVersion > currentVersion)
                {
                    var result = MessageBox.Show(
                        $"A new version is available.\n\nCurrent: v{currentVersion}\nLatest: v{latestVersion}\n\nOpen download page?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo { FileName = latestUrl, UseShellExecute = true });
                    }
                }
                else
                {
                    MessageBox.Show($"You're up to date.\n\nCurrent: v{currentVersion}", "Update Check",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return;
            }

            var fallbackResult = MessageBox.Show(
                $"Latest release: {latestTag}\n\nOpen download page?",
                "Update Check",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);
            if (fallbackResult == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo { FileName = latestUrl, UseShellExecute = true });
            }
        }

        private Version? GetCurrentVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null) return version;

            return ParseVersion(Title);
        }

        private static Version? ParseVersion(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string cleaned = value.Trim();
            if (cleaned.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned[1..];
            }

            if (Version.TryParse(cleaned, out var v))
            {
                return v;
            }

            var match = System.Text.RegularExpressions.Regex.Match(cleaned, @"\d+(\.\d+)+");
            return match.Success && Version.TryParse(match.Value, out v) ? v : null;
        }

        internal void btnOpenAiAdminUi_Click(object sender, RoutedEventArgs e)
        {
            string toolPath = Path.Combine(_serverFolder, "tools", "AiAdminUi", "AiAdminUi.exe");
            if (!File.Exists(toolPath))
            {
                MessageBox.Show(
                    "AI Admin UI not found.\nExpected path:\n" + toolPath,
                    "AI Admin UI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = toolPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open AI Admin UI:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new Window
            {
                Title = "About",
                Width = 350,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "The Isle Evrima Server Launcher",
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = $"Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "?"}",
                Margin = new Thickness(0, 0, 0, 10)
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Created by: Sibercat.",
                Margin = new Thickness(0, 0, 0, 10)
            });

            var linkTextBlock = new TextBlock { Margin = new Thickness(0, 0, 0, 10) };
            linkTextBlock.Inlines.Add(new Run("GitHub: "));
            var hyperlink = new Hyperlink(new Run("https://github.com/sibercat/The-Isle-Evrima-Server-Launcher"))
            {
                NavigateUri = new Uri("https://github.com/sibercat/The-Isle-Evrima-Server-Launcher")
            };
            hyperlink.RequestNavigate += (s, args) =>
            {
                Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri) { UseShellExecute = true });
                args.Handled = true;
            };
            linkTextBlock.Inlines.Add(hyperlink);
            stackPanel.Children.Add(linkTextBlock);

            var okButton = new Button
            {
                Content = "OK",
                Width = 75,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };
            okButton.Click += (s, args) => aboutWindow.Close();
            stackPanel.Children.Add(okButton);

            aboutWindow.Content = stackPanel;
            aboutWindow.ShowDialog();
        }

        private async void btnTestAnnounce_Click(object sender, RoutedEventArgs e)
        {
            if (_serverManager.CurrentState != ServerState.Running) { MessageBox.Show("Server must be running.", "Error"); return; }

            try
            {
                var config = GetCurrentConfiguration();
                using (var tempRcon = new RconClient("127.0.0.1", int.Parse(config.RconPort), config.RconPassword, _logger))
                {
                    string message = string.IsNullOrWhiteSpace(txtRestartMessage.Text) ? "Test Announcement" : txtRestartMessage.Text.Replace("{minutes}", "TEST");
                    await tempRcon.SendAnnounceAsync(message);
                    ShowToast("✓ Announcement Sent");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error"); }
        }

        private async void btnTestWebhook_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiscordWebhookUrl.Text)) return;
            btnTestWebhook.IsEnabled = false;
            try
            {
                await _discordWebhookService.SendTestNotificationAsync(txtDiscordWebhookUrl.Text, txtServerName.Text);
                ShowToast("✓ Test Notification Sent");
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error"); }
            finally { btnTestWebhook.IsEnabled = true; }
        }

        // ==========================================
    }
}