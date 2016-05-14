namespace HotSpot.Configuration.Gui
{
    internal sealed class ToolbarNode
    {
        public AutoBoolean Enable { get; }
        public string Texture { get;  }

        public ToolbarNode(AutoBoolean enable, string texture)
        {
            Enable = enable;
            Texture = texture;
        }

        public bool Save(ConfigNode node)
            => false;

        public static ToolbarNode GetDefault()
            => new ToolbarNode(new AutoBoolean(null), null);

        public static ToolbarNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var enable = node.TryParse<AutoBoolean>("enable") ?? new AutoBoolean(null);
                var texture = node.GetValue("texture");

                return new ToolbarNode(enable, texture);
            }

            Log.Warning("Could not parse missing TOOLBAR node");
            return null;
        }
    }
}
