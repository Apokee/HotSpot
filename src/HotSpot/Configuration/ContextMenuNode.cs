﻿using System.Collections.Generic;
using System.Linq;
using HotSpot.Configuration.ContextMenu;
using HotSpot.Model;

namespace HotSpot.Configuration
{
    internal sealed class ContextMenuNode
    {
        private const double DefaultUpdatePeriod = 0.250;
        private readonly Dictionary<string, MetricNode> _metricsDictionary;
        
        public double UpdatePeriod { get; }
        public MetricNode[] Metrics { get; }

        private ContextMenuNode(double updatePeriod, MetricNode[] metrics)
        {
            UpdatePeriod = updatePeriod;
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
            return new ContextMenuNode(DefaultUpdatePeriod, new MetricNode[] { });
        }

        public static ContextMenuNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var updatePeriod = node.TryParse<double>("updatePeriod") ?? DefaultUpdatePeriod;
                var metrics = node
                    .GetNodes("METRIC")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(MetricNode.TryParse)
                    .Where(i => i != null)
                    .ToArray();

                return new ContextMenuNode(updatePeriod, metrics);
            }

            return null;
        }
    }
}
