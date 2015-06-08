using System;
using HotSpot.Model;

namespace HotSpot.Configuration.ContextMenu
{
    internal sealed class MetricNode
    {
        public Metric Name { get; }
        public bool Enable { get; }
        public Unit Unit { get; }

        private MetricNode(Metric name, bool enable, Unit unit)
        {
            Name = name;
            Enable = enable;
            Unit = unit;
        }

        public static MetricNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var name = Metric.TryParse(node.GetValue("name"));
                var enable = node.TryParse<bool>("enable") ?? true;
                var unit = node.TryParse<Unit>("unit");

                if (name != null && unit != null)
                {
                    return new MetricNode(name, enable, unit.Value);
                }
            }

            Log.Warning($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }
    }
}
