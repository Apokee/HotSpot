using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;

namespace HotSpot.Configuration.Overlay
{
    internal sealed class MetricNode
    {
        private readonly string _origScheme;

        private readonly Dictionary<string, SchemeNode> _schemesDictionary;

        public Metric Name { get; }
        public string Scheme { get; set; }
        public SchemeNode[] Schemes { get; }

        private MetricNode(Metric name, string scheme, SchemeNode[] schemes)
        {
            _origScheme = scheme;

            Name = name;
            Scheme = scheme;
            Schemes = schemes;
            _schemesDictionary = schemes.ToDictionary(i => i.Name);
        }

        public SchemeNode GetActiveScheme()
        {
            return _schemesDictionary[Scheme];
        }

        public bool Save(ConfigNode node)
        {
            var save = false;

            if (_origScheme != Scheme)
            {
                node.AddValue("%scheme", Scheme);
                save = true;
            }

            return save;
        }

        public static MetricNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var name = Metric.TryParse(node.GetValue("name"));
                var scheme = node.GetValue("scheme");

                var schemes = node
                    .GetNodes("SCHEME")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(SchemeNode.TryParse)
                    .Where(i => i != null)
                    .ToArray();

                if (name != null && scheme != null)
                {
                    return new MetricNode(name, scheme, schemes);
                }
            }

            Log.Warning($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }
    }
}
