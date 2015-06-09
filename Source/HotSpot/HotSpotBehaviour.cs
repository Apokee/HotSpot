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
        private ScreenMessage _screenMessage = new ScreenMessage(string.Empty, 4, ScreenMessageStyle.LOWER_CENTER);

        #region MonoBehaviour

        // ReSharper disable once UnusedMember.Global
        public void LateUpdate()
        {
            Log.Trace("Entering HotSpotBehaviour.LateUpdate()");

            if (Config.Instance.Overlay.Enable)
            {
                var metric = Config.Instance.Overlay.Metric;

                LateUpdateScreenMessage(metric);
                LateUpdateColor(metric);
            }

            Log.Trace("Leaving HotSpotBehaviour.LateUpdate()");
        }

        #endregion

        private void LateUpdateScreenMessage(Metric metric)
        {
            if (_lastThermalColorsDebug != PhysicsGlobals.ThermalColorsDebug)
            {
                Log.Debug("PhysicsGlobals.ThermalColorsDebug has been toggled");

                var flightOverlays = FindObjectOfType<FlightOverlays>();

                if (flightOverlays != null)
                {
                    Log.Debug("Found FlightOverlays, removing its ScreenMessage");

                    ScreenMessages.RemoveMessage(flightOverlays.TryGetScreenMessage());
                }

                if (Config.Instance.Overlay.EnableScreenMessage)
                {
                    var scheme = Config.Instance.Overlay.GetActiveMetric().GetActiveScheme();

                    var metricMsg = metric.FriendlyName;
                    var schemMsg = scheme.FriendlyName == null ? string.Empty : $" ({scheme.FriendlyName})";
                    var stateMsg = PhysicsGlobals.ThermalColorsDebug ? "Enabled" : "Disabled";

                    ScreenMessages.PostScreenMessage($"{metricMsg}{schemMsg} Overlay: {stateMsg}", _screenMessage);
                }
            }

            _lastThermalColorsDebug = PhysicsGlobals.ThermalColorsDebug;
        }

        private static void LateUpdateColor(Metric metric)
        {
            if (PhysicsGlobals.ThermalColorsDebug)
            {
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
