using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration;
using UnityEngine;

namespace EnhancedThermalData.Model
{
    internal sealed class AbsoluteGradient
    {
        private readonly double _minValue;
        private readonly double _maxValue;
        private readonly Gradient _gradient = new Gradient();

        public Color this[double value] => _gradient.Evaluate((float)((value - _minValue) / (_maxValue - _minValue)));

        public AbsoluteGradient(double minValue, double maxValue, List<GradientNode.StopNode> stops)
        {
            _minValue = minValue;
            _maxValue = maxValue;

            var stopList = stops.ToArray();
            _gradient.SetKeys(GetColorKeys(stopList), GetAlphaKeys(stopList));
        }

        private GradientColorKey[] GetColorKeys(IEnumerable<GradientNode.StopNode> stops)
        {
            return stops
                .Select(i => i.TryConvertToColorKey(_minValue, _maxValue))
                .Where(i => i != null)
                .Select(i => i.Value)
                .ToArray();
        }

        private GradientAlphaKey[] GetAlphaKeys(IEnumerable<GradientNode.StopNode> stops)
        {
            return stops
                .Select(i => i.TryConvertToAlphaKey(_minValue, _maxValue))
                .Where(i => i != null)
                .Select(i => i.Value)
                .ToArray();
        }
    }
}
