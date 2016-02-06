namespace HotSpot.Configuration
{
    internal sealed class GuiNode
    {
        public string ButtonTexture { get; }
        public string ButtonTexture24 { get; }

        private GuiNode(string buttonTexture, string buttonTexture24)
        {
            ButtonTexture = buttonTexture;
            ButtonTexture24 = buttonTexture24;
        }

        public bool Save(ConfigNode node) => false;

        public static GuiNode GetDefault() => new GuiNode(buttonTexture: null, buttonTexture24: null);

        public static GuiNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var buttonTexture = node.GetValue("buttonTexture");
                var buttonTexture24 = node.GetValue("buttonTexture24");

                return new GuiNode(buttonTexture, buttonTexture24);
            }

            Log.Warning("Could not parse missing GUI node");
            return null;
        }
    }
}
