using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration;
using UnityEngine;

namespace EnhancedThermalData.Model
{
    internal sealed class AbsoluteGradient
    {
        private readonly double _maxValue;
        private readonly Gradient _gradient = new Gradient();

        public Color this[double value] => _gradient.Evaluate((float)(value / _maxValue));

        public AbsoluteGradient(double maxValue, List<StopNode> stops)
        {
            _maxValue = maxValue;

            var stopList = stops.ToArray();
            _gradient.SetKeys(GetColorKeys(stopList), GetAlphaKeys(stopList));
        }

        private GradientColorKey[] GetColorKeys(IEnumerable<StopNode> stops)
        {
            return stops
                .Select(i => i.TryConvertToColorKey(_maxValue))
                .Where(i => i != null)
                .Select(i => i.Value)
                .ToArray();
        }

        private GradientAlphaKey[] GetAlphaKeys(IEnumerable<StopNode> stops)
        {
            return stops
                .Select(i => i.TryConvertToAlphaKey(_maxValue))
                .Where(i => i != null)
                .Select(i => i.Value)
                .ToArray();
        }
    }
}
