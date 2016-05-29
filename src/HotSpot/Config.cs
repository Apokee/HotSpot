using System;
using System.IO;
using System.Linq;
using HotSpot.Configuration;

namespace HotSpot
{
    internal sealed class Config
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
                            Config staticConfig = null;
                            Config dynamicConfig = null;

                            var staticConfigNode = GetStaticConfigNode();
                            if (staticConfigNode != null)
                                staticConfig = TryParse(staticConfigNode);

                            var dynamicConfigNode = GetDynamicConfigNode();
                            if (dynamicConfigNode != null)
                                dynamicConfig = TryParse(dynamicConfigNode);

                            var defaultConfig = new Config();

                            _instance = Merge(dynamicConfig ?? defaultConfig, staticConfig ?? defaultConfig);

                            OnInitialLoad(staticConfigNode, dynamicConfigNode);
                        }
                    }
                }

                return _instance;
            }
        }

        private static void OnInitialLoad(ConfigNode staticNode, ConfigNode dynamicNode)
        {
            Log.Level = Instance.Diagnostics.LogLevel;

            Log.Debug(staticNode != null ?
                $"Static Configuration:{Environment.NewLine}{staticNode}" :
                "No static configuration found."
            );

            Log.Debug(staticNode != null ?
                $"Dynamic Configuration:{Environment.NewLine}{dynamicNode}" :
                "No dynamic configuration found."
            );

            GameEvents.onGameSceneSwitchRequested.Add(e => Instance.Save());
        }

        #endregion

        public GuiNode Gui { get; }
        public ContextMenuNode ContextMenu { get; }
        public OverlayNode Overlay { get; }
        public DiagnosticsNode Diagnostics { get; }

        private Config()
        {
            Gui = GuiNode.GetDefault();
            ContextMenu = ContextMenuNode.GetDefault();
            Overlay = OverlayNode.GetDefault();
            Diagnostics = DiagnosticsNode.GetDefault();
        }

        private Config(GuiNode gui, ContextMenuNode contextMenu, OverlayNode overlay, DiagnosticsNode diagnostics)
        {
            Gui = gui;
            ContextMenu = contextMenu;
            Overlay = overlay;
            Diagnostics = diagnostics;
        }

        public void Save()
        {
            var node = new ConfigNode("HOT_SPOT");

            var contextMenuNode = new ConfigNode("CONTEXT_MENU");
            var overlayNode = new ConfigNode("OVERLAY");

            ContextMenu.Save(contextMenuNode);
            node.AddNode(contextMenuNode);

            Overlay.Save(overlayNode);
            node.AddNode(overlayNode);

            FileSystem.WriteDynamicConfiguration(node);
            FileSystem.EnsureLegacyPlayerConfigFileDoesNotExist();
        }

        public static Config TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var gui = GuiNode.TryParse(node.GetNode("GUI")) ??
                    GuiNode.GetDefault();

                var contextMenu = ContextMenuNode.TryParse(node.GetNode("CONTEXT_MENU")) ??
                    ContextMenuNode.GetDefault();

                var overlay = OverlayNode.TryParse(node.GetNode("OVERLAY")) ??
                    OverlayNode.GetDefault();

                var diagnostics = DiagnosticsNode.TryParse(node.GetNode("DIAGNOSTICS")) ??
                    DiagnosticsNode.GetDefault();

                return new Config(gui, contextMenu, overlay, diagnostics);
            }

            Log.Debug("Could not parse missing HOT_SPOT node");
            return null;
        }

        public static ConfigNode GetStaticConfigNode()
        {
            return GameDatabase.Instance.GetConfigNodes("HOT_SPOT").SingleOrDefault();
        }

        public static ConfigNode GetDynamicConfigNode()
        {
            if (File.Exists(FileSystem.DynamicConfigFilePath))
            {
                var hotSpotNode = ConfigNode.Load(FileSystem.DynamicConfigFilePath).GetNode("HOT_SPOT");
                if (hotSpotNode != null)
                    return hotSpotNode;
            }

            return null;
        }

        public static Config Merge(Config source, Config target)
        {
            // CONTEXT_MENU

            var targetContextMenuMetrics = target.ContextMenu.Metrics.ToDictionary(i => i.Name.Name);
            foreach (var sourceMetric in source.ContextMenu.Metrics)
            {
                Configuration.ContextMenu.MetricNode targetMetric;
                if (targetContextMenuMetrics.TryGetValue(sourceMetric.Name.Name, out targetMetric))
                {
                    targetMetric.Enable = sourceMetric.Enable;
                    targetMetric.Prefix = sourceMetric.Prefix;
                    targetMetric.Unit = sourceMetric.Unit;
                }
            }

            // OVERLAY

            target.Overlay.Metric = source.Overlay.Metric;

            var targetOverlayMetrics = target.Overlay.Metrics.ToDictionary(i => i.Name.Name);
            foreach (var sourceMetric in source.Overlay.Metrics)
            {
                Configuration.Overlay.MetricNode targetMetric;
                if (targetOverlayMetrics.TryGetValue(sourceMetric.Name.Name, out targetMetric))
                {
                    targetMetric.Scheme = sourceMetric.Scheme;
                }
            }

            return target;
        }
    }
}
