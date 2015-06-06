using System.Collections.Generic;
using EnhancedThermalData.Configuration.Overlay;
using UnityEngine;

namespace EnhancedThermalData.Model
{
    internal sealed class EvaluatedStopNode
    {
        private readonly StopNode _stop;

        public string Name => _stop.Name;
        public double Value { get; }
        public Color? Color => _stop.Color;
        public float? Alpha => _stop.Alpha;

        public EvaluatedStopNode(StopNode stop, Dictionary<Variable, double> variables)
        {
            _stop = stop;
            Value = stop.Value.Evaluate(variables) * stop.Factor;
        }

        public GradientColorKey? TryConvertToColorKey(double minValue, double maxValue)
        {
            if (Color != null)
            {
                return new GradientColorKey(Color.Value,
                    (float)((Value - minValue) / (maxValue - minValue))
                );
            }

            return null;
        }

        public GradientAlphaKey? TryConvertToAlphaKey(double minValue, double maxValue)
        {
            if (Alpha != null)
            {
                return new GradientAlphaKey(Alpha.Value,
                    (float)((Value - minValue) / (maxValue - minValue))
                );
            }

            return null;
        }
    }
}
