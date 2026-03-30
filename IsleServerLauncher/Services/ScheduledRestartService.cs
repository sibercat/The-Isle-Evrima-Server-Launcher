using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IsleServerLauncher.Services
{
    public class ScheduledRestartService
    {
        private readonly ILogger _logger;
        private System.Threading.Timer? _restartTimer;
        private CancellationTokenSource? _cancellationTokenSource;

        private bool _isEnabled;
        private int _intervalHours;
        private int _warningMinutes;
        private bool _useFixedTimes;
        private List<TimeSpan> _fixedTimes = new List<TimeSpan>();

        // FIX: Initialize with a default value to satisfy CS8618
        private string _customMessage = "Server will restart in {minutes} minute(s)!";

        private DateTime _nextRestartTime;

        public event EventHandler<TimeSpan>? RestartScheduled;
        public event EventHandler? RestartTriggered;
        public event EventHandler<int>? WarningIssued;

        public bool IsEnabled => _isEnabled;
        public DateTime NextRestartTime => _nextRestartTime;

        public ScheduledRestartService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Configures and starts the scheduled restart timer
        /// </summary>
        public void Configure(bool enabled, int intervalHours, int warningMinutes, string customMessage = "Server will restart in {minutes} minute(s)!", bool useFixedTimes = false, string fixedTimes = "")
        {
            _isEnabled = enabled;
            _intervalHours = Math.Max(1, Math.Min(intervalHours, 24));
            _warningMinutes = Math.Max(1, Math.Min(warningMinutes, 60));
            _customMessage = string.IsNullOrWhiteSpace(customMessage) ? "Server will restart in {minutes} minute(s)!" : customMessage;
            _useFixedTimes = useFixedTimes;
            _fixedTimes = ParseFixedTimes(fixedTimes);

            Stop();

            if (_isEnabled && (_useFixedTimes ? _fixedTimes.Count > 0 : intervalHours > 0))
            {
                Start();
            }
            else if (_isEnabled && _useFixedTimes && _fixedTimes.Count == 0)
            {
                _logger.Warning("Scheduled restart enabled with fixed times, but no valid times were provided.");
            }
        }

        /// <summary>
        /// Starts the scheduled restart timer
        /// </summary>
        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            ScheduleNextRestart(DateTime.Now);

            if (_useFixedTimes)
            {
                _logger.Info($"Scheduled restart enabled: Fixed times ({string.Join(", ", _fixedTimes.Select(t => t.ToString(@"hh\:mm")))}) with {_warningMinutes} minute warning");
            }
            else
            {
                _logger.Info($"Scheduled restart enabled: Every {_intervalHours} hours with {_warningMinutes} minute warning");
            }
            _logger.Info($"Next restart scheduled for: {_nextRestartTime:yyyy-MM-dd HH:mm:ss}");

            RestartScheduled?.Invoke(this, _nextRestartTime - DateTime.Now);

            // Set up timer to check every minute
            _restartTimer = new System.Threading.Timer(CheckRestartTime, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Stops the scheduled restart timer
        /// </summary>
        public void Stop()
        {
            _restartTimer?.Dispose();
            _restartTimer = null;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            if (_isEnabled)
            {
                _logger.Info("Scheduled restart service stopped");
            }
        }

        /// <summary>
        /// Resets the restart timer (call after manual restart)
        /// </summary>
        public void ResetTimer()
        {
            if (_isEnabled && (_useFixedTimes ? _fixedTimes.Count > 0 : _intervalHours > 0))
            {
                ScheduleNextRestart(DateTime.Now);

                _logger.Info($"Restart timer reset. Next restart: {_nextRestartTime:yyyy-MM-dd HH:mm:ss}");
                RestartScheduled?.Invoke(this, _nextRestartTime - DateTime.Now);
            }
        }

        /// <summary>
        /// Gets time remaining until next restart
        /// </summary>
        public TimeSpan GetTimeUntilRestart()
        {
            if (!_isEnabled) return TimeSpan.Zero;

            TimeSpan remaining = _nextRestartTime - DateTime.Now;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        /// <summary>
        /// Timer callback to check if restart is due
        /// </summary>
        private void CheckRestartTime(object? state)
        {
            if (!_isEnabled) return;

            TimeSpan timeUntilRestart = _nextRestartTime - DateTime.Now;

            // Issue warnings
            if (timeUntilRestart.TotalMinutes <= _warningMinutes && timeUntilRestart.TotalMinutes > _warningMinutes - 1)
            {
                _logger.Info($"Restart warning issued: {_warningMinutes} minutes");
                WarningIssued?.Invoke(this, _warningMinutes);
            }
            else if (timeUntilRestart.TotalMinutes <= 5 && timeUntilRestart.TotalMinutes > 4)
            {
                _logger.Info("Restart warning issued: 5 minutes");
                WarningIssued?.Invoke(this, 5);
            }
            else if (timeUntilRestart.TotalMinutes <= 1 && timeUntilRestart.TotalMinutes > 0)
            {
                _logger.Info("Restart warning issued: 1 minute");
                WarningIssued?.Invoke(this, 1);
            }

            // Trigger restart
            if (timeUntilRestart <= TimeSpan.Zero)
            {
                _logger.Info("=== SCHEDULED RESTART TRIGGERED ===");
                RestartTriggered?.Invoke(this, EventArgs.Empty);

                // Schedule next restart
                ResetTimer();
            }
        }

        private void ScheduleNextRestart(DateTime now)
        {
            if (_useFixedTimes && _fixedTimes.Count > 0)
            {
                _nextRestartTime = GetNextFixedRestartTime(now, _fixedTimes);
            }
            else
            {
                _nextRestartTime = now.Add(TimeSpan.FromHours(_intervalHours));
            }
        }

        private static DateTime GetNextFixedRestartTime(DateTime now, List<TimeSpan> times)
        {
            var today = now.Date;
            foreach (var t in times.OrderBy(t => t))
            {
                var candidate = today.Add(t);
                if (candidate > now)
                {
                    return candidate;
                }
            }
            // Next day, first time
            return today.AddDays(1).Add(times.OrderBy(t => t).First());
        }

        private static List<TimeSpan> ParseFixedTimes(string raw)
        {
            var results = new List<TimeSpan>();
            if (string.IsNullOrWhiteSpace(raw)) return results;

            var parts = raw.Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (TimeSpan.TryParse(trimmed, out var ts))
                {
                    if (ts >= TimeSpan.Zero && ts < TimeSpan.FromDays(1))
                    {
                        results.Add(ts);
                    }
                }
            }
            return results.Distinct().OrderBy(t => t).ToList();
        }
    }
}