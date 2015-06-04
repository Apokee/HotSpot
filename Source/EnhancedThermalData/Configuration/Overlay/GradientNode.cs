using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration.Overlay.Gradient;

namespace EnhancedThermalData.Configuration.Overlay
{
    internal sealed class GradientNode : IConfigNode
    {
        public string Name { get; private set; }
        public List<StopNode> Stops { get; } = new List<StopNode>();

        public GradientNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = node.GetValue("name");

                Stops.AddRange(node.GetNodes("STOP").Select(i =>
                {
                    var stopNode = new StopNode();
                    stopNode.Load(i);

                    return stopNode;
                }));
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
