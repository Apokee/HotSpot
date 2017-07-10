using HotSpot.Model;
using HotSpot.Reflection;
using UnityEngine;

namespace HotSpot
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HotSpotBehavior : MonoBehaviour
    {
        private readonly VariableBag _variables = new VariableBag();

        #region MonoBehaviour

        public void LateUpdate()
        {
            Log.Trace("Entering HotSpotBehavior.LateUpdate()");

            if (ShouldUpdate())
            {
                _variables.ResetAll();

                var metric = Config.Instance.Overlay.Metric;
                var scheme = Config.Instance.Overlay.GetActiveMetric().GetActiveScheme();

                var vessels = FlightGlobals.Vessels;
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var iv = 0; iv < vessels.Count; iv++)
                {
                    var vessel = vessels[iv];
                    if (!vessel.loaded) continue;

                    _variables.ResetVesselSpecific();
                    metric.PopulateVesselVariables(vessel, _variables);

                    var parts = vessel.Parts;
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var ip = 0; ip < parts.Count; ip++)
                    {
                        var part = parts[ip];

                        Color? color = null;

                        if (metric.IsApplicable(part))
                        {
                            _variables.ResetPartSpecific();
                            metric.PopulatePartVariables(part, _variables);

                            color = scheme.EvaluateColor(_variables);
                        }

                        part.TryGetMaterialColorUpdater()?.Update(color ?? Part.defaultHighlightNone);
                    }
                }
            }

            Log.Trace("Leaving HotSpotBehavior.LateUpdate()");
        }

        private static bool ShouldUpdate()
        {
            return Config.Instance.Overlay.Enable
                && PhysicsGlobals.ThermalColorsDebug;
        }

        #endregion
    }
}
