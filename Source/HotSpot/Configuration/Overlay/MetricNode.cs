using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;

namespace HotSpot.Configuration.Overlay
{
    internal sealed class MetricNode : IConfigNode
    {
        private Dictionary<string, SchemeNode> _schemes = new Dictionary<string, SchemeNode>();

        public Metric Name { get; private set; }
        public string Scheme { get; private set; }

        public MetricNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = Metric.Parse(node.GetValue("name"));
                Scheme = node.GetValue("scheme");

                _schemes = node
                    .GetNodes("SCHEME")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(i => new SchemeNode(i))
                    .ToDictionary(i => i.Name);
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        public SchemeNode GetActiveScheme()
        {
            return _schemes[Scheme];
        }
    }
}
