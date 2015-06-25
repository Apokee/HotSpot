using System;
using System.IO;
using System.Linq;
using System.Text;
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
            // It appears the top-most node cannot use an edit-or-create operation (%) so use an edit operation (@)
            var node = new ConfigNode("@HOT_SPOT:AFTER[HotSpot]");

            var guiNode = new ConfigNode("%GUI");
            var contextMenuNode = new ConfigNode("%CONTEXT_MENU");
            var overlayNode = new ConfigNode("%OVERLAY");
            var diagnosticsNode = new ConfigNode("%DIAGNOSTICS");

            if (Gui.Save(guiNode))
            {
                node.AddNode(guiNode);
            }

            if (ContextMenu.Save(contextMenuNode))
            {
                node.AddNode(contextMenuNode);
            }

            if (Overlay.Save(overlayNode))
            {
                node.AddNode(overlayNode);
            }

            if (Diagnostics.Save(diagnosticsNode))
            {
                node.AddNode(diagnosticsNode);
            }

            File.WriteAllText(
                $"{KSPUtil.ApplicationRootPath}/GameData/HotSpot/Configuration/HotSpotPlayer.cfg",
                node.ToString(),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
            );
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
