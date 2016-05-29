namespace HotSpot.Configuration.Gui
{
    internal sealed class AppLauncherNode
    {
        public AutoBoolean Enable { get; }
        public string Texture { get;  }

        public AppLauncherNode(AutoBoolean enable, string texture)
        {
            Enable = enable;
            Texture = texture;
        }

        public bool Save(ConfigNode node)
            => false;

        public static AppLauncherNode GetDefault()
            => new AppLauncherNode(new AutoBoolean(null), null);

        public static AppLauncherNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var enable = node.TryParse<AutoBoolean>("enable") ?? new AutoBoolean(null);
                var texture = node.GetValue("texture");

                return new AppLauncherNode(enable, texture);
            }

            return null;
        }
    }
}
