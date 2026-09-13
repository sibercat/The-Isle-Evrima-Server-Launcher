using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IsleServerLauncher.Services
{
    /// <summary>
    /// What one TLS handshake showed about whether this machine trusts an endpoint's certificate.
    /// </summary>
    public class TlsTrustResult
    {
        public string Host { get; init; } = "";
        public string? ConnectError { get; init; }
        public SslPolicyErrors PolicyErrors { get; init; }
        public string LeafIssuer { get; init; } = "";
        public string RootSubject { get; init; } = "";
        public string RootThumbprint { get; init; } = "";
        public bool RootInMachineStore { get; init; }
        public bool RootIsSelfSigned { get; init; }
        public IReadOnlyList<string> ChainStatus { get; init; } = Array.Empty<string>();

        public bool Connected => ConnectError == null;

        // Both halves are needed. No policy errors alone could mean the root is trusted only for the
        // current Windows user, which a server running as another account wouldn't see. The root
        // thumbprint is read from the chain each time rather than hardcoded, because the chain has
        // changed before (Amazon, now GlobalSign) and GlobalSign Root CA itself expires in 2028.
        public bool Trusted => Connected && PolicyErrors == SslPolicyErrors.None && RootInMachineStore;
    }

    public class RootInstallResult
    {
        public bool Succeeded { get; init; }
        public int RootsBefore { get; init; }
        public int RootsAfter { get; init; }
        public string? Error { get; init; }
    }

    public class SystemSetupService
    {
        private readonly string _serverFolder;
        private readonly ILogger _logger;

        public SystemSetupService(string serverFolder, ILogger logger)
        {
            _serverFolder = serverFolder ?? throw new ArgumentNullException(nameof(serverFolder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.Info($"SystemSetupService initialized. Server folder: {_serverFolder}");
        }

        /// <summary>
        /// Checks if current process has administrator privileges
        /// </summary>
        public bool IsAdministrator()
        {
            try
            {
                bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);

                _logger.Debug($"Administrator check: {isAdmin}");
                return isAdmin;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error checking administrator status: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Runs server setup: firewall, VC++ runtime, IE ESC disable
        /// </summary>
        public async Task RunServerSetupAsync()
        {
            if (!IsAdministrator())
            {
                _logger.Error("Server setup requires administrator privileges");
                throw new UnauthorizedAccessException("Administrator privileges required");
            }

            _logger.Info("Starting server setup");

            string scriptPath = Path.Combine(_serverFolder, "setup.ps1");
            string scriptContent = @"
Write-Host '>>> STARTING SERVER SETUP...' -ForegroundColor Cyan

Write-Host '1. Configuring Windows Firewall...' -ForegroundColor Yellow
if (-not (Get-NetFirewallRule -DisplayName 'The Isle - Game Port' -ErrorAction SilentlyContinue)) {
    New-NetFirewallRule -DisplayName 'The Isle - Game Port' -Direction Inbound -LocalPort 7777 -Protocol UDP -Action Allow
}
if (-not (Get-NetFirewallRule -DisplayName 'The Isle - RCON Port' -ErrorAction SilentlyContinue)) {
    New-NetFirewallRule -DisplayName 'The Isle - RCON Port' -Direction Inbound -LocalPort 8888 -Protocol TCP -Action Allow
}

Write-Host '2. Installing Visual C++ Runtime...' -ForegroundColor Yellow
$vcUrl = 'https://aka.ms/vs/17/release/vc_redist.x64.exe'
$vcFile = ""$env:TEMP\vc_redist.x64.exe""
Invoke-WebRequest -Uri $vcUrl -OutFile $vcFile -UseBasicParsing
Start-Process -FilePath $vcFile -ArgumentList '/install', '/passive', '/norestart' -Wait
Remove-Item $vcFile -ErrorAction SilentlyContinue

Write-Host '3. Tuning Windows Settings...' -ForegroundColor Yellow
function Disable-IEESC {
    $AdminKey = 'HKLM:\SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}'
    $UserKey = 'HKLM:\SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A8-37EF-4b3f-8CFC-4F3A74704073}'
    Set-ItemProperty -Path $AdminKey -Name 'IsInstalled' -Value 0 -ErrorAction SilentlyContinue
    Set-ItemProperty -Path $UserKey -Name 'IsInstalled' -Value 0 -ErrorAction SilentlyContinue
}
Disable-IEESC

Write-Host '>>> SETUP COMPLETE!' -ForegroundColor Cyan
Start-Sleep -Seconds 3";

            try
            {
                await RunPowerShellScriptAsync(scriptPath, scriptContent);
                _logger.Info("Server setup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Server setup failed: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Applies network optimization for VPS/Cloud servers
        /// </summary>
        public async Task RunNetworkOptimizationAsync()
        {
            if (!IsAdministrator())
            {
                _logger.Error("Network optimization requires administrator privileges");
                throw new UnauthorizedAccessException("Administrator privileges required");
            }

            _logger.Info("Starting network optimization");

            string scriptPath = Path.Combine(_serverFolder, "network_fix.ps1");
            string scriptContent = @"
Write-Host '>>> APPLYING UE5 NETWORK OPTIMIZATION...' -ForegroundColor Cyan

Write-Host 'Optimizing Network Adapters...' -ForegroundColor Yellow
# Fixes 'Stuck Connecting' and silent packet drops on VirtIO/VPS adapters
Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | ForEach-Object {
    Write-Host ""Processing: $($_.Name)"" -ForegroundColor Cyan
    
    # Disable UDP Checksum Offload (critical for UE5 packet delivery)
    Disable-NetAdapterChecksumOffload -Name $_.Name -UdpIPv4 -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host ""  ✓ Disabled UDP Checksum Offload"" -ForegroundColor Green
    
    # Disable RSC (fixes connection freeze on Windows Server)
    try {
        Disable-NetAdapterRsc -Name $_.Name -IPv4 -Confirm:$false -ErrorAction SilentlyContinue
        Write-Host ""  ✓ Disabled RSC"" -ForegroundColor Green
    } catch {
        Write-Host ""  - RSC not supported/already disabled"" -ForegroundColor Yellow
    }
    
    # Disable LSO (reduces fragmentation)
    Disable-NetAdapterLso -Name $_.Name -IPv4 -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host ""  ✓ Disabled LSO"" -ForegroundColor Green
    
    Write-Host """" -ForegroundColor White
}

Write-Host '>>> OPTIMIZATION COMPLETE!' -ForegroundColor Cyan
Write-Host 'CRITICAL: You must RESTART your VPS for changes to take full effect.' -ForegroundColor Yellow
Start-Sleep -Seconds 5";

            try
            {
                await RunPowerShellScriptAsync(scriptPath, scriptContent);
                _logger.Info("Network optimization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Network optimization failed: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// The HTTPS endpoints the server must trust to create its EOS session: Epic's API and the
        /// game's own backend. A machine that rejects either logs "libcurl error 60" followed by
        /// "Failed to create session".
        /// </summary>
        public static readonly IReadOnlyList<string> EpicTlsEndpoints = new[]
        {
            "api.epicgames.dev",
            "api.warphosting.com.au"
        };

        /// <summary>
        /// Handshakes with each Epic endpoint through Windows' own TLS stack and reports whether this
        /// machine trusts the certificate it receives.
        /// </summary>
        /// <remarks>
        /// The handshake is the fix as well as the check. Windows ships a minimal root store and
        /// downloads a missing trusted root only when its own crypto stack builds a chain. The game's
        /// libcurl reads the Windows store but never triggers that download, so on a fresh VPS the
        /// root Epic's chain needs can stay missing forever. Confirmed on a clean Windows Sandbox: a
        /// .NET SslStream handshake here made Windows add GlobalSign Root CA, and the chain went from
        /// untrusted to trusted with no other change.
        /// </remarks>
        public async Task<List<TlsTrustResult>> CheckEpicCertificatesAsync()
        {
            if (!IsAdministrator())
            {
                _logger.Error("Certificate check requires administrator privileges");
                throw new UnauthorizedAccessException("Administrator privileges required");
            }

            var results = new List<TlsTrustResult>();
            foreach (string host in EpicTlsEndpoints)
            {
                var result = await ProbeTlsTrustAsync(host);
                results.Add(result);

                if (!result.Connected)
                    _logger.Warning($"Certificate check {host}: could not connect - {result.ConnectError}");
                else if (result.Trusted)
                    _logger.Info($"Certificate check {host}: trusted. Issuer: {result.LeafIssuer}. Root: {result.RootSubject} [{result.RootThumbprint}]");
                else
                    _logger.Warning($"Certificate check {host}: NOT trusted ({result.PolicyErrors}, root in machine store: {result.RootInMachineStore}). " +
                                    $"Issuer: {result.LeafIssuer}. Root: {result.RootSubject} [{result.RootThumbprint}]. " +
                                    $"Chain status: {string.Join("; ", result.ChainStatus)}");
            }

            return results;
        }

        private async Task<TlsTrustResult> ProbeTlsTrustAsync(string host)
        {
            var policyErrors = SslPolicyErrors.None;
            string leafIssuer = "";
            string rootSubject = "";
            string rootThumbprint = "";
            bool rootIsSelfSigned = false;
            var chainStatus = new List<string>();

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                using var tcp = new TcpClient();
                await tcp.ConnectAsync(host, 443, cts.Token);

                using var ssl = new SslStream(tcp.GetStream(), leaveInnerStreamOpen: false);
                var options = new SslClientAuthenticationOptions
                {
                    TargetHost = host,
                    // Accept whatever arrives so the result can say why it isn't trusted. Nothing is
                    // sent over the connection. The chain is only valid inside this callback, so
                    // copy out what the report needs here.
                    RemoteCertificateValidationCallback = (_, certificate, chain, errors) =>
                    {
                        policyErrors = errors;
                        leafIssuer = certificate?.Issuer ?? "";

                        if (chain != null)
                            chainStatus.AddRange(chain.ChainStatus.Select(s => $"{s.Status}: {s.StatusInformation.Trim()}"));

                        if (chain != null && chain.ChainElements.Count > 0)
                        {
                            // Top of the chain. It is only the real root when it is self-signed:
                            // on an incomplete chain the chain stops at an intermediate, and
                            // calling that "the root" would send an admin looking for the wrong
                            // certificate in their store.
                            var top = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                            rootSubject = top.Subject;
                            rootThumbprint = top.Thumbprint;
                            rootIsSelfSigned = string.Equals(top.Subject, top.Issuer, StringComparison.OrdinalIgnoreCase);
                        }

                        return true;
                    }
                };

                await ssl.AuthenticateAsClientAsync(options, cts.Token);
            }
            catch (OperationCanceledException)
            {
                return new TlsTrustResult { Host = host, ConnectError = "Timed out after 20 seconds" };
            }
            catch (Exception ex)
            {
                return new TlsTrustResult { Host = host, ConnectError = ex.Message };
            }

            // Read the store after the handshake: any root Windows fetched arrived during it.
            bool rootInStore = rootThumbprint.Length > 0 && GetMachineRootCertificates().ContainsKey(rootThumbprint);

            return new TlsTrustResult
            {
                Host = host,
                PolicyErrors = policyErrors,
                LeafIssuer = leafIssuer,
                RootSubject = rootSubject,
                RootThumbprint = rootThumbprint,
                RootInMachineStore = rootInStore,
                RootIsSelfSigned = rootIsSelfSigned,
                ChainStatus = chainStatus
            };
        }

        /// <summary>
        /// Thumbprint to subject for every certificate in LocalMachine\Root.
        /// </summary>
        /// <remarks>
        /// Opening "Root" includes the AuthRoot store that Windows' on-demand downloads land in
        /// (verified on a clean Windows Sandbox), so a root fetched by the handshake counts as
        /// trusted here without checking AuthRoot separately.
        /// </remarks>
        public Dictionary<string, string> GetMachineRootCertificates()
        {
            var roots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            foreach (var cert in store.Certificates)
            {
                roots[cert.Thumbprint] = cert.Subject;
                cert.Dispose();
            }

            return roots;
        }

        /// <summary>
        /// Installs every root certificate Microsoft trusts into LocalMachine\Root, for machines where
        /// the on-demand download didn't happen (blocked by policy, or the handshake alone wasn't
        /// enough).
        /// </summary>
        /// <remarks>
        /// Uses Microsoft's own documented route: certutil builds an SST from Windows Update, then
        /// imports it. Checked on Windows Sandbox with root auto-update disabled by policy: the chain
        /// was untrusted before and trusted after, with the store going from 16 roots to 568.
        /// </remarks>
        public async Task<RootInstallResult> InstallMicrosoftTrustedRootsAsync()
        {
            if (!IsAdministrator())
            {
                _logger.Error("Installing trusted roots requires administrator privileges");
                throw new UnauthorizedAccessException("Administrator privileges required");
            }

            string sstPath = Path.Combine(Path.GetTempPath(), $"IsleLauncher-roots-{Guid.NewGuid():N}.sst");
            int rootsBefore = GetMachineRootCertificates().Count;
            _logger.Info($"Installing Microsoft trusted roots from Windows Update. Machine root store has {rootsBefore} certificates.");

            try
            {
                var (generateExit, generateOutput) = await RunCertUtilAsync($"-generateSSTFromWU \"{sstPath}\"");
                if (generateExit != 0 || !File.Exists(sstPath))
                {
                    string error = $"Could not download the trusted root list from Windows Update: {LastLine(generateOutput)}";
                    _logger.Error($"{error} (certutil exit code {generateExit})");
                    return new RootInstallResult { RootsBefore = rootsBefore, RootsAfter = rootsBefore, Error = error };
                }

                var (importExit, importOutput) = await RunCertUtilAsync($"-addstore -f Root \"{sstPath}\"");
                int rootsAfter = GetMachineRootCertificates().Count;
                if (importExit != 0)
                {
                    string error = $"Could not import the trusted root list: {LastLine(importOutput)}";
                    _logger.Error($"{error} (certutil exit code {importExit})");
                    return new RootInstallResult { RootsBefore = rootsBefore, RootsAfter = rootsAfter, Error = error };
                }

                _logger.Info($"Microsoft trusted roots installed. Machine root store: {rootsBefore} -> {rootsAfter} certificates.");
                return new RootInstallResult { Succeeded = true, RootsBefore = rootsBefore, RootsAfter = rootsAfter };
            }
            catch (Exception ex)
            {
                // certutil.exe can be missing or blocked (AppLocker, WDAC). Report it as a failed
                // install so the caller still gets the certificate report it already computed.
                _logger.Error($"Installing Microsoft trusted roots failed: {ex.Message}", ex);
                return new RootInstallResult
                {
                    RootsBefore = rootsBefore,
                    RootsAfter = GetMachineRootCertificates().Count,
                    Error = $"Could not run certutil: {ex.Message}"
                };
            }
            finally
            {
                try { File.Delete(sstPath); } catch { }
            }
        }

        private async Task<(int ExitCode, string Output)> RunCertUtilAsync(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.SystemDirectory, "certutil.exe"),
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _logger.Info($"Running certutil {arguments}");

            using var proc = Process.Start(psi);
            if (proc == null) return (-1, "certutil could not be started");

            // Read both streams concurrently - reading them one after the other can deadlock if
            // certutil fills the other pipe's buffer
            var outputTask = proc.StandardOutput.ReadToEndAsync();
            var errorTask = proc.StandardError.ReadToEndAsync();

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            try
            {
                await proc.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                try { proc.Kill(entireProcessTree: true); } catch { }
                try { await Task.WhenAll(outputTask, errorTask); } catch { } // observe the stream reads
                return (-1, "certutil timed out after 3 minutes");
            }

            string output = (await outputTask) + Environment.NewLine + (await errorTask);
            return (proc.ExitCode, output);
        }

        private static string LastLine(string output)
        {
            return output
                .Split('\n')
                .Select(line => line.Trim())
                .LastOrDefault(line => line.Length > 0) ?? "no output";
        }

        /// <summary>
        /// Executes a PowerShell script as admin
        /// </summary>
        private async Task RunPowerShellScriptAsync(string scriptPath, string scriptContent)
        {
            try
            {
                if (!Directory.Exists(_serverFolder))
                {
                    Directory.CreateDirectory(_serverFolder);
                    _logger.Debug($"Created server folder: {_serverFolder}");
                }

                await File.WriteAllTextAsync(scriptPath, scriptContent);
                _logger.Debug($"PowerShell script written to: {scriptPath}");

                Process process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";

                _logger.Info($"Starting PowerShell script: {Path.GetFileName(scriptPath)}");
                process.Start();

                await process.WaitForExitAsync();
                _logger.Info($"PowerShell script completed with exit code: {process.ExitCode}");

                if (process.ExitCode != 0)
                {
                    _logger.Warning($"PowerShell script exited with non-zero code: {process.ExitCode}");
                }

                // Cleanup
                try
                {
                    File.Delete(scriptPath);
                    _logger.Debug($"Cleaned up script file: {scriptPath}");
                }
                catch (Exception ex)
                {
                    _logger.Warning($"Failed to delete script file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error running PowerShell script: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Plain-text result of the certificate check, written so an admin can act on it or paste it
        /// into a support request as-is.
        /// </summary>
        public static string BuildCertificateReport(
            List<TlsTrustResult> results,
            Dictionary<string, string> rootsBefore,
            Dictionary<string, string> rootsAfter,
            RootInstallResult? install,
            bool fallbackDeclined)
        {
            var sb = new StringBuilder();
            bool allTrusted = results.All(r => r.Trusted);
            bool anyRejected = results.Any(r => r.Connected && !r.Trusted);
            var added = rootsAfter.Where(root => !rootsBefore.ContainsKey(root.Key)).ToList();

            sb.AppendLine("EPIC CERTIFICATE CHECK");
            sb.AppendLine();
            // An endpoint that was never reached had no certificate to judge. Saying "not trusted"
            // there would send an admin after certificates when the problem is the connection.
            sb.AppendLine(allTrusted
                ? "RESULT: TRUSTED. This machine trusts every Epic endpoint."
                : anyRejected
                    ? "RESULT: NOT FIXED. At least one Epic endpoint is still not trusted."
                    : "RESULT: COULD NOT CHECK. An Epic endpoint could not be reached, so its certificate was never seen.");
            sb.AppendLine();
            sb.AppendLine();

            foreach (var r in results)
            {
                sb.AppendLine(r.Host);
                if (!r.Connected)
                {
                    sb.AppendLine($"  Status     : Could not connect - {r.ConnectError}");
                }
                else
                {
                    string status = r.Trusted ? "Trusted"
                        : r.PolicyErrors == SslPolicyErrors.None ? "Not trusted (its root is not in the machine's Trusted Root store)"
                        : $"Not trusted ({r.PolicyErrors})";
                    sb.AppendLine($"  Status     : {status}");
                    sb.AppendLine($"  Issued by  : {r.LeafIssuer}");
                    sb.AppendLine(r.RootIsSelfSigned
                        ? $"  Chain root : {r.RootSubject}"
                        : $"  Chain ends : {r.RootSubject}");
                    sb.AppendLine($"               thumbprint {r.RootThumbprint}");
                    if (!r.RootIsSelfSigned && r.RootThumbprint.Length > 0)
                        sb.AppendLine("               (incomplete chain: this is an intermediate, not a root)");
                    foreach (string problem in r.ChainStatus)
                        sb.AppendLine($"  Problem    : {problem}");
                }
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("WHAT WAS INSTALLED");
            sb.AppendLine();
            if (install != null && install.Succeeded)
                sb.AppendLine("  Microsoft's trusted root certificates from Windows Update.");
            if (added.Count == 0)
            {
                sb.AppendLine("  No new root certificates were added.");
            }
            else if (added.Count <= 10)
            {
                sb.AppendLine($"  {added.Count} root certificate(s) added to the Trusted Root store:");
                foreach (var root in added)
                    sb.AppendLine($"    {root.Value}");
            }
            else
            {
                sb.AppendLine($"  {added.Count} root certificates added (Trusted Root store: {rootsBefore.Count} -> {rootsAfter.Count}).");
            }
            if (install != null && !install.Succeeded)
                sb.AppendLine($"  Installing Microsoft's trusted roots FAILED: {install.Error}");

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("WHAT TO DO NEXT");
            sb.AppendLine();

            if (allTrusted)
            {
                if (added.Count > 0)
                {
                    sb.AppendLine("  Restart the server. It should now log \"Successfully created session\"");
                    sb.AppendLine("  instead of \"Failed to create session\".");
                }
                else
                {
                    sb.AppendLine("  Nothing needed fixing: this machine already trusted Epic's certificates,");
                    sb.AppendLine("  so certificates are not what stops the server creating its session.");
                }
                return sb.ToString();
            }

            foreach (var r in results.Where(r => !r.Connected))
            {
                sb.AppendLine($"  {r.Host} could not be reached.");
                sb.AppendLine("  Check this machine's internet access, DNS and firewall (outbound TCP 443).");
                sb.AppendLine("  No certificate change can help until the connection itself works.");
                sb.AppendLine();
            }

            var stillUntrusted = results.Where(r => r.Connected && !r.Trusted).ToList();
            if (stillUntrusted.Count == 0) return sb.ToString();

            // The clock comes first on purpose. A wrong system clock fails the certificate dates AND
            // the HTTPS download from Windows Update, so it shows up as an install failure too, and
            // telling the admin to unblock Windows Update would send them after the wrong thing.
            if (stillUntrusted.Any(r => r.ChainStatus.Any(s => s.StartsWith("NotTimeValid", StringComparison.Ordinal))))
            {
                sb.AppendLine("  The certificate is outside its valid dates as far as this machine can tell.");
                sb.AppendLine("  Check that the Windows date, time and time zone are correct, then run this");
                sb.AppendLine("  check again. A wrong clock also blocks the download of trusted roots.");
            }
            else if (fallbackDeclined)
            {
                sb.AppendLine("  Run this check again and choose Yes when asked to install Microsoft's");
                sb.AppendLine("  trusted root certificates.");
            }
            else if (install != null && !install.Succeeded)
            {
                sb.AppendLine("  Microsoft's trusted root list could not be installed (see above for the");
                sb.AppendLine("  reason). If it mentions a download or Windows Update, this machine may be");
                sb.AppendLine("  blocked from reaching ctldl.windowsupdate.com: allow it through the firewall");
                sb.AppendLine("  or proxy, or ask your host, then run this check again.");
            }
            else if (stillUntrusted.All(r => r.PolicyErrors == SslPolicyErrors.None))
            {
                sb.AppendLine("  The certificate is trusted for your Windows account only, not for the whole");
                sb.AppendLine("  machine. Its root is in your personal certificate store instead of the");
                sb.AppendLine("  machine's Trusted Root store, so a server running under another account");
                sb.AppendLine("  would still reject it.");
            }
            else if (install == null)
            {
                // Only reachable if the caller skipped the install without the user declining it.
                // Say what is known instead of claiming roots were installed when none were.
                sb.AppendLine("  No root certificates were installed. Run this check again and choose Yes");
                sb.AppendLine("  when asked to install Microsoft's trusted root certificates.");
            }
            else
            {
                sb.AppendLine("  Every root certificate Microsoft trusts is now installed, and the certificate");
                sb.AppendLine("  this machine receives still doesn't lead to one. That means it is not Epic's");
                sb.AppendLine("  real certificate.");
                sb.AppendLine();
                sb.AppendLine("  Epic's endpoints use certificates from public authorities (currently Google");
                sb.AppendLine("  Trust Services). Look at \"Issued by\" above. If it names anything else - your");
                sb.AppendLine("  hosting provider, a proxy, a firewall or an antivirus product with HTTPS");
                sb.AppendLine("  scanning - that is intercepting the connection. Contact your host, or turn");
                sb.AppendLine("  off HTTPS inspection for this machine.");
            }

            return sb.ToString();
        }
    }
}