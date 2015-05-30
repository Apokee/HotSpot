using System;

namespace EnhancedThermalData.Configuration
{
    internal sealed class ContextMenuNode : IConfigNode
    {
        public TemperatureNode Temperature { get; } = new TemperatureNode();

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Temperature.Load(node.GetNode("TEMPERATURE"));
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}