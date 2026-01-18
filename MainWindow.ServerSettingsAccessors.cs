using System.Windows.Controls;

namespace IsleServerLauncher
{
    public partial class MainWindow
    {
        private TextBox txtServerName => serverSettingsView.txtServerName;
        private TextBox txtMaxPlayers => serverSettingsView.txtMaxPlayers;
        private TextBox txtServerPass => serverSettingsView.txtServerPass;
        private TextBox txtRconPass => serverSettingsView.txtRconPass;
        private TextBox txtRconPort => serverSettingsView.txtRconPort;
        private TextBox txtGamePort => serverSettingsView.txtGamePort;
        private TextBox txtQueuePort => serverSettingsView.txtQueuePort;
        private TextBox txtCustomArgs => serverSettingsView.txtCustomArgs;
        private TextBox txtDayLength => serverSettingsView.txtDayLength;
        private TextBox txtNightLength => serverSettingsView.txtNightLength;
        private TextBox txtGrowth => serverSettingsView.txtGrowth;
        private TextBox txtDecay => serverSettingsView.txtDecay;
        private TextBox txtMigrationTime => serverSettingsView.txtMigrationTime;
        private TextBox txtAISpawn => serverSettingsView.txtAISpawn;
        private TextBox txtAIDensity => serverSettingsView.txtAIDensity;
        private TextBox txtRegionSpawnCooldown => serverSettingsView.txtRegionSpawnCooldown;
        private TextBox txtPlantSpawnMultiplier => serverSettingsView.txtPlantSpawnMultiplier;
        private TextBox txtMassMigrationTime => serverSettingsView.txtMassMigrationTime;
        private TextBox txtMassMigrationDisableTime => serverSettingsView.txtMassMigrationDisableTime;
        private TextBox txtSpeciesMigrationTime => serverSettingsView.txtSpeciesMigrationTime;
        private TextBox txtMinWeatherVariationInterval => serverSettingsView.txtMinWeatherVariationInterval;
        private TextBox txtMaxWeatherVariationInterval => serverSettingsView.txtMaxWeatherVariationInterval;
        private TextBox txtQueueJoinTimeoutSeconds => serverSettingsView.txtQueueJoinTimeoutSeconds;
        private TextBox txtQueueHeartbeatIntervalSeconds => serverSettingsView.txtQueueHeartbeatIntervalSeconds;
        private TextBox txtQueueHeartbeatTimeoutSeconds => serverSettingsView.txtQueueHeartbeatTimeoutSeconds;
        private TextBox txtQueueHeartbeatMaxMisses => serverSettingsView.txtQueueHeartbeatMaxMisses;
        private TextBox txtAdminSteamIds => serverSettingsView.txtAdminSteamIds;
        private TextBox txtWhitelistIds => serverSettingsView.txtWhitelistIds;
        private TextBox txtVipIds => serverSettingsView.txtVipIds;
        private TextBox txtDiscordInvite => serverSettingsView.txtDiscordInvite;

        private CheckBox chkEnableRcon => serverSettingsView.chkEnableRcon;
        private CheckBox chkWhitelist => serverSettingsView.chkWhitelist;
        private CheckBox chkQueueEnabled => serverSettingsView.chkQueueEnabled;
        private CheckBox chkGlobalChat => serverSettingsView.chkGlobalChat;
        private CheckBox chkHumans => serverSettingsView.chkHumans;
        private CheckBox chkMutations => serverSettingsView.chkMutations;
        private CheckBox chkMigration => serverSettingsView.chkMigration;
        private CheckBox chkFallDamage => serverSettingsView.chkFallDamage;
        private CheckBox chkSpawnAI => serverSettingsView.chkSpawnAI;
        private CheckBox chkPlants => serverSettingsView.chkPlants;
        private CheckBox chkDynamicWeather => serverSettingsView.chkDynamicWeather;
        private CheckBox chkUseRegionSpawnCooldown => serverSettingsView.chkUseRegionSpawnCooldown;
        private CheckBox chkUseRegionSpawning => serverSettingsView.chkUseRegionSpawning;
        private CheckBox chkAllowRecordingReplay => serverSettingsView.chkAllowRecordingReplay;
        private CheckBox chkEnableDiets => serverSettingsView.chkEnableDiets;
        private CheckBox chkEnablePatrolZones => serverSettingsView.chkEnablePatrolZones;
        private CheckBox chkEnableMassMigration => serverSettingsView.chkEnableMassMigration;

        private CheckBox chkEnableLogRedpointEOSVerbose => serverSettingsView.chkEnableLogRedpointEOSVerbose;
        private CheckBox chkEnableLogOnlineVerbose => serverSettingsView.chkEnableLogOnlineVerbose;
        private CheckBox chkEnableLogOnlineGameVerbose => serverSettingsView.chkEnableLogOnlineGameVerbose;
        private CheckBox chkEnableLogNetVerbose => serverSettingsView.chkEnableLogNetVerbose;
        private CheckBox chkEnableLogNetTrafficVerbose => serverSettingsView.chkEnableLogNetTrafficVerbose;
        private CheckBox chkEnableLogReplicationGraphVerbose => serverSettingsView.chkEnableLogReplicationGraphVerbose;
        private CheckBox chkEnableLogTheIsleVerbose => serverSettingsView.chkEnableLogTheIsleVerbose;
        private CheckBox chkEnableLogTheIsleAdminVerbose => serverSettingsView.chkEnableLogTheIsleAdminVerbose;
        private CheckBox chkEnableLogTheIsleAIVerbose => serverSettingsView.chkEnableLogTheIsleAIVerbose;
        private CheckBox chkEnableLogTheIsleAnimInstanceVerbose => serverSettingsView.chkEnableLogTheIsleAnimInstanceVerbose;
        private CheckBox chkEnableLogTheIsleAudioVerbose => serverSettingsView.chkEnableLogTheIsleAudioVerbose;
        private CheckBox chkEnableLogTheIsleAuthVerbose => serverSettingsView.chkEnableLogTheIsleAuthVerbose;
        private CheckBox chkEnableLogTheIsleCharacterVerbose => serverSettingsView.chkEnableLogTheIsleCharacterVerbose;
        private CheckBox chkEnableLogTheIsleCharacterMovementVerbose => serverSettingsView.chkEnableLogTheIsleCharacterMovementVerbose;
        private CheckBox chkEnableLogTheIsleDatabaseVerbose => serverSettingsView.chkEnableLogTheIsleDatabaseVerbose;
        private CheckBox chkEnableLogTheIsleEnvironmentVerbose => serverSettingsView.chkEnableLogTheIsleEnvironmentVerbose;
        private CheckBox chkEnableLogTheIsleGameVerbose => serverSettingsView.chkEnableLogTheIsleGameVerbose;
        private CheckBox chkEnableLogTheIsleNetworkVerbose => serverSettingsView.chkEnableLogTheIsleNetworkVerbose;
        private CheckBox chkEnableLogTheIsleServerVerbose => serverSettingsView.chkEnableLogTheIsleServerVerbose;
        private CheckBox chkEnableLogTheIslePlayerControllerVerbose => serverSettingsView.chkEnableLogTheIslePlayerControllerVerbose;
        private CheckBox chkEnableLogTheIsleUIVerbose => serverSettingsView.chkEnableLogTheIsleUIVerbose;
        private CheckBox chkEnableLogTheIsleWorldVerbose => serverSettingsView.chkEnableLogTheIsleWorldVerbose;
        private CheckBox chkEnableLogTheIsleJoinDataVerbose => serverSettingsView.chkEnableLogTheIsleJoinDataVerbose;
        private CheckBox chkEnableLogTheIsleChatDataVerbose => serverSettingsView.chkEnableLogTheIsleChatDataVerbose;
        private CheckBox chkEnableLogTheIsleKillDataVerbose => serverSettingsView.chkEnableLogTheIsleKillDataVerbose;
        private CheckBox chkEnableLogTheIsleCommandDataVerbose => serverSettingsView.chkEnableLogTheIsleCommandDataVerbose;
        private CheckBox chkEnableLogTheIsleAntiCheatVerbose => serverSettingsView.chkEnableLogTheIsleAntiCheatVerbose;

        private ListBox lstDinos => serverSettingsView.lstDinos;
        private ListBox lstDisallowedAI => serverSettingsView.lstDisallowedAI;
    }
}
