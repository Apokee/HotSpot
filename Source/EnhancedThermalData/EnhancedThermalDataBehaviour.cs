using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;
using UnityEngine;
using static EnhancedThermalData.Configuration.Config.OverlayNode.OverlayMode;

namespace EnhancedThermalData
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    // ReSharper disable once UnusedMember.Global
    public class EnhancedThermalDataBehaviour : MonoBehaviour
    {
        private readonly Dictionary<string, AbsoluteGradient> _gradientCache =
            new Dictionary<string, AbsoluteGradient>();

        #region MonoBehaviour

        // ReSharper disable once UnusedMember.Global
        public void LateUpdate()
        {
            Log.Trace("Entering EnhancedThermalDataBehaviour.LateUpdate()");

            if (PhysicsGlobals.ThermalColorsDebug && Config.Instance.Overlay.Enable)
            {
                foreach (var vessel in FlightGlobals.Vessels.Where(i => i.loaded))
                {
                    double? vesselGradientMin = null;
                    double? vesselGradientMax = null;

                    switch (Config.Instance.Overlay.Mode)
                    {
                        case Temperature:
                            break;
                        case ThermalRateInternal:
                            vesselGradientMin = vessel.Parts.Select(i => i.thermalInternalFlux).Min();
                            vesselGradientMax = vessel.Parts.Select(i => i.thermalInternalFlux).Max();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (var part in vessel.Parts)
                    {
                        double partGradientMin;
                        double partGradientMax;
                        double gradientValue;

                        switch (Config.Instance.Overlay.Mode)
                        {
                            case Temperature:
                                partGradientMin = 0;
                                partGradientMax = part.maxTemp;
                                gradientValue = part.temperature;
                                break;
                            case ThermalRateInternal:
                                // ReSharper disable PossibleInvalidOperationException
                                partGradientMin = vesselGradientMin.Value;
                                partGradientMax = vesselGradientMax.Value;
                                // ReSharper restore PossibleInvalidOperationException
                                gradientValue = part.thermalInternalFlux;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var gradient = Gradient(partGradientMin, partGradientMax, Config.Instance.Overlay.Mode);
                        part.UpdateMaterialColor(gradient[gradientValue]);
                    }
                }
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.LateUpdate()");
        }

        #endregion

        #region Helpers

        private AbsoluteGradient Gradient(double minValue, double maxValue, Config.OverlayNode.OverlayMode mode)
        {
            Log.Trace("Entering EnhancedThermalDataBehaviour.Gradient()");

            var cacheKey = GradientCacheKey(minValue, maxValue, mode);

            AbsoluteGradient gradient;
            if (!_gradientCache.TryGetValue(cacheKey, out gradient))
            {
                gradient = new AbsoluteGradient(minValue, maxValue, Config.Instance.Overlay[mode].Gradient.Stops);

                _gradientCache.Add(cacheKey, gradient);
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.Gradient()");

            return gradient;
        }

        private string GradientCacheKey(double minValue, double maxValue, Config.OverlayNode.OverlayMode mode)
        {
            return $"{minValue}:{maxValue}:{mode}";
        }

        #endregion
    }
}
