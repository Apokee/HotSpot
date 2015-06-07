using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;
using UnityEngine;

namespace HotSpot.Configuration.Overlay
{
    internal sealed class SchemeNode : IConfigNode
    {
        private List<GradientNode> _gradientList = new List<GradientNode>();

        public string Name { get; private set; }

        public SchemeNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = node.GetValue("name");

                _gradientList = node
                    .GetNodes("GRADIENT")
                    .Where(i => !i.GetValue("name").EndsWith("Template"))
                    .Select(i => new GradientNode(i))
                    .ToList();
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        public Color? EvaluateColor(double partCurrent, Dictionary<Variable, double> variables)
        {
            return _gradientList
                .Select(i => i.Evaluate(variables))
                .Where(i => i.Min < i.Max)
                .FirstOrDefault(i => i.Min <= partCurrent && partCurrent <= i.Max)
                ?.EvaluateColor(partCurrent);
        }
    }
}
