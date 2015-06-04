using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Configuration;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;
using UnityEngine;
using static EnhancedThermalData.Model.Metric;

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

                    switch (Config.Instance.Overlay.Metric)
                    {
                        case Temperature:
                            break;
                        case ThermalRateInternal:
                            vesselGradientMin = vessel.Parts.Select(i => i.thermalInternalFlux).Min();
                            vesselGradientMax = vessel.Parts.Select(i => i.thermalInternalFlux).Max();
                            break;
                        case ThermalRateConductive:
                            vesselGradientMin = vessel.Parts.Select(i => i.thermalConductionFlux).Min();
                            vesselGradientMax = vessel.Parts.Select(i => i.thermalConductionFlux).Max();
                            break;
                        case ThermalRateConvective:
                            vesselGradientMin = vessel.Parts.Select(i => i.thermalConvectionFlux).Min();
                            vesselGradientMax = vessel.Parts.Select(i => i.thermalConvectionFlux).Max();
                            break;
                        case ThermalRateRadiative:
                            vesselGradientMin = vessel.Parts.Select(i => i.thermalRadiationFlux).Min();
                            vesselGradientMax = vessel.Parts.Select(i => i.thermalRadiationFlux).Max();
                            break;
                        case ThermalRate:
                            vesselGradientMin = vessel.Parts.Select(i => i.GetThermalFlux()).Min();
                            vesselGradientMax = vessel.Parts.Select(i => i.GetThermalFlux()).Max();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (var part in vessel.Parts)
                    {
                        double partGradientMin;
                        double partGradientMax;
                        double gradientValue;

                        switch (Config.Instance.Overlay.Metric)
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
                            case ThermalRateConductive:
                                // ReSharper disable PossibleInvalidOperationException
                                partGradientMin = vesselGradientMin.Value;
                                partGradientMax = vesselGradientMax.Value;
                                // ReSharper restore PossibleInvalidOperationException
                                gradientValue = part.thermalConductionFlux;
                                break;
                            case ThermalRateConvective:
                                // ReSharper disable PossibleInvalidOperationException
                                partGradientMin = vesselGradientMin.Value;
                                partGradientMax = vesselGradientMax.Value;
                                // ReSharper restore PossibleInvalidOperationException
                                gradientValue = part.thermalConvectionFlux;
                                break;
                            case ThermalRateRadiative:
                                // ReSharper disable PossibleInvalidOperationException
                                partGradientMin = vesselGradientMin.Value;
                                partGradientMax = vesselGradientMax.Value;
                                // ReSharper restore PossibleInvalidOperationException
                                gradientValue = part.thermalRadiationFlux;
                                break;
                            case ThermalRate:
                                // ReSharper disable PossibleInvalidOperationException
                                partGradientMin = vesselGradientMin.Value;
                                partGradientMax = vesselGradientMax.Value;
                                // ReSharper restore PossibleInvalidOperationException
                                gradientValue = part.GetThermalFlux();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var gradientName = Config.Instance.Overlay.GetMetric(Config.Instance.Overlay.Metric).Gradient;
                        var gradient = Gradient(partGradientMin, partGradientMax, gradientName);

                        part.UpdateMaterialColor(gradient[gradientValue]);
                    }
                }
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.LateUpdate()");
        }

        #endregion

        #region Helpers

        private AbsoluteGradient Gradient(double minValue, double maxValue, string gradientName)
        {
            Log.Trace("Entering EnhancedThermalDataBehaviour.Gradient()");

            var cacheKey = GradientCacheKey(minValue, maxValue, gradientName);

            AbsoluteGradient gradient;
            if (!_gradientCache.TryGetValue(cacheKey, out gradient))
            {
                gradient = new AbsoluteGradient(minValue, maxValue, Config.Instance.Overlay.GetGradient(gradientName).Stops);

                _gradientCache.Add(cacheKey, gradient);
            }

            Log.Trace("Leaving EnhancedThermalDataBehaviour.Gradient()");

            return gradient;
        }

        private static string GradientCacheKey(double minValue, double maxValue, string gradientName)
        {
            return $"{minValue}:{maxValue}:{gradientName}";
        }

        #endregion
    }
}
