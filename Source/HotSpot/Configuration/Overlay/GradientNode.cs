using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration.Overlay
{
    internal class GradientNode : IConfigNode
    {
        private List<StopNode> _stopList = new List<StopNode>();

        public string Name { get; private set; }
        public Expression Min { get; private set; }
        public Expression Max { get; private set; }
        public OnConflict OnConflict { get; private set; }

        public IEnumerable<StopNode> Stops => _stopList;

        public GradientNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = node.GetValue("name");
                Min = Expression.Parse(node.GetValue("min"));
                Max = Expression.Parse(node.GetValue("max"));
                OnConflict = node.Parse<OnConflict>("onConflict");

                _stopList = node
                    .GetNodes("STOP")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(i => new StopNode(i))
                    .ToList();
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        public EvaluatedGradientNode Evaluate(Dictionary<Variable, double> variables)
        {
            return new EvaluatedGradientNode(this, variables);
        }
    }
}
