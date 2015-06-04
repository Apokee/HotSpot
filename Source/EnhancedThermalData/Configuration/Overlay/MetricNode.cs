using System;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration.Overlay
{
    internal sealed class MetricNode : IConfigNode
    {
        public Metric Name { get; private set; }
        public string Gradient { get; private set; }

        public MetricNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = node.Parse<Metric>("name");
                Gradient = node.GetValue("gradient");
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
