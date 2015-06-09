using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;
using UnityEngine;

namespace HotSpot.Configuration.Overlay
{
    internal sealed class SchemeNode
    {
        private readonly GradientNode[] _gradients; 

        public string Name { get; }
        public string FriendlyName { get; }

        private SchemeNode(string name, string friendlyName, GradientNode[] gradients)
        {
            Name = name;
            FriendlyName = friendlyName;
            _gradients = gradients;
        }

        public Color? EvaluateColor(double partCurrent, Dictionary<Variable, double> variables)
        {
            return _gradients
                .Select(i => i.Evaluate(variables))
                .Where(i => i.Min < i.Max)
                .FirstOrDefault(i => i.Min <= partCurrent && partCurrent <= i.Max)
                ?.EvaluateColor(partCurrent);
        }

        public static SchemeNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var name = node.GetValue("name");
                var friendlyName = node.GetValue("friendlyName");
                var gradients = node
                    .GetNodes("GRADIENT")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(GradientNode.TryParse)
                    .Where(i => i != null)
                    .ToArray();

                if (name != null)
                {
                    return new SchemeNode(name, friendlyName, gradients);
                }
            }

            Log.Warning($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }
    }
}
