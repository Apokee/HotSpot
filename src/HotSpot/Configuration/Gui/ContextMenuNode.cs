namespace HotSpot.Configuration.Gui
{
    internal sealed class ContextMenuNode
    {
        public bool DisableStockCoreTemp { get; }

        public ContextMenuNode(bool disableStockCoreTemp)
        {
            DisableStockCoreTemp = disableStockCoreTemp;
        }

        public bool Save(ConfigNode node)
            => false;

        public static ContextMenuNode GetDefault()
            => new ContextMenuNode(true);

        public static ContextMenuNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var disableStockCoreTemp = node.TryParse<bool>("disableStockCoreTemp") ?? true;

                return new ContextMenuNode(disableStockCoreTemp);
            }

            return null;
        }
    }
}
