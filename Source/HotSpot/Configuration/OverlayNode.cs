using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Configuration.Overlay;
using HotSpot.Model;

namespace HotSpot.Configuration
{
    internal sealed class OverlayNode
    {
        private readonly Dictionary<string, MetricNode> _metricsDictionary;

        private readonly string _origMetric;

        public bool Enable { get; }
        public bool EnableScreenMessage { get; }
        public Metric Metric { get; set; }
        public MetricNode[] Metrics { get; }

        private OverlayNode(bool enable, bool enableScreenMessage, Metric metric, MetricNode[] metrics)
        {
            _origMetric = metric.Name;

            Enable = enable;
            EnableScreenMessage = enableScreenMessage;
            Metric = metric;
            Metrics = metrics;
            _metricsDictionary = metrics.ToDictionary(i => i.Name.Name);
        }

        public MetricNode GetActiveMetric()
        {
            return _metricsDictionary[Metric.Name];
        }

        public bool Save(ConfigNode node)
        {
            var save = false;

            if (_origMetric != Metric.Name)
            {
                node.AddValue("%metric", Metric.Name);
                save = true;
            }

            foreach (var metric in Metrics)
            {
                var metricNode = new ConfigNode($"%METRIC[{metric.Name.Name}]");

                if (metric.Save(metricNode))
                {
                    node.AddNode(metricNode);
                    save = true;
                }
            }

            return save;
        }

        public static OverlayNode GetDefault()
        {
            return new OverlayNode(false, true, Metric.TemperatureInternal, new MetricNode[] { });
        }

        public static OverlayNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var enable = node.TryParse<bool>("enable") ?? true;
                var enableScrenMessage = node.TryParse<bool>("enableScreenMessage") ?? true;
                var metric = Metric.TryParse(node.GetValue("metric"));
                var metrics = node
                    .GetNodes("METRIC")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(MetricNode.TryParse)
                    .Where(i => i != null)
                    .ToArray();

                if (metric != null)
                {
                    return new OverlayNode(enable, enableScrenMessage, metric, metrics);
                }
            }

            Log.Warning($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }
    }
}
