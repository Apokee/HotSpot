using System;
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
                            
                            var node = GameDatabase
                                .Instance
                                .GetConfigNodes("HOT_SPOT")
                                .SingleOrDefault();

                            _instance = TryParse(node) ?? new Config();

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
            ContextMenu = contextMenu;
            Overlay = overlay;
            Diagnostics = diagnostics;
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

            Log.Warning("Could not parse missing HOT_SPOT node");
            return null;
        }
    }
}
