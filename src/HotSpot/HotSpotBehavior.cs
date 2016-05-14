using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;
using HotSpot.Reflection;
using UnityEngine;

namespace HotSpot
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HotSpotBehavior : MonoBehaviour
    {
        #region MonoBehaviour

        public void LateUpdate()
        {
            Log.Trace("Entering HotSpotBehavior.LateUpdate()");

            if (Config.Instance.Overlay.Enable)
            {
                LateUpdateColor();
            }

            Log.Trace("Leaving HotSpotBehavior.LateUpdate()");
        }

        private static void LateUpdateColor()
        {
            if (PhysicsGlobals.ThermalColorsDebug)
            {
                var metric = Config.Instance.Overlay.Metric;

                foreach (var vessel in FlightGlobals.Vessels.Where(i => i.loaded))
                {
                    var vesselVariables = metric.GetVesselValues(vessel);

                    foreach (var part in vessel.Parts)
                    {
                        var partVariables = metric.GetPartValues(part);
                        var partCurrent = metric.GetPartCurrent(part);

                        var color = Config
                            .Instance
                            .Overlay
                            .GetActiveMetric()
                            .GetActiveScheme()
                            .EvaluateColor(partCurrent, MergeVariables(vesselVariables, partVariables));

                        part.TryGetMaterialColorUpdater()?.Update(color ?? Part.defaultHighlightNone);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private static Dictionary<Variable, double> MergeVariables(
            Dictionary<Variable, double> vesselVariables, Dictionary<Variable, double> partVariables
        )
        {
            var result = new Dictionary<Variable, double>();

            foreach(var kv in vesselVariables)
            {
                result[kv.Key] = kv.Value;
            }

            foreach (var kv in partVariables)
            {
                result[kv.Key] = kv.Value;
            }

            return result;
        }

        #endregion
    }
}
