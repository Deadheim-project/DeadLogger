using BepInEx;
using UnityEngine;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;

namespace DeadLogger
{
    [BepInPlugin(PluginGUID, PluginGUID, Version)]
    public class DeadLogger : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.DeadLogger";
        public const string Name = "DeadLogger";
        public const string Version = "1.0.0";

        ConfigSync configSync = new ConfigSync("Detalhes.DeadLogger") { DisplayName = "DeadLogger", CurrentVersion = Version, MinimumRequiredVersion = Version };

        Harmony _harmony = new Harmony(PluginGUID);

        public static ConfigEntry<string> PrefabsToLog;

        public static GameObject Menu;

        public static string FileDirectory = BepInEx.Paths.ConfigPath + @"/DeadLogger/";

        public void Awake()
        {
            PrefabsToLog = config("Server config", "PrefabsToLog", "",
                   new ConfigDescription("PrefabsToLog", null));
            _harmony.PatchAll();
        }

        ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => config(group, name, value, new ConfigDescription(description), synchronizedSetting);

    }
}
