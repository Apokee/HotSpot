using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration.Overlay;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration
{
    internal sealed class OverlayNode : IConfigNode
    {
        private Dictionary<string, MetricNode> _metrics = new Dictionary<string, MetricNode>();

        public bool Enable { get; private set; } = true;
        public Metric Metric { get; private set; }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Enable = node.Parse<bool>("enable");
                Metric = Metric.Parse(node.GetValue("metric"));

                _metrics = node
                    .GetNodes("METRIC")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(i => new MetricNode(i))
                    .ToDictionary(i => i.Name.Name);
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        public MetricNode GetActiveMetric()
        {
            return _metrics[Metric.Name];
        }
    }
}
