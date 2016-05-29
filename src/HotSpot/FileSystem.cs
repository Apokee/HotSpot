using System;
using System.IO;
using System.Reflection;

namespace HotSpot
{
    internal static class FileSystem
    {
        private static string _modDirectoryPath;
        private static string _pluginDataDirectoryPath;
        private static string _dynamicConfigFilePath;

        public static string ModDirectoryPath
        {
            get
            {
                if (_modDirectoryPath == null)
                {
                    _modDirectoryPath =
                        Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.Parent?.FullName;

                    if (_modDirectoryPath == null)
                        throw new Exception("Unable to find HotSpot root directory.");
                }

                return _modDirectoryPath;
            }
        }

        public static string PluginDataDirectoryPath =>
            _pluginDataDirectoryPath ?? (_pluginDataDirectoryPath = Path.Combine(ModDirectoryPath, "PluginData"));

        public static string DynamicConfigFilePath =>
            _dynamicConfigFilePath ?? (_dynamicConfigFilePath = Path.Combine(PluginDataDirectoryPath, "HotSpot.cfg"));

        public static void WriteDynamicConfiguration(ConfigNode config)
        {
            if (!Directory.Exists(PluginDataDirectoryPath))
                Directory.CreateDirectory(PluginDataDirectoryPath);

            File.WriteAllText(DynamicConfigFilePath, config.ToString());
        }

        // TODO: LEGACY: Remove
        public static void EnsureLegacyPlayerConfigFileDoesNotExist()
        {
            var legacyDynamicFilePath = Path.Combine(ModDirectoryPath, "Configuration/HotSpotPlayer.cfg");

            if (File.Exists(legacyDynamicFilePath))
                File.Delete(legacyDynamicFilePath);
        }
    }
}
