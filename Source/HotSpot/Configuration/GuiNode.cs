using HotSpot.Configuration.Gui;

namespace HotSpot.Configuration
{
    internal sealed class GuiNode
    {
        public AppLauncherNode AppLauncher { get; }
        public ToolbarNode Toolbar { get; }

        private GuiNode(AppLauncherNode appLauncher, ToolbarNode toolbar)
        {
            AppLauncher = appLauncher;
            Toolbar = toolbar;
        }

        public bool Save(ConfigNode node)
            => false;

        public static GuiNode GetDefault()
            => new GuiNode(AppLauncherNode.GetDefault(), ToolbarNode.GetDefault());

        public static GuiNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var appLauncher = AppLauncherNode.TryParse(node.GetNode("APPLAUNCHER"))
                    ?? AppLauncherNode.GetDefault();
                var toolbar = ToolbarNode.TryParse(node.GetNode("TOOLBAR"))
                    ?? ToolbarNode.GetDefault();

                return new GuiNode(appLauncher, toolbar);
            }

            Log.Warning("Could not parse missing GUI node");
            return null;
        }
    }
}
