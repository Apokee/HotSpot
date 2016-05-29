using HotSpot.Configuration.Gui;

namespace HotSpot.Configuration
{
    internal sealed class GuiNode
    {
        public AppLauncherNode AppLauncher { get; }
        public ToolbarNode Toolbar { get; }
        public Gui.ContextMenuNode ContextMenu { get; }

        private GuiNode(AppLauncherNode appLauncher, ToolbarNode toolbar, Gui.ContextMenuNode contextMenu)
        {
            AppLauncher = appLauncher;
            Toolbar = toolbar;
            ContextMenu = contextMenu;
        }

        public static GuiNode GetDefault()
            => new GuiNode(AppLauncherNode.GetDefault(), ToolbarNode.GetDefault(), Gui.ContextMenuNode.GetDefault());

        public static GuiNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var appLauncher = AppLauncherNode.TryParse(node.GetNode("APPLAUNCHER"))
                    ?? AppLauncherNode.GetDefault();
                var toolbar = ToolbarNode.TryParse(node.GetNode("TOOLBAR"))
                    ?? ToolbarNode.GetDefault();
                var contextMenu = Gui.ContextMenuNode.TryParse(node.GetNode("CONTEXT_MENU"))
                    ?? Gui.ContextMenuNode.GetDefault();

                return new GuiNode(appLauncher, toolbar, contextMenu);
            }

            return null;
        }
    }
}
