using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;
using UnityEngine;

namespace EnhancedThermalData
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    // ReSharper disable once UnusedMember.Global
    public class EnhancedThermalDataBehaviour : MonoBehaviour
    {
        private readonly Dictionary<double, AbsoluteGradient> _gradientCache =
            new Dictionary<double, AbsoluteGradient>();

        #region MonoBehaviour

        // ReSharper disable once UnusedMember.Global
        public void LateUpdate()
        {
            Log.Trace("Entering EnhancedThermalDataBehaviour.LateUpdate()");

            if (PhysicsGlobals.ThermalColorsDebug && Config.Instance.ThermalOverlay.Enable)
            {
                foreach (var part in FlightGlobals.Vessels.SelectMany(i => i.Parts))
                {
                    part.UpdateMaterialColor(Gradient(0, part.maxTemp)[part.temperature]);
                }
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.LateUpdate()");
        }

        #endregion

        #region Helpers

        private AbsoluteGradient Gradient(double minValue, double maxValue)
        {
            Log.Trace("Entering EnhancedThermalDataBehaviour.Gradient()");

            AbsoluteGradient gradient;
            if (!_gradientCache.TryGetValue(maxValue, out gradient))
            {
                gradient = new AbsoluteGradient(minValue, maxValue,
                    Config.Instance.ThermalOverlay.Gradient.Stops
                );

                _gradientCache.Add(maxValue, gradient);
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.Gradient()");

            return gradient;
        }

        #endregion
    }
}
