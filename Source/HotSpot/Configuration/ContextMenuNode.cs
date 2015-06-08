using System.Collections.Generic;
using System.Linq;
using HotSpot.Configuration.ContextMenu;
using HotSpot.Model;

namespace HotSpot.Configuration
{
    internal sealed class ContextMenuNode
    {
        private readonly Dictionary<string, MetricNode> _metrics; 

        private ContextMenuNode(MetricNode[] metrics)
        {
            _metrics = metrics.ToDictionary(i => i.Name.Name);
        }

        public MetricNode GetMetric(Metric metric)
        {
            return _metrics[metric.Name];
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

            Log.Warning("Could not parse missing CONTEXT_MENU node");
            return null;
        }
    }
}
