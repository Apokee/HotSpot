namespace HotSpot.Configuration
{
    internal sealed class GuiNode
    {
        public string ButtonTexture { get; }

        private GuiNode(string buttonTexture)
        {
            ButtonTexture = buttonTexture;
        }

        public static GuiNode GetDefault()
        {
            return new GuiNode(buttonTexture: null);
        }

        public static GuiNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var buttonTexture = node.GetValue("buttonTexture");

                return new GuiNode(buttonTexture);
            }

            Log.Warning("Could not parse missing GUI node");
            return null;
        }
    }
}
