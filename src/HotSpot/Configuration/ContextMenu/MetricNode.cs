using System;
using HotSpot.Model;

namespace HotSpot.Configuration.ContextMenu
{
    internal sealed class MetricNode
    {
        public Metric Name { get; }
        public bool Enable { get; set; }
        public Unit Unit { get; set; }
        public Prefix? Prefix { get; set; }

        private MetricNode(Metric name, bool enable, Unit unit, Prefix? prefix)
        {
            Name = name;
            Enable = enable;
            // TODO: Legacy : Remove Kilowatt
            Unit = unit == Unit.Kilowatt ? Unit.Watt : unit;
            Prefix = prefix;
        }

        public bool Save(ConfigNode node)
        {
            node.AddValue("%enable", Enable);
            node.AddValue("%unit", Unit);
            if (Prefix != null)
                node.AddValue("%prefix", Prefix.Value);
            else
                node.AddValue("%prefix", "auto");

            return true;
        }

        public static MetricNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var name = Metric.TryParse(node.GetValue("name"));
                var enable = node.TryParse<bool>("enable") ?? true;
                var unit = node.TryParse<Unit>("unit");
                Prefix? prefix = null;

                var prefixString = node.GetValue("prefix");
                if (!string.IsNullOrEmpty(prefixString) && prefixString != "auto")
                {
                    prefix = node.TryParse<Prefix>("prefix");
                }

                if (name != null && unit != null)
                {
                    return new MetricNode(name, enable, unit.Value, prefix);
                }
            }

            Log.Warning($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }
    }
}
