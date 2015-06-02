using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration.Overlay;
using EnhancedThermalData.Extensions;

namespace EnhancedThermalData.Configuration
{
    internal sealed class OverlayNode : IConfigNode
    {
        private readonly Dictionary<Metric, MetricNode> _metrics = new Dictionary<Metric, MetricNode>();
        private readonly Dictionary<string, GradientNode> _gradients = new Dictionary<string, GradientNode>();

        public bool Enable { get; private set; } = true;
        public Metric Mode { get; private set; } = Metric.Temperature;

        public IEnumerable<MetricNode> Metrics => _metrics.Values;
        public IEnumerable<GradientNode> Gradients => _gradients.Values;

        public MetricNode GetMetric(Metric name)
        {
            return _metrics[name];
        }

        public GradientNode GetGradient(string name)
        {
            return _gradients[name];
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Enable = node.Parse<bool>("enable");
                Mode = node.Parse<Metric>("mode");

                foreach (var metric in node.GetNodes("METRIC").Select(i => new MetricNode(i)))
                {
                    _metrics[metric.Name] = metric;
                }

                foreach (var gradient in node.GetNodes("GRADIENT").Select(i => new GradientNode(i)))
                {
                    _gradients[gradient.Name] = gradient;
                }
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
