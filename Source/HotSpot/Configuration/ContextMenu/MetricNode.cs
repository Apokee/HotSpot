using System;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration.ContextMenu
{
    internal sealed class MetricNode : IConfigNode
    {
        public Metric Name { get; private set; }
        public bool Enable { get; private set; }
        public Unit Unit { get; private set; }

        public MetricNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = Metric.Parse(node.GetValue("name"));
                Enable = node.Parse<bool>("enable");
                Unit = node.Parse<Unit>("unit");
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
