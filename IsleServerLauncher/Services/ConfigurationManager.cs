using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IsleServerLauncher.Services
{
    public class DinoOption
    {
        public string Name { get; set; } = "";
        public bool IsEnabled { get; set; } = true;

        // Keeps list rows readable to screen readers and UI automation
        public override string ToString() => Name;
    }

    public class AiOption
    {
        public string Name { get; set; } = "";
        public bool IsEnabled { get; set; } = false;
    }

    public class ServerConfiguration
    {
        // Identity
        public string ServerName { get; set; } = "My Amazing Server";
        public string MaxPlayers { get; set; } = "100";
        public string ServerPassword { get; set; } = "";

        // Security & RCON
        public string RconPassword { get; set; } = "ChangeMe123";
        public string RconPort { get; set; } = "8888";
        public bool RconEnabled { get; set; } = false;
        public bool Whitelist { get; set; } = false;

        // Network
        public string GamePort { get; set; } = "7777";
        public string QueuePort { get; set; } = "10000";
        public bool QueueEnabled { get; set; } = false;
        public string CustomArgs { get; set; } = "";

        // Time
        public string DayLength { get; set; } = "45";
        public string NightLength { get; set; } = "20";

        // Gameplay
        public bool GlobalChat { get; set; } = false;
        public bool Humans { get; set; } = false;
        public bool Mutations { get; set; } = false;
        public bool Migration { get; set; } = false;
        public bool FallDamage { get; set; } = false;
        public string GrowthMultiplier { get; set; } = "1";
        public string CorpseDecay { get; set; } = "1";
        public string MigrationTime { get; set; } = "5400";

        // AI & Environment
        public bool SpawnAI { get; set; } = false;
        public bool SpawnPlants { get; set; } = false;
        public bool DynamicWeather { get; set; } = false;
        public string AISpawnInterval { get; set; } = "40";
        public string AIDensity { get; set; } = "1";
        public List<AiOption> DisallowedAIClasses { get; set; } = new List<AiOption>();
        public string RegionSpawnCooldownTimeSeconds { get; set; } = "30";
        public bool UseRegionSpawnCooldown { get; set; } = false;
        public bool UseRegionSpawning { get; set; } = false;
        public string PlantSpawnMultiplier { get; set; } = "1";
        public bool AllowRecordingReplay { get; set; } = true;
        public bool EnableDiets { get; set; } = true;
        public bool EnablePatrolZones { get; set; } = true;
        public string MassMigrationTime { get; set; } = "43200";
        public string MassMigrationDisableTime { get; set; } = "7200";
        public bool EnableMassMigration { get; set; } = false;
        public string SpeciesMigrationTime { get; set; } = "10800";
        public string MinWeatherVariationInterval { get; set; } = "600";
        public string MaxWeatherVariationInterval { get; set; } = "900";
        public string QueueJoinTimeoutSeconds { get; set; } = "30";
        public string QueueHeartbeatIntervalSeconds { get; set; } = "8";
        public string QueueHeartbeatTimeoutSeconds { get; set; } = "5";
        public string QueueHeartbeatMaxMisses { get; set; } = "2";

        // Performance
        public bool ValidateFiles { get; set; } = false;
        public bool DisableStreaming { get; set; } = false;
        public string ProcessPriority { get; set; } = "Normal";
        public string CpuAffinity { get; set; } = "";

        // Crash Detection
        public bool EnableCrashDetection { get; set; } = true;
        public bool AutoRestart { get; set; } = false;
        public int MaxRestartAttempts { get; set; } = 3;

        // Scheduled Restarts
        public bool ScheduledRestartEnabled { get; set; } = false;
        public int RestartIntervalHours { get; set; } = 6;
        public int RestartWarningMinutes { get; set; } = 15;
        public string RestartMessage { get; set; } = "Server will restart in {minutes} minute(s)!";
        public bool UseFixedRestartTimes { get; set; } = false;
        public string FixedRestartTimes { get; set; } = "";
        public string RestartScriptPath { get; set; } = "";
        public int RestartScriptDelaySeconds { get; set; } = 0;
        public bool RestartScriptEnabled { get; set; } = false;

        // Discord Webhook
        public bool EnableDiscordWebhook { get; set; } = false;
        public string DiscordWebhookUrl { get; set; } = "";
        public string DiscordInvite { get; set; } = "";

        // Mods
        public string ModLoaderPath { get; set; } = "";
        public string ModDllPath { get; set; } = "";
        public string ModConfigDir { get; set; } = "";
        public bool UseModBatInjection { get; set; } = false;
        public bool AutoInjectAfterRestart { get; set; } = false;
        public int AutoInjectDelaySeconds { get; set; } = 5;

        // Auto Broadcast
        public bool AutoBroadcastEnabled { get; set; } = false;
        public string AutoBroadcastMessage { get; set; } = "";
        public int AutoBroadcastIntervalMinutes { get; set; } = 15;

        // Automatic Backups
        public bool AutoBackupEnabled { get; set; } = false;
        public int BackupIntervalHours { get; set; } = 6;

        // Maintenance
        public bool AutoWipeCorpsesEnabled { get; set; } = false;
        public int WipeCorpsesIntervalMinutes { get; set; } = 60;
        public int WipeCorpsesDelayMinutes { get; set; } = 0;
        public string WipeWarningMessage { get; set; } = "Warning: All Corpses will be wiped in {minutes} minute(s)!";
        public string WipeCompleteMessage { get; set; } = "All Corpses have been wiped.";
        public bool AutoRconSaveEnabled { get; set; } = false;
        public int RconSaveIntervalMinutes { get; set; } = 30;

        // Chat Monitor
        public bool EnableChatMonitor { get; set; } = false;
        public bool EnableChatWebhook { get; set; } = false;
        public string ChatWebhookUrl { get; set; } = "";
        public int ChatRefreshInterval { get; set; } = 2;

        // Zombie Check
        public bool EnableZombieCheck { get; set; } = false;
        public int ZombieTimeoutSeconds { get; set; } = 60;

        // Debug Logging
        public bool EnableLogRedpointEOSVerbose { get; set; } = false;
        public bool EnableLogOnlineVerbose { get; set; } = false;
        public bool EnableLogOnlineGameVerbose { get; set; } = false;
        public bool EnableLogNetVerbose { get; set; } = false;
        public bool EnableLogNetTrafficVerbose { get; set; } = false;
        public bool EnableLogReplicationGraphVerbose { get; set; } = false;
        public bool EnableLogTheIsleVerbose { get; set; } = false;
        public bool EnableLogTheIsleAdminVerbose { get; set; } = false;
        public bool EnableLogTheIsleAIVerbose { get; set; } = false;
        public bool EnableLogTheIsleAnimInstanceVerbose { get; set; } = false;
        public bool EnableLogTheIsleAudioVerbose { get; set; } = false;
        public bool EnableLogTheIsleAuthVerbose { get; set; } = false;
        public bool EnableLogTheIsleCharacterVerbose { get; set; } = false;
        public bool EnableLogTheIsleCharacterMovementVerbose { get; set; } = false;
        public bool EnableLogTheIsleDatabaseVerbose { get; set; } = false;
        public bool EnableLogTheIsleEnvironmentVerbose { get; set; } = false;
        public bool EnableLogTheIsleGameVerbose { get; set; } = false;
        public bool EnableLogTheIsleNetworkVerbose { get; set; } = false;
        public bool EnableLogTheIsleServerVerbose { get; set; } = false;
        public bool EnableLogTheIslePlayerControllerVerbose { get; set; } = false;
        public bool EnableLogTheIsleUIVerbose { get; set; } = false;
        public bool EnableLogTheIsleWorldVerbose { get; set; } = false;
        public bool EnableLogTheIsleJoinDataVerbose { get; set; } = false;
        public bool EnableLogTheIsleChatDataVerbose { get; set; } = false;
        public bool EnableLogTheIsleKillDataVerbose { get; set; } = false;
        public bool EnableLogTheIsleCommandDataVerbose { get; set; } = false;
        public bool EnableLogTheIsleAntiCheatVerbose { get; set; } = false;

        // Application Settings
        public string Theme { get; set; } = "Light";
        public bool CheckUpdatesOnStartup { get; set; } = true;

        // Lists
        public List<string> AdminSteamIds { get; set; } = new List<string>();
        public List<string> WhitelistIds { get; set; } = new List<string>();
        public List<string> VipIds { get; set; } = new List<string>();
        public List<DinoOption> Dinosaurs { get; set; } = new List<DinoOption>();
    }

    public class ConfigurationManager
    {
        // UE reads a Config property only from its owning class's section, so these names
        // decide which keys the game actually sees. Verified against the Dumper-7 SDK:
        // AdminsSteamIDs / WhitelistIDs / VIPs / AllowedClasses live on ATIGameStateBase,
        // while DisallowedAIClasses / AIDensity / bSpawnAI live on ATIGameSession.
        private const string SessionSection = "/Script/TheIsle.TIGameSession";
        private const string StateSection = "/Script/TheIsle.TIGameStateBase";

        // These decide what the UI is allowed to represent, and they are anchored on purpose: an
        // entry is shown only when the WHOLE value is something the game can act on. Matching a
        // prefix instead would surface "AllowedClasses=BP_Deer.BP_Deer_C" as "BP_Deer" and save
        // it back truncated, and would list "AdminsSteamIDs=<id> (Bob)" as a live admin when the
        // game reads that entire string as the ID and grants Bob nothing. Values that don't
        // match are carried through untouched instead - see SplitIniEntries.
        private static readonly Regex ClassNameEntry =
            new Regex("^" + InputValidator.PlayableClassNamePattern + "$", RegexOptions.Compiled);
        private static readonly Regex SteamIdEntry = new Regex(@"^\d+$", RegexOptions.Compiled);

        // AI class identifiers can't be pattern-checked - they are FNames from level data that
        // the SDK dump doesn't expose, and the built-in list holds labels like "Frogs/Toads".
        // So accept anything that wouldn't corrupt the file if written back out.
        private static readonly Regex AiClassEntry =
            new Regex(@"^[^=\[\]()""\r\n]+$", RegexOptions.Compiled);

        // Versions up to 1.0.11 shipped display labels here rather than the identifiers the game
        // matches on, so these entries sat in Game.ini doing nothing. Confirmed on a live server
        // that "Chicken" suppresses the spawn and "Chickens" does not. The admin's intent when
        // they ticked the old label is unambiguous, so carry it across to the name that works
        // instead of leaving a setting that silently fails.
        private static readonly Dictionary<string, string> LegacyAiNames =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Chickens"] = "Chicken",
                ["Turtles"] = "SeaTurtle",
                ["Frogs/Toads"] = "Bullfrog",
                ["Crabs"] = "Crab",
            };

        private static string MigrateAiName(string name) =>
            LegacyAiNames.TryGetValue(name, out var current) ? current : name;

        private readonly string _serverFolder;
        private readonly string _configPath;
        private readonly string _engineConfigPath;
        private readonly string _settingsPath;
        private readonly ILogger _logger;

        public ConfigurationManager(string serverFolder, ILogger logger)
        {
            _serverFolder = serverFolder ?? throw new ArgumentNullException(nameof(serverFolder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _configPath = Path.Combine(serverFolder, @"TheIsle\Saved\Config\WindowsServer\Game.ini");
            _engineConfigPath = Path.Combine(serverFolder, @"TheIsle\Saved\Config\WindowsServer\Engine.ini");
            _settingsPath = Path.Combine(serverFolder, "launcher_settings.ini");

            _logger.Info($"ConfigurationManager initialized. Config path: {_configPath}");
        }

        /// <summary>
        /// Loads configuration from all config files
        /// </summary>
        public ServerConfiguration LoadConfiguration(List<string> allDinos, List<string> allAi)
        {
            _logger.Info("Loading configuration");
            var config = new ServerConfiguration();

            // Initialize dinosaur list
            config.Dinosaurs = allDinos.Select(d => new DinoOption { Name = d, IsEnabled = false }).ToList();
            config.DisallowedAIClasses = allAi.Select(ai => new AiOption { Name = ai, IsEnabled = false }).ToList();

            // Load Game.ini
            if (!File.Exists(_configPath))
            {
                _logger.Warning($"Game.ini not found at {_configPath}, creating default");
                CreateDefaultGameConfig();
            }

            try
            {
                string content = File.ReadAllText(_configPath);
                _logger.Debug("Game.ini loaded successfully");

                // Identity
                config.ServerName = GetGameIniValue(content, "ServerName") ?? "My Amazing Server";
                config.MaxPlayers = GetGameIniValue(content, "MaxPlayerCount") ?? "100";
                config.ServerPassword = GetGameIniValue(content, "ServerPassword")?.Replace("\"", "") ?? "";

                // Security
                config.RconPassword = GetGameIniValue(content, "RconPassword")?.Replace("\"", "") ?? "ChangeMe123";
                config.RconPort = GetGameIniValue(content, "RconPort") ?? "8888";
                config.RconEnabled = GetGameIniBool(content, "bRconEnabled", defaultValue: false);
                config.Whitelist = GetGameIniBool(content, "bServerWhitelist");

                // Network
                config.QueuePort = GetGameIniValue(content, "QueuePort") ?? "10000";
                config.QueueEnabled = GetGameIniBool(content, "bQueueEnabled");

                // Time
                config.DayLength = GetGameIniValue(content, "ServerDayLengthMinutes") ?? "45";
                config.NightLength = GetGameIniValue(content, "ServerNightLengthMinutes") ?? "20";

                // Gameplay
                config.GlobalChat = GetGameIniBool(content, "bEnableGlobalChat") || GetGameIniBool(content, "bServerGlobalChat");
                config.Humans = GetGameIniBool(content, "bEnableHumans");
                config.Mutations = GetGameIniBool(content, "bEnableMutations");
                config.Migration = GetGameIniBool(content, "bEnableMigration");
                config.FallDamage = GetGameIniBool(content, "bServerFallDamage");
                config.GrowthMultiplier = GetGameIniValue(content, "GrowthMultiplier") ?? "1";
                config.CorpseDecay = GetGameIniValue(content, "CorpseDecayMultiplier") ?? "1";
                config.MigrationTime = GetGameIniValue(content, "MaxMigrationTime") ?? "5400";

                // AI & Environment
                config.SpawnAI = GetGameIniBool(content, "bSpawnAI");
                config.SpawnPlants = GetGameIniBool(content, "bSpawnPlants");
                config.DynamicWeather = GetGameIniBool(content, "bServerDynamicWeather");
                config.AISpawnInterval = GetGameIniValue(content, "AISpawnInterval") ?? "40";
                config.AIDensity = GetGameIniValue(content, "AIDensity") ?? "1";
                config.DiscordInvite = GetGameIniValue(content, "Discord") ?? "";
                config.RegionSpawnCooldownTimeSeconds = GetGameIniValue(content, "RegionSpawnCooldownTimeSeconds") ?? "30";
                config.UseRegionSpawnCooldown = GetGameIniBool(content, "bUseRegionSpawnCooldown");
                config.UseRegionSpawning = GetGameIniBool(content, "bUseRegionSpawning");
                config.PlantSpawnMultiplier = GetGameIniValue(content, "PlantSpawnMultiplier") ?? "1";
                config.AllowRecordingReplay = GetGameIniBool(content, "bAllowRecordingReplay", defaultValue: true);
                config.EnableDiets = GetGameIniBool(content, "bEnableDiets", defaultValue: true);
                config.EnablePatrolZones = GetGameIniBool(content, "bEnablePatrolZones", defaultValue: true);
                config.MassMigrationTime = GetGameIniValue(content, "MassMigrationTime") ?? "43200";
                config.MassMigrationDisableTime = GetGameIniValue(content, "MassMigrationDisableTime") ?? "7200";
                config.EnableMassMigration = GetGameIniBool(content, "bEnableMassMigration");
                config.SpeciesMigrationTime = GetGameIniValue(content, "SpeciesMigrationTime") ?? "10800";
                config.MinWeatherVariationInterval = GetGameIniValue(content, "MinWeatherVariationInterval") ?? "600";
                config.MaxWeatherVariationInterval = GetGameIniValue(content, "MaxWeatherVariationInterval") ?? "900";
                config.QueueJoinTimeoutSeconds = GetGameIniValue(content, "QueueJoinTimeoutSeconds") ?? "30";
                config.QueueHeartbeatIntervalSeconds = GetGameIniValue(content, "QueueHeartbeatIntervalSeconds") ?? "8";
                config.QueueHeartbeatTimeoutSeconds = GetGameIniValue(content, "QueueHeartbeatTimeoutSeconds") ?? "5";
                config.QueueHeartbeatMaxMisses = GetGameIniValue(content, "QueueHeartbeatMaxMisses") ?? "2";

                // DisallowedAIClasses is a TArray<FString>, so it is one line per entry.
                // Older launcher versions wrote a single comma-joined line, so split on
                // commas too and existing selections survive the upgrade.
                // Read only the sections the save path rewrites - the session section where it
                // belongs, plus the state section older versions wrongly wrote it to.
                var disallowed = SplitIniEntries(
                        ReadIniValuesInSections(content, "DisallowedAIClasses", SessionSection, StateSection),
                        AiClassEntry)
                    .Recognised
                    .Select(MigrateAiName)
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                foreach (var name in disallowed)
                {
                    var option = config.DisallowedAIClasses.FirstOrDefault(ai =>
                        ai.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                    if (option == null)
                    {
                        // Adopt AI classes outside the built-in list instead of dropping them.
                        // The save path rewrites this key wholesale, so anything not carried
                        // here would be deleted from Game.ini - the same trap that used to
                        // eat hand-added AllowedClasses entries.
                        option = new AiOption { Name = name };
                        config.DisallowedAIClasses.Add(option);
                        _logger.Info($"Discovered non-standard AI class '{name}' in Game.ini; preserving it.");
                    }

                    option.IsEnabled = true;
                }
                
                // AdminsSteamIDs, WhitelistIDs, VIPs and AllowedClasses are all Config
                // properties on ATIGameStateBase (verified in the Dumper-7 SDK), so the game
                // reads them from that section and nowhere else. Scoping the read to it keeps
                // the launcher honest - a copy in another section is dead weight the game
                // ignores, and listing it would claim someone is an admin when they are not.
                // It also keeps removal working: an entry shown in the UI is one the save,
                // which only ever rewrites this section, can actually delete again.
                config.AdminSteamIds = ReadIniIdList(content, "AdminsSteamIDs", StateSection);
                _logger.Debug($"Loaded {config.AdminSteamIds.Count} admin Steam IDs");

                config.WhitelistIds = ReadIniIdList(content, "WhitelistIDs", StateSection);
                _logger.Debug($"Loaded {config.WhitelistIds.Count} whitelist IDs");

                config.VipIds = ReadIniIdList(content, "VIPs", StateSection);
                _logger.Debug($"Loaded {config.VipIds.Count} VIP IDs");

                WarnAboutIgnoredSectionEntries(content, "AdminsSteamIDs");
                WarnAboutIgnoredSectionEntries(content, "WhitelistIDs");
                WarnAboutIgnoredSectionEntries(content, "VIPs");
                WarnAboutIgnoredSectionEntries(content, "AllowedClasses");

                // Load dinosaurs from launcher settings first (priority)
                bool dinosLoadedFromSettings = false;
                if (File.Exists(_settingsPath))
                {
                    string settings = File.ReadAllText(_settingsPath);
                    _logger.Debug("Launcher settings loaded");

                    config.ValidateFiles = GetBoolValue(settings, "ValidateFiles");
                    config.DisableStreaming = GetBoolValue(settings, "DisableStreaming");
                    config.ProcessPriority = GetConfigValue(settings, "ProcessPriority") ?? "Normal";
                    config.CpuAffinity = GetConfigValue(settings, "CpuAffinity") ?? "";
                    config.GamePort = GetConfigValue(settings, "GamePort") ?? "7777";

                    // Theme
                    config.Theme = GetConfigValue(settings, "Theme") ?? "Light";

                    // Updates
                    config.CheckUpdatesOnStartup = GetBoolValue(settings, "CheckUpdatesOnStartup", defaultValue: true);

                    // Crash Detection
                    config.EnableCrashDetection = GetBoolValue(settings, "EnableCrashDetection", defaultValue: true);
                    config.AutoRestart = GetBoolValue(settings, "AutoRestart");
                    config.MaxRestartAttempts = int.TryParse(GetConfigValue(settings, "MaxRestartAttempts") ?? "3", out int maxAttempts) ? maxAttempts : 3;

                    // Scheduled Restarts
                    config.ScheduledRestartEnabled = GetBoolValue(settings, "ScheduledRestartEnabled");
                    config.RestartIntervalHours = int.TryParse(GetConfigValue(settings, "RestartIntervalHours") ?? "6", out int intervalHours) ? intervalHours : 6;
                    config.RestartWarningMinutes = int.TryParse(GetConfigValue(settings, "RestartWarningMinutes") ?? "15", out int warningMinutes) ? warningMinutes : 15;
                    config.RestartMessage = GetConfigValue(settings, "RestartMessage") ?? "Server will restart in {minutes} minute(s)!";
                    config.UseFixedRestartTimes = GetBoolValue(settings, "UseFixedRestartTimes");
                    config.FixedRestartTimes = GetConfigValue(settings, "FixedRestartTimes") ?? "";
                    config.RestartScriptPath = GetConfigValue(settings, "RestartScriptPath") ?? "";
                    config.RestartScriptDelaySeconds = int.TryParse(GetConfigValue(settings, "RestartScriptDelaySeconds") ?? "0", out int scriptDelay) ? scriptDelay : 0;
                    config.RestartScriptEnabled = GetBoolValue(settings, "RestartScriptEnabled");

                    // Discord Webhook
                    config.EnableDiscordWebhook = GetBoolValue(settings, "EnableDiscordWebhook");
                    config.DiscordWebhookUrl = GetConfigValue(settings, "DiscordWebhookUrl") ?? "";

                    // Mods
                    config.ModLoaderPath = GetConfigValue(settings, "ModLoaderPath") ?? "";
                    config.ModDllPath = GetConfigValue(settings, "ModDllPath") ?? "";
                    config.ModConfigDir = GetConfigValue(settings, "ModConfigDir") ?? "";
                    config.UseModBatInjection = GetBoolValue(settings, "UseModBatInjection");
                    config.AutoInjectAfterRestart = GetBoolValue(settings, "AutoInjectAfterRestart");
                    config.AutoInjectDelaySeconds = int.TryParse(GetConfigValue(settings, "AutoInjectDelaySeconds") ?? "5", out int autoInjectDelay)
                        ? autoInjectDelay
                        : 5;

                    // Auto Broadcast
                    config.AutoBroadcastEnabled = GetBoolValue(settings, "AutoBroadcastEnabled");
                    config.AutoBroadcastMessage = GetConfigValue(settings, "AutoBroadcastMessage") ?? "";
                    config.AutoBroadcastIntervalMinutes = int.TryParse(GetConfigValue(settings, "AutoBroadcastIntervalMinutes") ?? "15", out int broadcastInterval)
                        ? broadcastInterval
                        : 15;

                    // Automatic Backups
                    config.AutoBackupEnabled = GetBoolValue(settings, "AutoBackupEnabled");
                    config.BackupIntervalHours = int.TryParse(GetConfigValue(settings, "BackupIntervalHours") ?? "6", out int backupInterval) ? backupInterval : 6;

                    // Maintenance
                    config.AutoWipeCorpsesEnabled = GetBoolValue(settings, "AutoWipeCorpsesEnabled");
                    config.WipeCorpsesIntervalMinutes = int.TryParse(GetConfigValue(settings, "WipeCorpsesIntervalMinutes") ?? "60", out int wipeInt) ? wipeInt : 60;
                    config.WipeCorpsesDelayMinutes = int.TryParse(GetConfigValue(settings, "WipeCorpsesDelayMinutes") ?? "0", out int wipeDelay) ? wipeDelay : 0;
                    config.WipeWarningMessage = GetConfigValue(settings, "WipeWarningMessage") ?? "Warning: All Corpses will be wiped in {minutes} minute(s)!";
                    config.WipeCompleteMessage = GetConfigValue(settings, "WipeCompleteMessage") ?? "All Corpses have been wiped.";
                    config.AutoRconSaveEnabled = GetBoolValue(settings, "AutoRconSaveEnabled");
                    config.RconSaveIntervalMinutes = int.TryParse(GetConfigValue(settings, "RconSaveIntervalMinutes") ?? "30", out int saveInt) ? saveInt : 30;

                    // Chat Monitor
                    config.EnableChatMonitor = GetBoolValue(settings, "EnableChatMonitor");
                    config.EnableChatWebhook = GetBoolValue(settings, "EnableChatWebhook");
                    config.ChatWebhookUrl = GetConfigValue(settings, "ChatWebhookUrl") ?? "";
                    config.ChatRefreshInterval = int.TryParse(GetConfigValue(settings, "ChatRefreshInterval") ?? "2", out int chatInterval) ? chatInterval : 2;

                    // Zombie Check
                    config.EnableZombieCheck = GetBoolValue(settings, "EnableZombieCheck");
                    config.ZombieTimeoutSeconds = int.TryParse(GetConfigValue(settings, "ZombieTimeoutSeconds") ?? "60", out int zombieTimeout) ? zombieTimeout : 60;

                    // Debug Logging
                    config.EnableLogRedpointEOSVerbose = GetBoolValue(settings, "EnableLogRedpointEOSVerbose");
                    config.EnableLogOnlineVerbose = GetBoolValue(settings, "EnableLogOnlineVerbose");
                    config.EnableLogOnlineGameVerbose = GetBoolValue(settings, "EnableLogOnlineGameVerbose");
                    config.EnableLogNetVerbose = GetBoolValue(settings, "EnableLogNetVerbose");
                    config.EnableLogNetTrafficVerbose = GetBoolValue(settings, "EnableLogNetTrafficVerbose");
                    config.EnableLogReplicationGraphVerbose = GetBoolValue(settings, "EnableLogReplicationGraphVerbose");
                    config.EnableLogTheIsleVerbose = GetBoolValue(settings, "EnableLogTheIsleVerbose");
                    config.EnableLogTheIsleAdminVerbose = GetBoolValue(settings, "EnableLogTheIsleAdminVerbose");
                    config.EnableLogTheIsleAIVerbose = GetBoolValue(settings, "EnableLogTheIsleAIVerbose");
                    config.EnableLogTheIsleAnimInstanceVerbose = GetBoolValue(settings, "EnableLogTheIsleAnimInstanceVerbose");
                    config.EnableLogTheIsleAudioVerbose = GetBoolValue(settings, "EnableLogTheIsleAudioVerbose");
                    config.EnableLogTheIsleAuthVerbose = GetBoolValue(settings, "EnableLogTheIsleAuthVerbose");
                    config.EnableLogTheIsleCharacterVerbose = GetBoolValue(settings, "EnableLogTheIsleCharacterVerbose");
                    config.EnableLogTheIsleCharacterMovementVerbose = GetBoolValue(settings, "EnableLogTheIsleCharacterMovementVerbose");
                    config.EnableLogTheIsleDatabaseVerbose = GetBoolValue(settings, "EnableLogTheIsleDatabaseVerbose");
                    config.EnableLogTheIsleEnvironmentVerbose = GetBoolValue(settings, "EnableLogTheIsleEnvironmentVerbose");
                    config.EnableLogTheIsleGameVerbose = GetBoolValue(settings, "EnableLogTheIsleGameVerbose");
                    config.EnableLogTheIsleNetworkVerbose = GetBoolValue(settings, "EnableLogTheIsleNetworkVerbose");
                    config.EnableLogTheIsleServerVerbose = GetBoolValue(settings, "EnableLogTheIsleServerVerbose");
                    config.EnableLogTheIslePlayerControllerVerbose = GetBoolValue(settings, "EnableLogTheIslePlayerControllerVerbose");
                    config.EnableLogTheIsleUIVerbose = GetBoolValue(settings, "EnableLogTheIsleUIVerbose");
                    config.EnableLogTheIsleWorldVerbose = GetBoolValue(settings, "EnableLogTheIsleWorldVerbose");
                    config.EnableLogTheIsleJoinDataVerbose = GetBoolValue(settings, "EnableLogTheIsleJoinDataVerbose");
                    config.EnableLogTheIsleChatDataVerbose = GetBoolValue(settings, "EnableLogTheIsleChatDataVerbose");
                    config.EnableLogTheIsleKillDataVerbose = GetBoolValue(settings, "EnableLogTheIsleKillDataVerbose");
                    config.EnableLogTheIsleCommandDataVerbose = GetBoolValue(settings, "EnableLogTheIsleCommandDataVerbose");
                    config.EnableLogTheIsleAntiCheatVerbose = GetBoolValue(settings, "EnableLogTheIsleAntiCheatVerbose");

                    string customArgs = GetConfigValue(settings, "CustomArgs") ?? "";
                    config.CustomArgs = customArgs.StartsWith("Example:") ? "" : customArgs;

                    // KnownDinos remembers discovered species even while they are unticked,
                    // so they don't vanish from the list the next time the launcher starts.
                    string? knownDinos = GetConfigValue(settings, "KnownDinos");
                    if (!string.IsNullOrWhiteSpace(knownDinos))
                    {
                        MergeDiscoveredDinos(config, knownDinos.Split(','), "launcher settings");
                    }

                    // Same for adopted AI classes, so unticking one doesn't make it vanish
                    string? knownAi = GetConfigValue(settings, "KnownAi");
                    if (!string.IsNullOrWhiteSpace(knownAi))
                    {
                        foreach (var raw in knownAi.Split(','))
                        {
                            string name = MigrateAiName(raw.Trim());
                            if (name.Length == 0) continue;
                            if (config.DisallowedAIClasses.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) continue;
                            config.DisallowedAIClasses.Add(new AiOption { Name = name });
                        }
                    }

                    string? enabledDinos = GetConfigValue(settings, "EnabledDinos");
                    if (enabledDinos != null)
                    {
                        MergeDiscoveredDinos(config, enabledDinos.Split(','), "launcher settings");
                        dinosLoadedFromSettings = true;
                        if (!string.IsNullOrWhiteSpace(enabledDinos))
                        {
                            var enabled = enabledDinos.Split(',');
                            foreach (var dino in config.Dinosaurs)
                            {
                                // Case-insensitive and trimmed: the rest of the dino handling compares
                                // with OrdinalIgnoreCase, and an ordinal match here would silently untick
                                // a species that was discovered under different casing (e.g. a hand-added
                                // "austroraptor") once it joins the built-in roster.
                                dino.IsEnabled = enabled.Any(e =>
                                    e.Trim().Equals(dino.Name, StringComparison.OrdinalIgnoreCase));
                            }
                            _logger.Debug($"Loaded {enabled.Length} enabled dinosaurs from settings");
                        }
                        else
                        {
                            foreach (var dino in config.Dinosaurs)
                            {
                                dino.IsEnabled = false;
                            }
                            _logger.Debug("No dinosaurs enabled (from settings)");
                        }
                    }
                }

                // Adopt any AllowedClasses entry the launcher doesn't ship in its built-in
                // roster (new Evrima species, hand-edited Game.ini, modded classes). Without
                // this they'd be dropped from the list and then erased from Game.ini on the
                // next save. Runs after the settings pass so a brand-new discovery isn't
                // immediately unticked by EnabledDinos: its presence in Game.ini is a
                // deliberate signal, so it starts enabled. KnownDinos remembers it from then
                // on, and the user's tick state is respected afterwards.
                MergeDiscoveredDinos(config, ExtractAllowedClasses(content), "Game.ini", enabledIfNew: true);

                // Load dinos from Game.ini only if not loaded from settings
                if (!dinosLoadedFromSettings)
                {
                    // Presence and parse result are separate questions. Keying off the parsed
                    // count alone would treat a present-but-unparsable list (UE's "+Key=" append
                    // form, quoted values) as "no data" and enable every species, wiping the
                    // admin's restriction. Substring-matching "AllowedClasses=" is no good
                    // either - it misses the spaced form.
                    // Presence is a raw line count; the enabled set goes through the one parser,
                    // so this path can never disagree with the discovery pass above about what
                    // a line means.
                    bool hasAllowedClasses =
                        ReadIniValuesInSections(content, "AllowedClasses", StateSection).Count > 0;
                    var allowed = ExtractAllowedClasses(content);

                    if (hasAllowedClasses)
                    {
                        foreach (var dino in config.Dinosaurs) dino.IsEnabled = false;
                        foreach (var name in allowed)
                        {
                            var item = config.Dinosaurs.FirstOrDefault(d =>
                                d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                            if (item != null) item.IsEnabled = true;
                        }
                        _logger.Debug($"Loaded {allowed.Count} enabled dinosaurs from Game.ini");
                    }
                    else
                    {
                        // No saved data - enable all by default
                        foreach (var dino in config.Dinosaurs)
                        {
                            dino.IsEnabled = true;
                        }
                        _logger.Debug("All dinosaurs enabled by default");
                    }
                }

                _logger.Info("Configuration loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading configuration: {ex.Message}", ex);
                // Return default configuration on error
                config = new ServerConfiguration();
                config.Dinosaurs = allDinos.Select(d => new DinoOption { Name = d, IsEnabled = true }).ToList();
                config.DisallowedAIClasses = allAi.Select(ai => new AiOption { Name = ai, IsEnabled = false }).ToList();
            }

            return config;
        }

        /// <summary>
        /// Saves configuration to all config files
        /// </summary>
        public void SaveConfiguration(ServerConfiguration config)
        {
            _logger.Info("Saving configuration");

            try
            {
                // Save Game.ini
                UpdateGameConfig(config);

                // Save Engine.ini
                UpdateEngineConfig(config);

                // Save launcher settings
                SaveLauncherSettings(config);

                _logger.Info("Configuration saved successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving configuration: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Updates Game.ini with safe editing
        /// </summary>
        private void UpdateGameConfig(ServerConfiguration config)
        {
            try
            {
                _logger.Debug("Updating Game.ini");

                string? dir = Path.GetDirectoryName(_configPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    _logger.Debug($"Created directory: {dir}");
                }

                List<string> lines = File.Exists(_configPath)
                    ? File.ReadAllLines(_configPath).ToList()
                    : new List<string>();

                string section = SessionSection;

                // Identity
                UpdateIniValue(lines, section, "ServerName", config.ServerName);
                UpdateIniValue(lines, section, "MaxPlayerCount", config.MaxPlayers);
                UpdateIniValue(lines, section, "MapName", "Gateway");

                // Security
                bool hasPass = !string.IsNullOrWhiteSpace(config.ServerPassword);
                UpdateIniValue(lines, section, "bServerPassword", hasPass.ToString().ToLower());
                UpdateIniValue(lines, section, "ServerPassword", hasPass ? config.ServerPassword : null);

                UpdateIniValue(lines, section, "bRconEnabled", config.RconEnabled.ToString().ToLower());
                UpdateIniValue(lines, section, "RconPassword", config.RconPassword);
                UpdateIniValue(lines, section, "RconPort", config.RconPort);
                UpdateIniValue(lines, section, "bServerWhitelist", config.Whitelist.ToString().ToLower());

                // Queue
                UpdateIniValue(lines, section, "bQueueEnabled", config.QueueEnabled.ToString().ToLower());
                UpdateIniValue(lines, section, "QueuePort", config.QueuePort);

                // Gameplay
                UpdateIniValue(lines, section, "bEnableHumans", config.Humans.ToString().ToLower());
                UpdateIniValue(lines, section, "bEnableMutations", config.Mutations.ToString().ToLower());
                UpdateIniValue(lines, section, "bEnableGlobalChat", config.GlobalChat.ToString().ToLower());

                // bServerGlobalChat is a legacy spelling the loader still ORs in, but no class in
                // the SDK dump declares it, so the game ignores it. Left in place it would win
                // that OR forever and Global Chat could never be turned off: the save wrote
                // bEnableGlobalChat=false and the next load read the stale true. Its value has
                // already been migrated into config.GlobalChat above, so drop it.
                UpdateIniValue(lines, section, "bServerGlobalChat", null);
                UpdateIniValue(lines, section, "bEnableMigration", config.Migration.ToString().ToLower());
                UpdateIniValue(lines, section, "bServerFallDamage", config.FallDamage.ToString().ToLower());
                UpdateIniValue(lines, section, "GrowthMultiplier", InputValidator.NormalizeNumberForConfig(config.GrowthMultiplier));
                UpdateIniValue(lines, section, "CorpseDecayMultiplier", InputValidator.NormalizeNumberForConfig(config.CorpseDecay));
                UpdateIniValue(lines, section, "ServerDayLengthMinutes", InputValidator.NormalizeNumberForConfig(config.DayLength));
                UpdateIniValue(lines, section, "ServerNightLengthMinutes", InputValidator.NormalizeNumberForConfig(config.NightLength));
                UpdateIniValue(lines, section, "MaxMigrationTime", InputValidator.NormalizeNumberForConfig(config.MigrationTime));

                // AI & Environment
                UpdateIniValue(lines, section, "bSpawnAI", config.SpawnAI.ToString().ToLower());
                UpdateIniValue(lines, section, "bSpawnPlants", config.SpawnPlants.ToString().ToLower());
                UpdateIniValue(lines, section, "bServerDynamicWeather", config.DynamicWeather.ToString().ToLower());
                UpdateIniValue(lines, section, "AISpawnInterval", InputValidator.NormalizeNumberForConfig(config.AISpawnInterval));
                UpdateIniValue(lines, section, "AIDensity", InputValidator.NormalizeNumberForConfig(config.AIDensity));
                UpdateIniValue(lines, section, "Discord", string.IsNullOrWhiteSpace(config.DiscordInvite) ? null : config.DiscordInvite);
                UpdateIniValue(lines, section, "RegionSpawnCooldownTimeSeconds", InputValidator.NormalizeNumberForConfig(config.RegionSpawnCooldownTimeSeconds));
                UpdateIniValue(lines, section, "bUseRegionSpawnCooldown", config.UseRegionSpawnCooldown.ToString().ToLower());
                UpdateIniValue(lines, section, "bUseRegionSpawning", config.UseRegionSpawning.ToString().ToLower());
                UpdateIniValue(lines, section, "PlantSpawnMultiplier", InputValidator.NormalizeNumberForConfig(config.PlantSpawnMultiplier));
                UpdateIniValue(lines, section, "bAllowRecordingReplay", config.AllowRecordingReplay.ToString().ToLower());
                UpdateIniValue(lines, section, "bEnableDiets", config.EnableDiets.ToString().ToLower());
                UpdateIniValue(lines, section, "bEnablePatrolZones", config.EnablePatrolZones.ToString().ToLower());
                UpdateIniValue(lines, section, "MassMigrationTime", InputValidator.NormalizeNumberForConfig(config.MassMigrationTime));
                UpdateIniValue(lines, section, "MassMigrationDisableTime", InputValidator.NormalizeNumberForConfig(config.MassMigrationDisableTime));
                UpdateIniValue(lines, section, "bEnableMassMigration", config.EnableMassMigration.ToString().ToLower());
                UpdateIniValue(lines, section, "SpeciesMigrationTime", InputValidator.NormalizeNumberForConfig(config.SpeciesMigrationTime));
                UpdateIniValue(lines, section, "MinWeatherVariationInterval", InputValidator.NormalizeNumberForConfig(config.MinWeatherVariationInterval));
                UpdateIniValue(lines, section, "MaxWeatherVariationInterval", InputValidator.NormalizeNumberForConfig(config.MaxWeatherVariationInterval));
                UpdateIniValue(lines, section, "QueueJoinTimeoutSeconds", InputValidator.NormalizeNumberForConfig(config.QueueJoinTimeoutSeconds));
                UpdateIniValue(lines, section, "QueueHeartbeatIntervalSeconds", InputValidator.NormalizeNumberForConfig(config.QueueHeartbeatIntervalSeconds));
                UpdateIniValue(lines, section, "QueueHeartbeatTimeoutSeconds", InputValidator.NormalizeNumberForConfig(config.QueueHeartbeatTimeoutSeconds));
                UpdateIniValue(lines, section, "QueueHeartbeatMaxMisses", config.QueueHeartbeatMaxMisses);

                // Lists
                string stateSection = StateSection;

                var adminList = config.AdminSteamIds
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => $"AdminsSteamIDs={id.Trim()}")
                    .ToList();

                var whitelistList = config.WhitelistIds
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => $"WhitelistIDs={id.Trim()}")
                    .ToList();

                var vipList = config.VipIds
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => $"VIPs={id.Trim()}")
                    .ToList();

                var dinoList = config.Dinosaurs
                    .Where(d => d.IsEnabled)
                    .Select(d => $"AllowedClasses={d.Name}")
                    .ToList();

                // Values the loader didn't recognise were never shown in the UI, so they aren't in
                // the lists above - carry them over from the file being rewritten. Reading them
                // back here rather than off the config object means no UI code path can drop
                // them. Every line for the key is then replaced, which is what keeps entries the
                // admin CAN see removable.
                string existing = string.Join("\n", lines);
                adminList.AddRange(PreservedEntries(existing, "AdminsSteamIDs", stateSection, SteamIdEntry));
                whitelistList.AddRange(PreservedEntries(existing, "WhitelistIDs", stateSection, SteamIdEntry));
                vipList.AddRange(PreservedEntries(existing, "VIPs", stateSection, SteamIdEntry));
                dinoList.AddRange(PreservedEntries(existing, "AllowedClasses", stateSection, ClassNameEntry));

                UpdateIniList(lines, stateSection, "AdminsSteamIDs", adminList);
                UpdateIniList(lines, stateSection, "WhitelistIDs", whitelistList);
                UpdateIniList(lines, stateSection, "VIPs", vipList);
                UpdateIniList(lines, stateSection, "AllowedClasses", dinoList);

                // DisallowedAIClasses is a Config TArray<FString> on TIGameSession - not
                // TIGameStateBase - so it belongs in the session section and must be written
                // one line per entry, exactly like AllowedClasses above. Earlier versions
                // wrote a single comma-joined line into the wrong section, which the game
                // would have read as one nonsense entry (if it read it at all).
                var disallowedAiList = config.DisallowedAIClasses
                    .Where(ai => ai.IsEnabled)
                    .Select(ai => $"DisallowedAIClasses={ai.Name}")
                    .ToList();
                disallowedAiList.AddRange(
                    PreservedEntries(existing, "DisallowedAIClasses", AiClassEntry, section, stateSection));

                UpdateIniList(lines, section, "DisallowedAIClasses", disallowedAiList);

                // Drop every misplaced legacy line so none can shadow the correct ones.
                // UpdateIniValue(null) would only remove the first match, and the loader
                // reads all DisallowedAIClasses lines, so leftovers would re-tick classes
                // the user just unticked.
                UpdateIniList(lines, stateSection, "DisallowedAIClasses", new List<string>());

                File.WriteAllLines(_configPath, lines);
                _logger.Debug("Game.ini updated successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating Game.ini: {ex.Message}", ex);
                throw new InvalidOperationException($"Error updating Game.ini: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates Engine.ini with safe editing
        /// </summary>
        private void UpdateEngineConfig(ServerConfiguration config)
        {
            try
            {
                _logger.Debug("Updating Engine.ini");

                string? dir = Path.GetDirectoryName(_engineConfigPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    _logger.Debug($"Created directory: {dir}");
                }

                List<string> lines = File.Exists(_engineConfigPath)
                    ? File.ReadAllLines(_engineConfigPath).ToList()
                    : new List<string>();

                // Debug logging
                UpdateIniValue(lines, "Core.Log", "LogRedpointEOS", config.EnableLogRedpointEOSVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogOnline", config.EnableLogOnlineVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogOnlineGame", config.EnableLogOnlineGameVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogNet", config.EnableLogNetVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogNetTraffic", config.EnableLogNetTrafficVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogReplicationGraph", config.EnableLogReplicationGraphVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsle", config.EnableLogTheIsleVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleAdmin", config.EnableLogTheIsleAdminVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleAI", config.EnableLogTheIsleAIVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleAnimInstance", config.EnableLogTheIsleAnimInstanceVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleAudio", config.EnableLogTheIsleAudioVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleAuth", config.EnableLogTheIsleAuthVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleCharacter", config.EnableLogTheIsleCharacterVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleCharacterMovement", config.EnableLogTheIsleCharacterMovementVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleDatabase", config.EnableLogTheIsleDatabaseVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleEnvironment", config.EnableLogTheIsleEnvironmentVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleGame", config.EnableLogTheIsleGameVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleNetwork", config.EnableLogTheIsleNetworkVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleServer", config.EnableLogTheIsleServerVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIslePlayerController", config.EnableLogTheIslePlayerControllerVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleUI", config.EnableLogTheIsleUIVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleWorld", config.EnableLogTheIsleWorldVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleJoinData", config.EnableLogTheIsleJoinDataVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleChatData", config.EnableLogTheIsleChatDataVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleKillData", config.EnableLogTheIsleKillDataVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleCommandData", config.EnableLogTheIsleCommandDataVerbose ? "Verbose" : null);
                UpdateIniValue(lines, "Core.Log", "LogTheIsleAntiCheat", config.EnableLogTheIsleAntiCheatVerbose ? "Verbose" : null);

                // Mandatory EOS keys
                UpdateIniValue(lines, "EpicOnlineServices", "DedicatedServerClientId", "xyza7891gk5PRo3J7G9puCJGFJjmEguW");
                UpdateIniValue(lines, "EpicOnlineServices", "DedicatedServerClientSecret", "pKWl6t5i9NJK8gTpVlAxzENZ65P8hYzodV8Dqe5Rlc8");

                // Streaming options
                if (config.DisableStreaming)
                {
                    UpdateIniValue(lines, "ConsoleVariables", "wp.Runtime.EnableServerStreaming", "0");
                    UpdateIniValue(lines, "ConsoleVariables", "wp.Runtime.EnableServerStreamingOut", "0");
                    _logger.Debug("World streaming disabled");
                }
                else
                {
                    UpdateIniValue(lines, "ConsoleVariables", "wp.Runtime.EnableServerStreaming", null);
                    UpdateIniValue(lines, "ConsoleVariables", "wp.Runtime.EnableServerStreamingOut", null);
                }

                File.WriteAllLines(_engineConfigPath, lines);
                _logger.Debug("Engine.ini updated successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating Engine.ini: {ex.Message}", ex);
                throw new InvalidOperationException($"Error updating Engine.ini: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves launcher-specific settings
        /// </summary>
        private void SaveLauncherSettings(ServerConfiguration config)
        {
            try
            {
                _logger.Debug("Saving launcher settings");

                string? dir = Path.GetDirectoryName(_settingsPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    _logger.Debug($"Created directory: {dir}");
                }

                StringBuilder settings = new StringBuilder();
                settings.AppendLine($"ValidateFiles={config.ValidateFiles.ToString().ToLower()}");
                settings.AppendLine($"DisableStreaming={config.DisableStreaming.ToString().ToLower()}");
                settings.AppendLine($"CustomArgs={config.CustomArgs}");
                settings.AppendLine($"ProcessPriority={config.ProcessPriority}");
                settings.AppendLine($"CpuAffinity={config.CpuAffinity}");
                settings.AppendLine($"GamePort={config.GamePort}");

                // Theme
                settings.AppendLine($"Theme={config.Theme}");

                // Updates
                settings.AppendLine($"CheckUpdatesOnStartup={config.CheckUpdatesOnStartup.ToString().ToLower()}");

                // Crash Detection
                settings.AppendLine($"EnableCrashDetection={config.EnableCrashDetection.ToString().ToLower()}");
                settings.AppendLine($"AutoRestart={config.AutoRestart.ToString().ToLower()}");
                settings.AppendLine($"MaxRestartAttempts={config.MaxRestartAttempts}");

                // Scheduled Restarts
                settings.AppendLine($"ScheduledRestartEnabled={config.ScheduledRestartEnabled.ToString().ToLower()}");
                settings.AppendLine($"RestartIntervalHours={config.RestartIntervalHours}");
                settings.AppendLine($"RestartWarningMinutes={config.RestartWarningMinutes}");
                settings.AppendLine($"RestartMessage={config.RestartMessage}");
                settings.AppendLine($"UseFixedRestartTimes={config.UseFixedRestartTimes.ToString().ToLower()}");
                settings.AppendLine($"FixedRestartTimes={config.FixedRestartTimes}");
                settings.AppendLine($"RestartScriptPath={config.RestartScriptPath}");
                settings.AppendLine($"RestartScriptDelaySeconds={config.RestartScriptDelaySeconds}");
                settings.AppendLine($"RestartScriptEnabled={config.RestartScriptEnabled.ToString().ToLower()}");

                // Discord Webhook
                settings.AppendLine($"EnableDiscordWebhook={config.EnableDiscordWebhook.ToString().ToLower()}");
                settings.AppendLine($"DiscordWebhookUrl={config.DiscordWebhookUrl}");

                // Mods
                settings.AppendLine($"ModLoaderPath={config.ModLoaderPath}");
                settings.AppendLine($"ModDllPath={config.ModDllPath}");
                settings.AppendLine($"ModConfigDir={config.ModConfigDir}");
                settings.AppendLine($"UseModBatInjection={config.UseModBatInjection.ToString().ToLower()}");
                settings.AppendLine($"AutoInjectAfterRestart={config.AutoInjectAfterRestart.ToString().ToLower()}");
                settings.AppendLine($"AutoInjectDelaySeconds={config.AutoInjectDelaySeconds}");

                // Auto Broadcast
                settings.AppendLine($"AutoBroadcastEnabled={config.AutoBroadcastEnabled.ToString().ToLower()}");
                settings.AppendLine($"AutoBroadcastMessage={config.AutoBroadcastMessage}");
                settings.AppendLine($"AutoBroadcastIntervalMinutes={config.AutoBroadcastIntervalMinutes}");

                // Automatic Backups
                settings.AppendLine($"AutoBackupEnabled={config.AutoBackupEnabled.ToString().ToLower()}");
                settings.AppendLine($"BackupIntervalHours={config.BackupIntervalHours}");

                // Maintenance
                settings.AppendLine($"AutoWipeCorpsesEnabled={config.AutoWipeCorpsesEnabled.ToString().ToLower()}");
                settings.AppendLine($"WipeCorpsesIntervalMinutes={config.WipeCorpsesIntervalMinutes}");
                settings.AppendLine($"WipeCorpsesDelayMinutes={config.WipeCorpsesDelayMinutes}");
                settings.AppendLine($"WipeWarningMessage={config.WipeWarningMessage}");
                settings.AppendLine($"WipeCompleteMessage={config.WipeCompleteMessage}");
                settings.AppendLine($"AutoRconSaveEnabled={config.AutoRconSaveEnabled.ToString().ToLower()}");
                settings.AppendLine($"RconSaveIntervalMinutes={config.RconSaveIntervalMinutes}");

                // Chat Monitor
                settings.AppendLine($"EnableChatMonitor={config.EnableChatMonitor.ToString().ToLower()}");
                settings.AppendLine($"EnableChatWebhook={config.EnableChatWebhook.ToString().ToLower()}");
                settings.AppendLine($"ChatWebhookUrl={config.ChatWebhookUrl}");
                settings.AppendLine($"ChatRefreshInterval={config.ChatRefreshInterval}");

                // Zombie Check
                settings.AppendLine($"EnableZombieCheck={config.EnableZombieCheck.ToString().ToLower()}");
                settings.AppendLine($"ZombieTimeoutSeconds={config.ZombieTimeoutSeconds}");

                // Debug Logging
                settings.AppendLine($"EnableLogRedpointEOSVerbose={config.EnableLogRedpointEOSVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogOnlineVerbose={config.EnableLogOnlineVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogOnlineGameVerbose={config.EnableLogOnlineGameVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogNetVerbose={config.EnableLogNetVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogNetTrafficVerbose={config.EnableLogNetTrafficVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogReplicationGraphVerbose={config.EnableLogReplicationGraphVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleVerbose={config.EnableLogTheIsleVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleAdminVerbose={config.EnableLogTheIsleAdminVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleAIVerbose={config.EnableLogTheIsleAIVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleAnimInstanceVerbose={config.EnableLogTheIsleAnimInstanceVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleAudioVerbose={config.EnableLogTheIsleAudioVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleAuthVerbose={config.EnableLogTheIsleAuthVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleCharacterVerbose={config.EnableLogTheIsleCharacterVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleCharacterMovementVerbose={config.EnableLogTheIsleCharacterMovementVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleDatabaseVerbose={config.EnableLogTheIsleDatabaseVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleEnvironmentVerbose={config.EnableLogTheIsleEnvironmentVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleGameVerbose={config.EnableLogTheIsleGameVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleNetworkVerbose={config.EnableLogTheIsleNetworkVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleServerVerbose={config.EnableLogTheIsleServerVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIslePlayerControllerVerbose={config.EnableLogTheIslePlayerControllerVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleUIVerbose={config.EnableLogTheIsleUIVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleWorldVerbose={config.EnableLogTheIsleWorldVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleJoinDataVerbose={config.EnableLogTheIsleJoinDataVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleChatDataVerbose={config.EnableLogTheIsleChatDataVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleKillDataVerbose={config.EnableLogTheIsleKillDataVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleCommandDataVerbose={config.EnableLogTheIsleCommandDataVerbose.ToString().ToLower()}");
                settings.AppendLine($"EnableLogTheIsleAntiCheatVerbose={config.EnableLogTheIsleAntiCheatVerbose.ToString().ToLower()}");

                var enabledDinos = config.Dinosaurs.Where(d => d.IsEnabled).Select(d => d.Name);
                settings.AppendLine($"EnabledDinos={string.Join(",", enabledDinos)}");

                // Every species the launcher knows about, enabled or not, so discovered
                // ones stay in the list after being unticked
                settings.AppendLine($"KnownDinos={string.Join(",", config.Dinosaurs.Select(d => d.Name))}");
                settings.AppendLine($"KnownAi={string.Join(",", config.DisallowedAIClasses.Select(a => a.Name))}");

                File.WriteAllText(_settingsPath, settings.ToString());
                _logger.Debug("Launcher settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving launcher settings: {ex.Message}", ex);
                throw new InvalidOperationException($"Error saving launcher settings: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates default Game.ini if it doesn't exist
        /// </summary>
        private void CreateDefaultGameConfig()
        {
            try
            {
                string? dir = Path.GetDirectoryName(_configPath);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[/Script/TheIsle.TIGameSession]");
                sb.AppendLine("ServerName=My Amazing Server");
                sb.AppendLine("bQueueEnabled=false");

                File.WriteAllText(_configPath, sb.ToString());
                _logger.Info("Created default Game.ini");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating default Game.ini: {ex.Message}", ex);
                // Don't throw - this is not critical
            }
        }

        /// <summary>
        /// Safely updates or adds a single INI value
        /// </summary>
        private void UpdateIniValue(List<string> lines, string section, string key, string? value)
        {
            int sectionIdx = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (IsSectionHeader(lines[i], section))
                {
                    sectionIdx = i;
                    break;
                }
            }

            if (sectionIdx == -1)
            {
                if (value == null) return;
                if (lines.Count > 0 && !string.IsNullOrWhiteSpace(lines.Last()))
                    lines.Add("");
                lines.Add($"[{section}]");
                lines.Add($"{key}={value}");
                return;
            }

            // A section can appear more than once and UE merges the blocks, so a scalar left
            // behind in a later block would shadow the one written here and the setting would
            // never stick. Collect every occurrence across all blocks of this section.
            var keyIdxs = new List<int>();
            int endOfFirstBlock = lines.Count;
            bool inSection = false;

            for (int i = sectionIdx; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                if (TryGetSectionName(line, out _))
                {
                    inSection = IsSectionHeader(lines[i], section);
                    if (!inSection && endOfFirstBlock == lines.Count && i > sectionIdx)
                        endOfFirstBlock = i;
                    continue;
                }

                if (inSection && TryMatchIniKey(line, key, out _)) keyIdxs.Add(i);
            }

            // Collapse onto the LAST occurrence. A repeated assignment overwrites the earlier
            // one, so the last is the value the server is actually running with; keeping any
            // other would silently change the server's behaviour while tidying the file. The
            // reader picks the last match for the same reason - the two must not disagree.
            int keepIdx = keyIdxs.Count > 0 ? keyIdxs[keyIdxs.Count - 1] : -1;

            if (keyIdxs.Count > 1)
            {
                _logger.Warning($"Game.ini defines '{key}' {keyIdxs.Count} times under " +
                                $"[{section}]. Keeping the last value, which is the one the " +
                                "server uses, and removing the earlier copies.");
            }

            for (int k = keyIdxs.Count - 2; k >= 0; k--)
            {
                lines.RemoveAt(keyIdxs[k]);
                if (keyIdxs[k] < keepIdx) keepIdx--;
                if (keyIdxs[k] < endOfFirstBlock) endOfFirstBlock--;
            }

            if (value == null)
            {
                if (keepIdx != -1)
                    lines.RemoveAt(keepIdx);
            }
            else
            {
                if (keepIdx != -1)
                    lines[keepIdx] = $"{key}={value}";
                else
                    lines.Insert(endOfFirstBlock, $"{key}={value}");
            }
        }

        /// <summary>
        /// Safely updates or replaces a list of INI values (like AllowedClasses)
        /// </summary>
        /// <remarks>
        /// Every line for the key is replaced, so <paramref name="newValues"/> must already
        /// include the entries the loader preserved verbatim as well as the ones the UI owns.
        /// Skipping lines here instead would make an entry shown in the UI impossible to delete.
        /// </remarks>
        private void UpdateIniList(List<string> lines, string section, string keyPrefix,
                                   List<string> newValues)
        {
            // A section can appear more than once; UE merges the blocks and so does the reader.
            // Purging only the first block would leave entries the loader still sees, so a
            // deleted admin ID or unticked species would quietly come back on the next load.
            int firstSectionIdx = -1;
            var staleKeyLines = new List<int>();
            bool inSection = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();

                if (TryGetSectionName(trimmed, out _))
                {
                    inSection = IsSectionHeader(lines[i], section);
                    if (inSection && firstSectionIdx == -1) firstSectionIdx = i;
                    continue;
                }

                if (inSection && TryMatchIniKey(trimmed, keyPrefix, out _)) staleKeyLines.Add(i);
            }

            if (firstSectionIdx == -1)
            {
                if (newValues.Count == 0) return;
                if (lines.Count > 0 && !string.IsNullOrWhiteSpace(lines.Last()))
                    lines.Add("");
                lines.Add($"[{section}]");
                lines.AddRange(newValues);
                return;
            }

            // Everything is rewritten into the first block, so find where that block ends.
            int insertAt = lines.Count;
            for (int i = firstSectionIdx + 1; i < lines.Count; i++)
            {
                if (TryGetSectionName(lines[i], out _))
                {
                    insertAt = i;
                    break;
                }
            }

            for (int k = staleKeyLines.Count - 1; k >= 0; k--)
            {
                lines.RemoveAt(staleKeyLines[k]);
                if (staleKeyLines[k] < insertAt) insertAt--;
            }

            lines.InsertRange(insertAt, newValues);
        }

        /// <summary>
        /// Reads the AllowedClasses entries the game actually honours, in file order - that is,
        /// only those in the section the property is declared on and the save path rewrites.
        /// </summary>
        private static List<string> ExtractAllowedClasses(string content)
        {
            return SplitIniEntries(ReadIniValuesInSections(content, "AllowedClasses", StateSection),
                                   ClassNameEntry).Recognised;
        }

        /// <summary>
        /// Returns the leading run of valid characters, or null if the value doesn't start with
        /// one. Used to repair a trailing-junk value rather than discard it.
        /// </summary>
        /// <remarks>
        /// Discarding is not a safe default here: the save path's removal predicate matches the
        /// whole line regardless, so an entry this rejects is deleted from Game.ini without ever
        /// being shown - losing an admin's access or a species silently. Salvaging the prefix
        /// keeps the round trip lossless and rewrites the line in clean form.
        /// </remarks>
        private static string? TakeLeadingToken(string value, Regex leading)
        {
            var match = leading.Match(value);
            return match.Success ? match.Value : null;
        }

        /// <summary>
        /// Logs a warning when a key appears outside the section its property is declared on.
        /// </summary>
        /// <remarks>
        /// The game ignores those lines, so the launcher deliberately does not show them, move
        /// them or delete them - moving one would turn a server that allows everything into a
        /// restricted one on upgrade, and deleting it would throw away an admin's list. A log
        /// line is enough to explain why an entry the admin can see in the file isn't in the UI.
        /// </remarks>
        private void WarnAboutIgnoredSectionEntries(string content, string key)
        {
            string? currentSection = null;

            foreach (var rawLine in content.Split('\n'))
            {
                string line = rawLine.Trim();

                if (TryGetSectionName(line, out string header))
                {
                    currentSection = header;
                    continue;
                }

                if (currentSection == null) continue;
                if (currentSection.Equals(StateSection, StringComparison.OrdinalIgnoreCase)) continue;
                if (!TryMatchIniKey(line, key, out _)) continue;

                _logger.Warning($"Game.ini has '{key}' under [{currentSection}], but the game only " +
                                $"reads it from [{StateSection}]. That line is ignored by the server " +
                                "and is left untouched by the launcher.");
            }
        }

        /// <summary>
        /// True if the line is the header for the given section. Readers and writers must use
        /// this one rule: if the reader recognises a header the writer doesn't, the save can't
        /// find the section, appends a duplicate at end of file and leaves the originals in
        /// place - so unticking a species or deleting an admin appears to work and then comes
        /// back on the next load.
        /// </summary>
        private static bool IsSectionHeader(string line, string section) =>
            TryGetSectionName(line, out string name) &&
            name.Equals(section, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Extracts the section name from a header line, or false if the line isn't one.
        /// </summary>
        /// <remarks>
        /// Only the bracketed part counts; UE ignores whatever follows, so
        /// "[/Script/TheIsle.TIGameStateBase] ; my admins" is still that section. Requiring the
        /// line to end in "]" hid such a section from the readers while the server still honoured
        /// it - the UI showed no admins at all and the save appended a second header.
        /// </remarks>
        private static bool TryGetSectionName(string line, out string name)
        {
            name = "";

            string trimmed = line.Trim();
            if (!trimmed.StartsWith("[")) return false;

            int close = trimmed.IndexOf(']');
            if (close < 1) return false;

            name = trimmed.Substring(1, close - 1).Trim();
            return name.Length > 0;
        }

        /// <summary>
        /// Splits raw ini list values into the entries the game can act on and the ones it
        /// can't, accepting every form the writer also matches: quoted, parenthesised and
        /// comma-joined.
        /// </summary>
        /// <remarks>
        /// This is the rule that keeps two promises at once. Everything in <c>Recognised</c> is
        /// shown in the UI and rewritten from it, so it can always be removed. Everything in
        /// <c>Preserved</c> is written back verbatim, so a value the launcher doesn't understand
        /// - a legacy "STEAM_0:1:5", an asset path, a hand-written note - survives untouched
        /// rather than being deleted for being unreadable. Neither list can silently lose data.
        /// </remarks>
        private static (List<string> Recognised, List<string> Preserved) SplitIniEntries(
            IEnumerable<string> rawValues, Regex entryPattern)
        {
            var recognised = new List<string>();
            var preserved = new List<string>();

            foreach (var value in rawValues)
            {
                foreach (var part in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Wrapper characters are stripped to recognise the value, but what gets
                    // preserved is the original text - trimming first would write back a
                    // "Deer (herbivore" that has lost its closing bracket.
                    string original = part.Trim();
                    string entry = original.Trim('(', ')', '"').Trim();
                    if (entry.Length == 0) continue;

                    if (entryPattern.IsMatch(entry)) recognised.Add(entry);
                    else preserved.Add(original);
                }
            }

            return (recognised, preserved);
        }

        /// <summary>
        /// The entries for a key that the loader could not represent, formatted as ini lines so
        /// the save can put them back exactly as they were.
        /// </summary>
        private static List<string> PreservedEntries(string content, string key, Regex entryPattern,
                                                     params string[] sections)
        {
            return SplitIniEntries(ReadIniValuesInSections(content, key, sections), entryPattern)
                .Preserved
                .Select(value => $"{key}={value}")
                .ToList();
        }

        private static List<string> PreservedEntries(string content, string key, string section,
                                                     Regex entryPattern) =>
            PreservedEntries(content, key, entryPattern, section);

        /// <summary>
        /// Reads a Steam ID list from the one section the game reads it from, keeping only
        /// well-formed numeric IDs and collapsing repeats.
        /// </summary>
        private static List<string> ReadIniIdList(string content, string key, string section)
        {
            return SplitIniEntries(ReadIniValuesInSections(content, key, section), SteamIdEntry)
                .Recognised
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// Adds species that aren't part of the launcher's built-in roster so they survive
        /// a load/save round trip instead of being stripped out of Game.ini.
        /// </summary>
        private void MergeDiscoveredDinos(ServerConfiguration config, IEnumerable<string> names, string source, bool enabledIfNew = false)
        {
            foreach (var raw in names)
            {
                var name = raw?.Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (config.Dinosaurs.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) continue;

                config.Dinosaurs.Add(new DinoOption { Name = name, IsEnabled = enabledIfNew });
                _logger.Info($"Discovered non-standard playable '{name}' from {source}; preserving it (enabled={enabledIfNew}).");
            }
        }

        /// <summary>
        /// Matches "Key=value" and the spaced "Key = value" form that UpdateIniValue also
        /// tolerates. Reader and writer must agree on this, or a spaced line is invisible to
        /// the loader yet survives the rewrite and shadows the correct entry.
        /// </summary>
        private static bool TryMatchIniKey(string trimmedLine, string key, out string value)
        {
            value = "";

            // Inside a single ini, UE's "+Key=" append form means the same as "Key=" for the
            // repeated list keys this helper serves, and their readers accept it, so the writer
            // must be able to replace those lines too - otherwise an entry loads into the UI,
            // survives the rewrite and gets duplicated. Scalar keys go through GetConfigValue /
            // UpdateIniValue instead, which don't take a prefix; "+" is meaningless there.
            // "-", "!" and "." carry different semantics and are deliberately left untouched.
            if (trimmedLine.StartsWith("+")) trimmedLine = trimmedLine.Substring(1).TrimStart();

            if (!trimmedLine.StartsWith(key, StringComparison.OrdinalIgnoreCase)) return false;

            int i = key.Length;
            while (i < trimmedLine.Length && (trimmedLine[i] == ' ' || trimmedLine[i] == '\t')) i++;
            if (i >= trimmedLine.Length || trimmedLine[i] != '=') return false;

            value = trimmedLine.Substring(i + 1).Trim();
            return true;
        }

        /// <summary>
        /// Reads every value of a repeated key, but only from the sections given. Scanning the
        /// whole file would pick up occurrences the save path never rewrites, so unticking
        /// such an entry would appear to work and then come back on the next load.
        /// </summary>
        private static List<string> ReadIniValuesInSections(string content, string key, params string[] sections)
        {
            var wanted = new HashSet<string>(sections, StringComparer.OrdinalIgnoreCase);
            var results = new List<string>();
            string? currentSection = null;

            foreach (var rawLine in content.Split('\n'))
            {
                string line = rawLine.Trim();
                if (TryGetSectionName(line, out string header))
                {
                    currentSection = header;
                    continue;
                }

                if (currentSection == null || !wanted.Contains(currentSection)) continue;
                if (!TryMatchIniKey(line, key, out string value)) continue;

                results.Add(value);
            }

            return results;
        }

        /// <summary>
        /// Reads a single-valued key, accepting the same line shapes UpdateIniValue writes and
        /// matches: leading indent, and spaces or tabs around the "=".
        /// </summary>
        /// <remarks>
        /// This reader used to demand the key at column zero with "=" immediately after it,
        /// while the writer matched "Key =" too. A spaced "AIDensity = 3" therefore read as
        /// absent, fell back to the default, and was rewritten as "AIDensity=1" - silently
        /// resetting the operator's value on the next save.
        /// </remarks>
        private string? GetConfigValue(string content, string key, string? section = null)
        {
            string? currentSection = null;
            string? lastMatch = null;

            foreach (var rawLine in content.Split('\n'))
            {
                string line = rawLine.Trim();

                if (TryGetSectionName(line, out string header))
                {
                    currentSection = header;
                    continue;
                }

                if (section != null &&
                    !string.Equals(currentSection, section, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!TryMatchIniKey(line, key, out string value)) continue;

                value = value.Trim();

                // Strip one matched pair of wrapping quotes, leaving any inside the value alone.
                if (value.Length >= 2 && value.StartsWith("\"") && value.EndsWith("\""))
                    value = value.Substring(1, value.Length - 2);

                // Keep looking: a later assignment overwrites an earlier one, so the last match
                // is the value in force. UpdateIniValue collapses duplicates onto the same one.
                lastMatch = value.Trim();
            }

            return lastMatch;
        }

        private bool GetBoolValue(string content, string key, bool defaultValue = false)
        {
            var value = GetConfigValue(content, key);
            if (value == null) return defaultValue;
            return value.ToLower() == "true";
        }

        /// <summary>
        /// Reads a Game.ini scalar from the section the game reads it from and the save path
        /// writes it to. Every scalar the launcher manages lives on ATIGameSession.
        /// </summary>
        /// <remarks>
        /// Scanning the whole file instead would let a stray copy in another section shadow the
        /// real value: the loader would show the stray, the save would rewrite the real one, and
        /// the setting would appear to revert on every restart.
        /// </remarks>
        private string? GetGameIniValue(string content, string key) =>
            GetConfigValue(content, key, SessionSection);

        private bool GetGameIniBool(string content, string key, bool defaultValue = false)
        {
            var value = GetGameIniValue(content, key);
            if (value == null) return defaultValue;
            return value.ToLower() == "true";
        }
    }
}