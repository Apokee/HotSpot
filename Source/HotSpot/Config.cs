using System;
using System.Linq;
using EnhancedThermalData.Configuration;

namespace EnhancedThermalData
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
                            
                            var node = GameDatabase
                                .Instance
                                .GetConfigNodes("ENHANCED_THERMAL_DATA")
                                .SingleOrDefault();

                            _instance = new Config(node);

                            OnInitialLoad(node);
                        }
                    }
                }

                return _instance;
            }
        }

        private static void OnInitialLoad(ConfigNode node)
        {
            Log.Level = Instance.Diagnostics.LogLevel;

            Log.Debug(node != null ?
                $"Exploded Configuration:{Environment.NewLine}{node}" :
                "No configuration found."
            );
        }

        #endregion

        public ContextMenuNode ContextMenu { get; } = new ContextMenuNode();
        public OverlayNode Overlay { get; } = new OverlayNode();
        public DiagnosticsNode Diagnostics { get; } = new DiagnosticsNode();

        private Config(ConfigNode node)
        {
            Load(node);
        }

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
