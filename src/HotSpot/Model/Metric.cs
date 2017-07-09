using System;
using System.Reflection;

namespace HotSpot.Model
{
    // TODO: Optimize, convert to subclasses
    internal sealed class Metric
    {
        public static readonly Metric TemperatureInternal = new Metric("TemperatureInternal",
            "Temp [I]",
            "Internal Temperature",
            new[] { Unit.Kelvin, Unit.Celsius, Unit.Rankine, Unit.Fahrenheit },
            false,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;
                variables.VesselAbsoluteMinimum = 0;
                variables.VesselAbsoluteMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    if (part.temperature < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = part.temperature;

                    if (part.temperature > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = part.temperature;

                    if (part.maxTemp > variables.VesselAbsoluteMaximum)
                        variables.VesselAbsoluteMaximum = part.maxTemp;
                }

                if (!foundValidPart)
                {
                    variables.VesselCurrentMinimum = double.NaN;
                    variables.VesselCurrentMaximum = double.NaN;
                    variables.VesselAbsoluteMinimum = double.NaN;
                    variables.VesselAbsoluteMaximum = double.NaN;
                }
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.temperature;
                variables.PartAbsoluteMinimum = 0;
                variables.PartAbsoluteMaximum = part.maxTemp;
            },
            (part, unit, prefix) => TemperatureToString(part.temperature, part.maxTemp, null, unit)
        );

        public static readonly Metric TemperatureSkin = new Metric("TemperatureSkin",
            "Temp [S]",
            "Skin Temperature",
            new[] { Unit.Kelvin, Unit.Celsius, Unit.Rankine, Unit.Fahrenheit },
            false,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;
                variables.VesselAbsoluteMinimum = 0;
                variables.VesselAbsoluteMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    if (part.skinTemperature < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = part.skinTemperature;

                    if (part.skinTemperature > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = part.skinTemperature;

                    if (part.skinMaxTemp > variables.VesselAbsoluteMaximum)
                        variables.VesselAbsoluteMaximum = part.skinMaxTemp;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.skinTemperature;
                variables.PartAbsoluteMinimum = 0;
                variables.PartAbsoluteMaximum = part.skinMaxTemp;
            },
            (part, unit, prefix) => TemperatureToString(part.skinTemperature, part.skinMaxTemp, null, unit)
        );

        public static readonly Metric TemperatureCore = new Metric("TemperatureCore",
            "Temp [C]",
            "Core Temperature",
            new[] { Unit.Kelvin, Unit.Celsius, Unit.Rankine, Unit.Fahrenheit },
            false,
            part => part.FindModuleImplementing<ModuleCoreHeat>() != null,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;
                variables.VesselAbsoluteMinimum = 0;
                variables.VesselAbsoluteMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    var part = vessel.Parts[i];

                    var coreHeat = part.FindModuleImplementing<ModuleCoreHeat>();

                    if (coreHeat != null)
                    {
                        foundValidPart = true;

                        if (coreHeat.CoreTemperature < variables.VesselCurrentMinimum)
                            variables.VesselCurrentMinimum = coreHeat.CoreTemperature;

                        if (coreHeat.CoreTemperature > variables.VesselCurrentMaximum)
                            variables.VesselCurrentMaximum = coreHeat.CoreTemperature;

                        if (coreHeat.CoreShutdownTemp > variables.VesselAbsoluteMaximum)
                            variables.VesselAbsoluteMaximum = coreHeat.CoreShutdownTemp;
                    }
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                var coreHeatModule = part.FindModuleImplementing<ModuleCoreHeat>();

                if (coreHeatModule != null)
                {
                    variables.PartCurrent = coreHeatModule.CoreTemperature;
                    variables.PartAbsoluteMinimum = 0;
                    variables.PartAbsoluteMaximum = coreHeatModule.CoreShutdownTemp;
                }
            },
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
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i<vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = part.GetThermalFlux();

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.GetThermalFlux();
            },
            (part, unit, prefix) => (part.GetThermalFlux() * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        // Use thermalInternalFluxPrevious since the current value is always zero at the time we read it.
        public static readonly Metric ThermalRateInternal = new Metric("ThermalRateInternal",
            "Thermal Rate [I]",
            "Internal Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = part.thermalInternalFluxPrevious;

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.thermalInternalFluxPrevious;
            },
            (part, unit, prefix) => (part.thermalInternalFluxPrevious * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateConductive = new Metric("ThermalRateConductive",
            "Thermal Rate [Cd]",
            "Conductive Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = part.thermalConductionFlux;

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.thermalConductionFlux;
            },
            (part, unit, prefix) => (part.thermalConductionFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateConvective = new Metric("ThermalRateConvective",
            "Thermal Rate [Cv]",
            "Convective Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = part.thermalConvectionFlux;

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.thermalConvectionFlux;
            },
            (part, unit, prefix) => (part.thermalConvectionFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateRadiative = new Metric("ThermalRateRadiative",
            "Thermal Rate [R]",
            "Radiative Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = part.thermalRadiationFlux;

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.thermalRadiationFlux;
            },
            (part, unit, prefix) => (part.thermalRadiationFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateSkinToInternal = new Metric("ThermalRateSkinToInternal",
            "Thermal Rate [S-I]",
            "Skin to Internal Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = part.skinToInternalFlux;

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = part.skinToInternalFlux;
            },
            (part, unit, prefix) => (part.skinToInternalFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        public static readonly Metric ThermalRateInternalToSkin = new Metric("ThermalRateInternalToSkin",
            "Thermal Rate [I-S]",
            "Internal to Skin Thermal Rate",
            new[] { Unit.Watt },
            true,
            part => true,
            (vessel, variables) =>
            {
                var foundValidPart = false;

                variables.VesselCurrentMinimum = double.PositiveInfinity;
                variables.VesselCurrentMaximum = double.NegativeInfinity;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < vessel.Parts.Count; i++)
                {
                    foundValidPart = true;

                    var part = vessel.Parts[i];

                    var thermalFlux = -part.skinToInternalFlux;

                    if (thermalFlux < variables.VesselCurrentMinimum)
                        variables.VesselCurrentMinimum = thermalFlux;

                    if (thermalFlux > variables.VesselCurrentMaximum)
                        variables.VesselCurrentMaximum = thermalFlux;
                }

                if (!foundValidPart)
                    variables.ResetVesselSpecific();
            },
            (part, variables) =>
            {
                variables.PartCurrent = -part.skinToInternalFlux;
            },
            (part, unit, prefix) => (-part.skinToInternalFlux * 1000.0).ToQuantityString(prefix, "W", "F2")
        );

        private readonly Func<Part, bool> _isApplicable;
        private readonly Action<Vessel, VariableBag> _populateVesselVariables;
        private readonly Action<Part, VariableBag> _populatePartVariables;
        private readonly Func<Part, Unit, Prefix?, string> _getPartCurrentString;

        public string Name { get; }
        public string ShortFriendlyName { get; }
        public string LongFriendlyName { get; }
        public Unit[] Units { get; }
        public bool EnablePrefixSelection { get; }

        private Metric(string name, string shortFriendlyName, string longFriendlyName,
            Unit[] units, bool enablePrefixSelection,
            Func<Part, bool> isApplicable,
            Action<Vessel, VariableBag> populateVesselVariables,
            Action<Part, VariableBag> populatePartVariables,
            Func<Part, Unit, Prefix?, string> getPartCurrentString
        )
        {
            Name = name;
            ShortFriendlyName = shortFriendlyName;
            LongFriendlyName = longFriendlyName;
            Units = units;
            EnablePrefixSelection = enablePrefixSelection;
            _isApplicable = isApplicable;
            _populateVesselVariables = populateVesselVariables;
            _populatePartVariables = populatePartVariables;
            _getPartCurrentString = getPartCurrentString;
        }

        public bool IsApplicable(Part part)
            => _isApplicable(part);

        public void PopulateVesselVariables(Vessel vessel, VariableBag variables)
            => _populateVesselVariables(vessel, variables);

        public void PopulatePartVariables(Part part, VariableBag variables)
            => _populatePartVariables(part, variables);

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
