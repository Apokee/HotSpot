using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Configuration.ContextMenu;
using HotSpot.Model;

namespace HotSpot.Configuration
{
    internal sealed class ContextMenuNode : IConfigNode
    {
        private Dictionary<string, MetricNode> _metricDictionary = new Dictionary<string, MetricNode>(); 

        public MetricNode GetMetric(Metric metric)
        {
            return _metricDictionary[metric.Name];
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                _metricDictionary = node
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
    }
}
