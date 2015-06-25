using System;
using HotSpot.Model;

namespace HotSpot.Configuration.ContextMenu
{
    internal sealed class MetricNode
    {
        private readonly bool _origEnable;
        private readonly Unit _origUnit;

        public Metric Name { get; }
        public bool Enable { get; set; }
        public Unit Unit { get; set; }

        private MetricNode(Metric name, bool enable, Unit unit)
        {
            _origEnable = enable;
            _origUnit = unit;

            Name = name;
            Enable = enable;
            Unit = unit;
        }

        public bool Save(ConfigNode node)
        {
            var save = false;

            if (_origEnable != Enable)
            {
                node.AddValue("%enable", Enable);
                save = true;
            }

            if (_origUnit != Unit)
            {
                node.AddValue("%unit", Unit);
                save = true;
            }

            return save;
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
