using System;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedThermalData.Configuration
{
    internal sealed class GradientNode : IConfigNode
    {
        public List<StopNode> Stops { get; } = new List<StopNode>();

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
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
