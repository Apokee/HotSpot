using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Configuration.Overlay;
using UnityEngine;

namespace HotSpot.Model
{
    internal sealed class EvaluatedGradientNode
    {
        private readonly GradientNode _gradient;
        private Gradient _unityGradient;

        public string Name => _gradient.Name;
        public double Min { get; }
        public double Max { get; }
        public OnConflict OnConflict => _gradient.OnConflict;

        public IEnumerable<EvaluatedStopNode> Stops { get; }

        public EvaluatedGradientNode(GradientNode gradient, Dictionary<Variable, double> variables)
        {
            _gradient = gradient;
            Min = gradient.Min.Evaluate(variables);
            Max = gradient.Max.Evaluate(variables);
            Stops = FilterStopsForConflicts(gradient.Stops.Select(i => i.Evaluate(CalculateVariables(variables))))
                .ToList();
        }

        public Color EvaluateColor(double value)
        {
            if (_unityGradient == null)
            {
                var colorKeys = Stops
                    .Select(i => i.TryConvertToColorKey(Min, Max))
                    .Where(i => i != null)
                    .Select(i => i.Value)
                    .ToArray();

                var alphaKeys = Stops
                    .Select(i => i.TryConvertToAlphaKey(Min, Max))
                    .Where(i => i != null)
                    .Select(i => i.Value)
                    .ToArray();

                _unityGradient = new Gradient();
                _unityGradient.SetKeys(colorKeys, alphaKeys);
            }

            return _unityGradient.Evaluate((float)((value - Min) / (Max - Min)));
        }

        private Dictionary<Variable, double> CalculateVariables(Dictionary<Variable, double> variables)
        {
            var result = new Dictionary<Variable, double>();

            foreach (var variable in variables)
            {
                result[variable.Key] = variable.Value;
            }

            result[Variable.GradientMinimum] = Min;
            result[Variable.GradientMaximum] = Max;

            return result;
        }

        private IEnumerable<EvaluatedStopNode> FilterStopsForConflicts(IEnumerable<EvaluatedStopNode> stops)
        {
            var stopList = stops.ToList();

            if (OnConflict == OnConflict.Ignore)
            {
                return stopList;
            }

            var iterationCheck = 0;

            while (true)
            {
                if (iterationCheck++ > 100)
                {
                    Log.Error("Runaway iteration in EvalautedGradientNode.FilterStopsForConflicts");
                    throw new Exception("Runaway iteration in FilterStopsForConflicts");
                }

                var filteredStops = new List<EvaluatedStopNode>();

                for (var i = 0; i < stopList.Count; i++)
                {
                    var previous = i > 0 ? stopList[i - 1] : null;
                    var current = stopList[i];
                    var next = i < stopList.Count - 1 ? stopList[i + 1] : null;

                    if (previous == null && next == null)
                    {
                        filteredStops.Add(current);
                    }
                    else if (previous == null)
                    {
                        if (current.Value < next.Value || OnConflict == OnConflict.RemoveLater)
                        {
                            filteredStops.Add(current);
                        }
                    }
                    else if (next == null)
                    {
                        if (previous.Value < current.Value || OnConflict == OnConflict.RemoveEarlier)
                        {
                            filteredStops.Add(current);
                        }
                    }
                    else
                    {
                        var noConflictWithNext = current.Value < next.Value
                            || OnConflict == OnConflict.RemoveLater;

                        var noConflictWithPrevious = previous.Value < current.Value
                            || OnConflict == OnConflict.RemoveEarlier;

                        if (noConflictWithNext && noConflictWithPrevious)
                        {
                            filteredStops.Add(current);
                        }
                    }
                }

                if (filteredStops.Count == stopList.Count)
                {
                    return filteredStops;
                }

                stopList = filteredStops;
            }
        }
    }
}
