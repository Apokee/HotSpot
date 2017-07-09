using System;
using System.Collections.Generic;
using HotSpot.Configuration.Overlay;
using UnityEngine;

namespace HotSpot.Model
{
    internal sealed class EvaluatedGradientNode
    {
        private readonly GradientNode _gradient;
        private readonly Gradient _unityGradient;

        public string Name => _gradient.Name;
        public double Min { get; }
        public double Max { get; }
        public OnConflict OnConflict => _gradient.OnConflict;

        public List<EvaluatedStopNode> Stops { get; }

        public EvaluatedGradientNode(GradientNode gradient, VariableBag variables)
        {
            _gradient = gradient;
            Min = gradient.Min.Evaluate(variables);
            Max = gradient.Max.Evaluate(variables);

            variables.GradientMinimum = Min;
            variables.GradientMaximum = Max;

            var evaluatedStops = new List<EvaluatedStopNode>();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < gradient.Stops.Length; i++)
                evaluatedStops.Add(gradient.Stops[i].Evaluate(variables));

            Stops = FilterStopsForConflicts(evaluatedStops);

            var colorKeys = new List<GradientColorKey>();
            var alphaKeys = new List<GradientAlphaKey>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Stops.Count; i++)
            {
                var stop = Stops[i];

                var colorKey = stop.TryConvertToColorKey(Min, Max);
                var alphaKey = stop.TryConvertToAlphaKey(Min, Max);

                if (colorKey != null)
                    colorKeys.Add(colorKey.Value);

                if (alphaKey != null)
                    alphaKeys.Add(alphaKey.Value);
            }

            _unityGradient = new Gradient();
            _unityGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
        }

        public Color EvaluateColor(double value)
        {
            return _unityGradient.Evaluate((float)((value - Min) / (Max - Min)));
        }

        private List<EvaluatedStopNode> FilterStopsForConflicts(List<EvaluatedStopNode> stops)
        {
            if (OnConflict == OnConflict.Ignore)
            {
                return stops;
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

                for (var i = 0; i < stops.Count; i++)
                {
                    var previous = i > 0 ? stops[i - 1] : null;
                    var current = stops[i];
                    var next = i < stops.Count - 1 ? stops[i + 1] : null;

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

                if (filteredStops.Count == stops.Count)
                {
                    return filteredStops;
                }

                stops = filteredStops;
            }
        }
    }
}
