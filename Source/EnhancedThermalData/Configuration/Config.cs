using System;
using System.Linq;
using EnhancedThermalData.Diagnostics;

namespace EnhancedThermalData.Configuration
{
    internal sealed class Config : IConfigNode
    {
        #region Singleton

        private static readonly object InstanceLock = new object();
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            var config = new Config();
                            config.Load(
                                GameDatabase.Instance.GetConfigNodes("ENHANCED_THERMAL_DATA").SingleOrDefault()
                            );

                            Log.Info($"BLAH: {GameDatabase.Instance.GetConfigNodes("ENHANCED_THERMAL_DATA").SingleOrDefault()}");

                            _instance = config;

                            OnInitialLoad();
                        }
                    }
                }

                return _instance;
            }
        }

        private static void OnInitialLoad()
        {
            Log.Level = Instance.Diagnostics.LogLevel;
        }

        #endregion

        public ContextMenuNode ContextMenu { get; } = new ContextMenuNode();
        public OverlayNode Overlay { get; } = new OverlayNode();
        public DiagnosticsNode Diagnostics { get; } = new DiagnosticsNode();

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                ContextMenu.Load(node.GetNode("CONTEXT_MENU"));
                Overlay.Load(node.GetNode("OVERLAY"));
                Diagnostics.Load(node.GetNode("DIAGNOSTICS"));
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
