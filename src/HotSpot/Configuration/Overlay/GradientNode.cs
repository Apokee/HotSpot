using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;

namespace HotSpot.Configuration.Overlay
{
    internal sealed class GradientNode
    {
        public string Name { get; }
        public Expression Min { get; }
        public Expression Max { get; }
        public OnConflict OnConflict { get; }
        public StopNode[] Stops { get; }

        private GradientNode(string name, Expression min, Expression max, OnConflict onConflict, StopNode[] stops)
        {
            Name = name;
            Min = min;
            Max = max;
            OnConflict = onConflict;
            Stops = stops;
        }

        public EvaluatedGradientNode Evaluate(Dictionary<Variable, double> variables)
        {
            return new EvaluatedGradientNode(this, variables);
        }

        public static GradientNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var name = node.GetValue("name");
                var min = Expression.TryParse(node.GetValue("min"));
                var max = Expression.TryParse(node.GetValue("max"));
                var onConflict = node.TryParse<OnConflict>("onConflict") ?? OnConflict.Ignore;
                var stops = node
                    .GetNodes("STOP")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(StopNode.TryParse)
                    .Where(i => i != null)
                    .ToArray();

                if (name != null && min != null && max != null)
                {
                    return new GradientNode(name, min, max, onConflict, stops);
                }
            }

            Log.Debug($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }
    }
}
