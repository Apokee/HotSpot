using System.Collections.Generic;
using System.Linq;
using HotSpot.Configuration.ContextMenu;
using HotSpot.Model;

namespace HotSpot.Configuration
{
    internal sealed class ContextMenuNode
    {
        private readonly Dictionary<string, MetricNode> _metricsDictionary;
        
        public MetricNode[] Metrics { get; } 

        private ContextMenuNode(MetricNode[] metrics)
        {
            Metrics = metrics;
            _metricsDictionary = metrics.ToDictionary(i => i.Name.Name);
        }

        public MetricNode GetMetric(Metric metric)
        {
            return _metricsDictionary[metric.Name];
        }

        public void Save(ConfigNode node)
        {
            foreach (var metric in Metrics)
            {
                var metricNode = new ConfigNode("METRIC");

                metric.Save(metricNode);

                node.AddNode(metricNode);
            }
        }

        public static ContextMenuNode GetDefault()
        {
            return new ContextMenuNode(new MetricNode[] { });
        }

        public static ContextMenuNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var metrics = node
                    .GetNodes("METRIC")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(MetricNode.TryParse)
                    .Where(i => i != null)
                    .ToArray();

                return new ContextMenuNode(metrics);
            }

            return null;
        }
    }
}
