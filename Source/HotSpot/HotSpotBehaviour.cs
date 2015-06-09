using System.Collections.Generic;
using System.Linq;
using HotSpot.Model;
using HotSpot.Reflection;
using UnityEngine;

namespace HotSpot
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    // ReSharper disable once UnusedMember.Global
    public class HotSpotBehaviour : MonoBehaviour
    {
        private bool _lastThermalColorsDebug;

        #region MonoBehaviour

        // ReSharper disable once UnusedMember.Global
        public void LateUpdate()
        {
            Log.Trace("Entering HotSpotBehaviour.LateUpdate()");

            if (Config.Instance.Overlay.Enable)
            {
                if (_lastThermalColorsDebug != PhysicsGlobals.ThermalColorsDebug)
                {
                    Log.Debug("PhysicsGlobals.ThermalColorsDebug has been toggled");

                    var flightOverlays = FindObjectOfType<FlightOverlays>();

                    if (flightOverlays)
                    {
                        Log.Debug("Found FlightOverlays, removing its ScreenMessage");

                        ScreenMessages.RemoveMessage(flightOverlays.TryGetScreenMessage());
                    }
                }

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

            _lastThermalColorsDebug = PhysicsGlobals.ThermalColorsDebug;

            Log.Trace("Leaving HotSpotBehaviour.LateUpdate()");
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
