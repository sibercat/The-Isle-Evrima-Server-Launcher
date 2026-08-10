using System;
using System.Collections.Generic;
using System.Linq;

namespace IsleServerLauncher.Services
{
    public static class InputValidator
    {
        // Port validation
        public static bool IsValidPort(string portText, out int port)
        {
            port = 0;
            if (string.IsNullOrWhiteSpace(portText))
                return false;

            if (!int.TryParse(portText, out port))
                return false;

            return port >= 1024 && port <= 65535;
        }

        // IP address validation
        public static bool IsValidIPv4(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            string[] parts = ip.Split('.');
            if (parts.Length != 4)
                return false;

            return parts.All(part => byte.TryParse(part, out _));
        }

        // Server name validation
        public static bool IsValidServerName(string name, out string? error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(name))
            {
                error = "Server name cannot be empty";
                return false;
            }

            if (name.Length > 100)
            {
                error = "Server name must be 100 characters or less";
                return false;
            }

            // Check for invalid characters that could break config files
            char[] invalidChars = new[] { '\n', '\r', '\0', '=', '[', ']' };
            if (name.Any(c => invalidChars.Contains(c)))
            {
                error = "Server name contains invalid characters";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Character set for a playable species class name as it appears in Game.ini
        /// (AllowedClasses=Tyrannosaurus). Shared with the Game.ini parser so a name the
        /// UI accepts is always a name that survives a reload.
        /// </summary>
        public const string PlayableClassNamePattern = "[A-Za-z0-9_]+";

        public const int MaxPlayableClassNameLength = 64;

        public static bool IsValidPlayableClassName(string name, out string? error)
        {
            error = null;
            name = (name ?? "").Trim();

            if (name.Length == 0)
            {
                error = "Species name cannot be empty";
                return false;
            }

            if (name.Length > MaxPlayableClassNameLength)
            {
                error = $"Species name must be {MaxPlayableClassNameLength} characters or less";
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(name, $"^{PlayableClassNamePattern}$"))
            {
                error = "Species names can only contain letters, numbers and underscores";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses a number the user typed, accepting either decimal separator. Someone on a
        /// German locale types "1,5" and someone on an English one types "1.5"; both mean the
        /// same value, and the server only ever accepts the invariant form.
        /// </summary>
        public static bool TryParseUserNumber(string? text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = text.Trim();
            const System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Float;

            return double.TryParse(text, styles, System.Globalization.CultureInfo.InvariantCulture, out value)
                || double.TryParse(text, styles, System.Globalization.CultureInfo.CurrentCulture, out value);
        }

        /// <summary>
        /// Rewrites a user-typed number into the invariant form the game's ini parser expects.
        /// Only the decimal separator is swapped - the text is never reformatted through a
        /// numeric type, which would turn 0.00001 into "1E-05" and large values into "1E+18",
        /// neither of which the game's ini parser understands. Non-numbers pass through.
        /// </summary>
        public static string NormalizeNumberForConfig(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text ?? "";

            string trimmed = text.Trim();
            const System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Float;

            // Already in the form the game wants - leave exactly as typed
            if (double.TryParse(trimmed, styles, System.Globalization.CultureInfo.InvariantCulture, out _))
                return trimmed;

            // Otherwise, if this locale reads it as a number, swap its decimal separator
            if (double.TryParse(trimmed, styles, System.Globalization.CultureInfo.CurrentCulture, out _))
            {
                string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if (sep != ".") return trimmed.Replace(sep, ".");
            }

            return trimmed;
        }

        // Numeric range validation
        public static bool IsValidNumber(string text, int min, int max, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (!int.TryParse(text, out value))
                return false;

            return value >= min && value <= max;
        }

        // Decimal validation (for multipliers)
        public static bool IsValidDecimal(string text, decimal min, decimal max, out decimal value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (!decimal.TryParse(text, out value))
                return false;

            return value >= min && value <= max;
        }

        // Steam ID validation (17-digit number)
        public static bool IsValidSteamId(string steamId)
        {
            if (string.IsNullOrWhiteSpace(steamId))
                return false;

            steamId = steamId.Trim();

            if (steamId.Length != 17)
                return false;

            return steamId.All(char.IsDigit);
        }

        // Path validation (prevents directory traversal)
        public static bool IsValidFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            // Check for path separators or invalid characters
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return !filename.Any(c => invalidChars.Contains(c) || c == '/' || c == '\\');
        }

        // Validate CPU core selection
        public static bool IsValidCpuAffinity(string cpuAffinity, int totalCores, out string? error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(cpuAffinity))
                return true; // Empty is valid (use all cores)

            var parts = cpuAffinity.Split(',');
            var cores = new HashSet<int>();

            foreach (var part in parts)
            {
                if (!int.TryParse(part.Trim(), out int core))
                {
                    error = $"Invalid core number: {part}";
                    return false;
                }

                if (core < 0 || core >= totalCores)
                {
                    error = $"Core {core} is out of range (0-{totalCores - 1})";
                    return false;
                }

                if (!cores.Add(core))
                {
                    error = $"Core {core} is listed multiple times";
                    return false;
                }
            }

            if (cores.Count == 0)
            {
                error = "At least one CPU core must be selected";
                return false;
            }

            return true;
        }
    }

    public class ConfigValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }

        public string GetErrorMessage()
        {
            return string.Join("\n", Errors);
        }
    }
}
