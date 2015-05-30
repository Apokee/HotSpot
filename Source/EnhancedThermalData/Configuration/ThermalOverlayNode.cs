using System;
using EnhancedThermalData.Extensions;

namespace EnhancedThermalData.Configuration
{
    internal sealed class ThermalOverlayNode : IConfigNode
    {
        public bool Enable { get; private set; } = true;

        public GradientNode Gradient { get; } = new GradientNode();

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Enable = node.Parse<bool>("enable");

                Gradient.Load(node.GetNode("GRADIENT"));
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
