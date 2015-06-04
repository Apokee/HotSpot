using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration.ContextMenu;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration
{
    internal sealed class ContextMenuNode : IConfigNode
    {
        private Dictionary<Metric, MetricNode> _metrics = new Dictionary<Metric, MetricNode>(); 

        public IEnumerable<MetricNode> Metrics => _metrics.Values;

        public MetricNode GetMetric(Metric metric)
        {
            return _metrics[metric];
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                _metrics = node.GetNodes("METRIC").Select(i => new MetricNode(i)).ToDictionary(i => i.Name);
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
