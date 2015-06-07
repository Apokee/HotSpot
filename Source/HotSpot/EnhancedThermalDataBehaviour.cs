using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Model;
using UnityEngine;

namespace EnhancedThermalData
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    // ReSharper disable once UnusedMember.Global
    public class EnhancedThermalDataBehaviour : MonoBehaviour
    {
        #region MonoBehaviour

        // ReSharper disable once UnusedMember.Global
        public void LateUpdate()
        {
            Log.Trace("Entering EnhancedThermalDataBehaviour.LateUpdate()");

            if (PhysicsGlobals.ThermalColorsDebug && Config.Instance.Overlay.Enable)
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

                        part.UpdateMaterialColor(color ?? Part.defaultHighlightNone);
                    }
                }
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.LateUpdate()");
        }

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
