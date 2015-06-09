using HotSpot.Reflection;
using UnityEngine;

namespace HotSpot
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public sealed class HotSpotGuiBehavior : MonoBehaviour
    {
        private bool _lastThermalColorsDebug;
        private readonly ScreenMessage _screenMessage =
            new ScreenMessage(string.Empty, 4, ScreenMessageStyle.LOWER_CENTER);

        #region MonoBehaiour

        public void LateUpdate()
        {
            Log.Trace("Entering HotSpotGuiBehavior.LateUpdate()");

            if (Config.Instance.Overlay.Enable)
            {
                LateUpdateScreenMessage();
            }

            Log.Trace("Leaving HotSpotGuiBehavior.LateUpdate()");
        }

        private void LateUpdateScreenMessage()
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

                    var metricMsg = Config.Instance.Overlay.Metric.FriendlyName;
                    var schemMsg = scheme.FriendlyName == null ? string.Empty : $" ({scheme.FriendlyName})";
                    var stateMsg = PhysicsGlobals.ThermalColorsDebug ? "Enabled" : "Disabled";

                    ScreenMessages.PostScreenMessage($"{metricMsg}{schemMsg} Overlay: {stateMsg}", _screenMessage);
                }
            }

            _lastThermalColorsDebug = PhysicsGlobals.ThermalColorsDebug;
        }

        #endregion
    }
}
