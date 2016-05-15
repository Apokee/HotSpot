using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotSpot.Model
{
    internal sealed class Metric
    {
        public static readonly Metric TemperatureInternal = new Metric("TemperatureInternal",
            "Temp [I]",
            "Internal Temperature",
            new[] { Unit.Kelvin, Unit.Celsius, Unit.Rankine, Unit.Fahrenheit },
            false,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.temperature),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.temperature),
                [Variable.VesselAbsoluteMinimum] = 0,
                [Variable.VesselAbsoluteMaximum] = vessel.Parts.Max(i => i.maxTemp)
            },
            part => new Dictionary<Variable, double>
            {
                [Variable.PartAbsoluteMinimum] = 0,
                [Variable.PartAbsoluteMaximum] = part.maxTemp
            },
            part => part.temperature,
            (part, unit, prefix) => TemperatureToString(part.temperature, part.maxTemp, null, unit)
        );

        public static readonly Metric TemperatureSkin = new Metric("TemperatureSkin",
            "Temp [S]",
            "Skin Temperature",
            new[] { Unit.Kelvin, Unit.Celsius, Unit.Rankine, Unit.Fahrenheit },
            false,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.skinTemperature),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.skinTemperature),
                [Variable.VesselAbsoluteMinimum] = 0,
                [Variable.VesselAbsoluteMaximum] = vessel.Parts.Max(i => i.skinMaxTemp)
            },
            part => new Dictionary<Variable, double>
            {
                [Variable.PartAbsoluteMinimum] = 0,
                [Variable.PartAbsoluteMaximum] = part.skinMaxTemp
            },
            part => part.skinTemperature,
            (part, unit, prefix) => TemperatureToString(part.skinTemperature, part.skinMaxTemp, null, unit)
        );

        public static readonly Metric TemperatureCore = new Metric("TemperatureCore",
            "Temp [C]",
            "Core Temperature",
            new[] { Unit.Kelvin, Unit.Celsius, Unit.Rankine, Unit.Fahrenheit },
            false,
            part => part.FindModuleImplementing<ModuleCoreHeat>() != null,
            vessel =>
            {
                var coreHeatModules = vessel
                    .Parts
                    .Select(i => i.FindModuleImplementing<ModuleCoreHeat>())
                    .Where(i => i != null)
                    .ToArray();

                if (coreHeatModules.Any())
                {
                    return new Dictionary<Variable, double>
                    {
                        [Variable.VesselCurrentMinimum] = coreHeatModules.Min(i => i.CoreTemperature),
                        [Variable.VesselCurrentMaximum] = coreHeatModules.Max(i => i.CoreTemperature),
                        [Variable.VesselAbsoluteMinimum] = 0,
                        [Variable.VesselAbsoluteMaximum] = coreHeatModules.Max(i => i.CoreShutdownTemp)
                    };
                }
                else
                {
                    return null;
                }
            },
            part =>
            {
                var coreHeatModule = part.FindModuleImplementing<ModuleCoreHeat>();

                if (coreHeatModule != null)
                {
                    return new Dictionary<Variable, double>
                    {
                        [Variable.PartAbsoluteMinimum] = 0,
                        [Variable.PartAbsoluteMaximum] = coreHeatModule.CoreShutdownTemp,
                        [Variable.PartIdeal] = coreHeatModule.CoreTempGoal
                    };
                }
                else
                {
                    return null;
                }
            },
            part => part.FindModuleImplementing<ModuleCoreHeat>()?.CoreTemperature,
            (part, unit, prefix) =>
            {
                var coreHeatModule = part.FindModuleImplementing<ModuleCoreHeat>();

                return coreHeatModule != null
                    ? TemperatureToString(
                        coreHeatModule.CoreTemperature,
                        coreHeatModule.CoreShutdownTemp,
                        coreHeatModule.CoreTempGoal,
                        unit
                    )
                    : null;
            }
        );

        public static readonly Metric ThermalRate = new Metric("ThermalRate",
            "Thermal Rate",
            "Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.GetThermalFlux()),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.GetThermalFlux())
            },
            part => new Dictionary<Variable, double>(),
            part => part.GetThermalFlux(),
            (part, unit, prefix) => (part.GetThermalFlux() * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateInternal = new Metric("ThermalRateInternal",
            "Thermal Rate [I]",
            "Internal Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalInternalFluxPrevious),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalInternalFluxPrevious)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalInternalFluxPrevious,
            (part, unit, prefix) => (part.thermalInternalFluxPrevious * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateConductive = new Metric("ThermalRateConductive",
            "Thermal Rate [Cd]",
            "Conductive Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalConductionFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalConductionFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalConductionFlux,
            (part, unit, prefix) => (part.thermalConductionFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateConvective = new Metric("ThermalRateConvective",
            "Thermal Rate [Cv]",
            "Convective Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalConvectionFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalConvectionFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalConvectionFlux,
            (part, unit, prefix) => (part.thermalConvectionFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateRadiative = new Metric("ThermalRateRadiative",
            "Thermal Rate [R]",
            "Radiative Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalRadiationFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalRadiationFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalRadiationFlux,
            (part, unit, prefix) => (part.thermalRadiationFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateSkinToInternal = new Metric("ThermalRateSkinToInternal",
            "Thermal Rate [S-I]",
            "Skin to Internal Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.skinToInternalFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.skinToInternalFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.skinToInternalFlux,
            (part, unit, prefix) => (part.skinToInternalFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateInternalToSkin = new Metric("ThermalRateInternalToSkin",
            "Thermal Rate [I-S]",
            "Internal to Skin Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => -i.skinToInternalFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => -i.skinToInternalFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => -part.skinToInternalFlux,
            (part, unit, prefix) => (-part.skinToInternalFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        private readonly Func<Part, bool> _isApplicable;
        private readonly Func<Vessel, Dictionary<Variable, double>> _getVesselValues;
        private readonly Func<Part, Dictionary<Variable, double>> _getPartValues;
        private readonly Func<Part, double?> _getPartCurrent;
        private readonly Func<Part, Unit, Prefix?, string> _getPartCurrentString;

        public string Name { get; }
        public string ShortFriendlyName { get; }
        public string LongFriendlyName { get; }
        public Unit[] Units { get; }
        public bool EnablePrefixSelection { get; }

        private Metric(string name, string shortFriendlyName, string longFriendlyName,
            Unit[] units, bool enablePrefixSelection,
            Func<Part, bool> isApplicable,
            Func<Vessel, Dictionary<Variable, double>> getVesselValues,
            Func<Part, Dictionary<Variable, double>> getPartValues,
            Func<Part, double?> getPartCurrent,
            Func<Part, Unit, Prefix?, string> getPartCurrentString
        )
        {
            Name = name;
            ShortFriendlyName = shortFriendlyName;
            LongFriendlyName = longFriendlyName;
            Units = units;
            EnablePrefixSelection = enablePrefixSelection;
            _isApplicable = isApplicable;
            _getVesselValues = getVesselValues;
            _getPartValues = getPartValues;
            _getPartCurrent = getPartCurrent;
            _getPartCurrentString = getPartCurrentString;
        }

        public bool IsApplicable(Part part)
            => _isApplicable(part);

        public Dictionary<Variable, double> GetVesselValues(Vessel vessel)
            => _getVesselValues(vessel);

        public Dictionary<Variable, double> GetPartValues(Part part)
            => _getPartValues(part);

        public double? GetPartCurrent(Part part)
            => _getPartCurrent(part);

        public string GetPartCurrentString(Part part, Unit unit, Prefix? prefix)
            => _getPartCurrentString(part, unit, prefix);

        public static Metric Parse(string s)
        {
            var metric = TryParse(s);

            if (metric != null)
            {
                return metric;
            }
            else
            {
                throw new FormatException($"Could not parse Metric: {s}");
            }
        }

        public static Metric TryParse(string s)
        {
            if (s != null)
            {
                return (Metric)typeof(Metric).GetField(s, BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
            }

            return null;
        }

        #region Helpers

        private static double ConvertKelvinToRankine(double temp)
        {
            return temp * (9.0 / 5.0);
        }

        private static double ConvertKelvinToCelsius(double temp)
        {
            return temp - 273.15;
        }

        private static double ConvertKelvinToFahrenheit(double temp)
        {
            return temp * (9.0 / 5.0) - 459.67;
        }

        private static string TemperatureToString(
            double tempKelvin,
            double maxTempKelvin,
            double? idealTempKelvin,
            Unit unit
        )
        {
            double temp;
            double maxTemp;
            double? idealTemp = null;
            string unitSymbol;

            switch (unit)
            {
                case Unit.Kelvin:
                    temp = tempKelvin;
                    maxTemp = maxTempKelvin;
                    if (idealTempKelvin != null) idealTemp = idealTempKelvin;
                    unitSymbol = "K";
                    break;
                case Unit.Rankine:
                    temp = ConvertKelvinToRankine(tempKelvin);
                    maxTemp = ConvertKelvinToRankine(maxTempKelvin);
                    if (idealTempKelvin != null) idealTemp = ConvertKelvinToRankine(idealTempKelvin.Value);
                    unitSymbol = "°R";
                    break;
                case Unit.Celsius:
                    temp = ConvertKelvinToCelsius(tempKelvin);
                    maxTemp = ConvertKelvinToCelsius(maxTempKelvin);
                    if (idealTempKelvin != null) idealTemp = ConvertKelvinToCelsius(idealTempKelvin.Value);
                    unitSymbol = "°C";
                    break;
                case Unit.Fahrenheit:
                    temp = ConvertKelvinToFahrenheit(tempKelvin);
                    maxTemp = ConvertKelvinToFahrenheit(maxTempKelvin);
                    if (idealTempKelvin != null) idealTemp = ConvertKelvinToFahrenheit(idealTempKelvin.Value);
                    unitSymbol = "°F";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var result = $"{temp:F1}";

            if (idealTemp != null)
            {
                result += $" / {idealTemp.Value:F1}";
            }

            result += $" / {maxTemp:F1} {unitSymbol}";

            return result;
        }

        #endregion
    }
}
