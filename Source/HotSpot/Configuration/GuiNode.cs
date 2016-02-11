using HotSpot.Configuration.Gui;

namespace HotSpot.Configuration
{
    internal sealed class GuiNode
    {
        // TODO: This is a legacy option that should be removed in a future version
        public string ButtonTexture { get; }

        public AppLauncherNode AppLauncher { get; }
        public ToolbarNode Toolbar { get; }

        private GuiNode(string buttonTexture, AppLauncherNode appLauncher, ToolbarNode toolbar)
        {
            ButtonTexture = buttonTexture;
            AppLauncher = appLauncher;
            Toolbar = toolbar;
        }

        public bool Save(ConfigNode node)
            => false;

        public static GuiNode GetDefault()
            => new GuiNode(null, AppLauncherNode.GetDefault(), ToolbarNode.GetDefault());

        public static GuiNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var buttonTexture = node.GetValue("buttonTexture");
                var appLauncher = AppLauncherNode.TryParse(node.GetNode("APPLAUNCHER"))
                    ?? AppLauncherNode.GetDefault();
                var toolbar = ToolbarNode.TryParse(node.GetNode("TOOLBAR"))
                    ?? ToolbarNode.GetDefault();

                return new GuiNode(buttonTexture, appLauncher, toolbar);
            }

            Log.Warning("Could not parse missing GUI node");
            return null;
        }
    }
}
