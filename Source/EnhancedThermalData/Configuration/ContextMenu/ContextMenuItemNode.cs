using System;

namespace EnhancedThermalData.Configuration.ContextMenu
{
    internal class ContextMenuItemNode : IConfigNode
    {
        public bool Enable { get; private set; } = true;

        public virtual void Load(ConfigNode node)
        {
            Enable = node.Parse<bool>("enable");
        }

        public virtual void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
