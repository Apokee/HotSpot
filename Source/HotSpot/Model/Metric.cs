using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotSpot.Model
{
    internal sealed class Metric
    {
        public static readonly Metric TemperatureInternal = new Metric("TemperatureInternal",
            "Temperature [I]",
            "Internal Temperature",
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
            (part, unit) =>
            {
                double temp;
                double maxTemp;
                string unitSymbol;

                switch (unit)
                {
                    case Unit.Kelvin:
                        temp = part.temperature;
                        maxTemp = part.maxTemp;
                        unitSymbol = "K";
                        break;
                    case Unit.Rankine:
                        temp = ConvertKelvinToRankine(part.temperature);
                        maxTemp = ConvertKelvinToRankine(part.maxTemp);
                        unitSymbol = "°R";
                        break;
                    case Unit.Celsius:
                        temp = ConvertKelvinToCelsius(part.temperature);
                        maxTemp = ConvertKelvinToCelsius(part.maxTemp);
                        unitSymbol = "°C";
                        break;
                    case Unit.Fahrenheit:
                        temp = ConvertKelvinToFahrenheit(part.temperature);
                        maxTemp = ConvertKelvinToFahrenheit(part.maxTemp);
                        unitSymbol = "°F";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return $"{temp:F2}{unitSymbol} / {maxTemp:F2}{unitSymbol}";
            }
        );

        public static readonly Metric TemperatureSkin = new Metric("TemperatureSkin",
            "Temperature [S]",
            "Skin Temperature",
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
            (part, unit) =>
            {
                double temp;
                double maxTemp;
                string unitSymbol;

                switch (unit)
                {
                    case Unit.Kelvin:
                        temp = part.skinTemperature;
                        maxTemp = part.skinMaxTemp;
                        unitSymbol = "K";
                        break;
                    case Unit.Rankine:
                        temp = ConvertKelvinToRankine(part.skinTemperature);
                        maxTemp = ConvertKelvinToRankine(part.skinMaxTemp);
                        unitSymbol = "°R";
                        break;
                    case Unit.Celsius:
                        temp = ConvertKelvinToCelsius(part.skinTemperature);
                        maxTemp = ConvertKelvinToCelsius(part.skinMaxTemp);
                        unitSymbol = "°C";
                        break;
                    case Unit.Fahrenheit:
                        temp = ConvertKelvinToFahrenheit(part.skinTemperature);
                        maxTemp = ConvertKelvinToFahrenheit(part.skinMaxTemp);
                        unitSymbol = "°F";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return $"{temp:F2}{unitSymbol} / {maxTemp:F2}{unitSymbol}";
            }
        );

        public static readonly Metric ThermalRate = new Metric("ThermalRate",
            "Thermal Rate",
            "Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.GetThermalFlux()),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.GetThermalFlux())
            },
            part => new Dictionary<Variable, double>(),
            part => part.GetThermalFlux(),
            (part, unit) => $"{part.GetThermalFlux():F2}kW"
        );

        public static readonly Metric ThermalRateInternal = new Metric("ThermalRateInternal",
            "Thermal Rate [I]",
            "Internal Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalInternalFluxPrevious),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalInternalFluxPrevious)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalInternalFluxPrevious,
            (part, unit) => $"{part.thermalInternalFluxPrevious:F2}kW"
        );

        public static readonly Metric ThermalRateConductive = new Metric("ThermalRateConductive",
            "Thermal Rate [Cd]",
            "Conductive Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalConductionFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalConductionFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalConductionFlux,
            (part, unit) => $"{part.thermalConductionFlux:F2}kW"
        );

        public static readonly Metric ThermalRateConvective = new Metric("ThermalRateConvective",
            "Thermal Rate [Cv]",
            "Convective Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalConvectionFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalConvectionFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalConvectionFlux,
            (part, unit) => $"{part.thermalConvectionFlux:F2}kW"
        );

        public static readonly Metric ThermalRateRadiative = new Metric("ThermalRateRadiative",
            "Thermal Rate [R]",
            "Radiative Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalRadiationFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalRadiationFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalRadiationFlux,
            (part, unit) => $"{part.thermalRadiationFlux:F2}kW"
        );

        private readonly Func<Vessel, Dictionary<Variable, double>> _getVesselValues;
        private readonly Func<Part, Dictionary<Variable, double>> _getPartValues;
        private readonly Func<Part, double> _getPartCurrent;
        private readonly Func<Part, Unit, string> _getPartCurrentString;

        public string Name { get; }
        public string ShortFriendlyName { get; }
        public string LongFriendlyName { get; }

        private Metric(string name, string shortFriendlyName, string longFriendlyName,
            Func<Vessel, Dictionary<Variable, double>> getVesselValues,
            Func<Part, Dictionary<Variable, double>> getPartValues,
            Func<Part, double> getPartCurrent,
            Func<Part, Unit, string> getPartCurrentString
        )
        {
            Name = name;
            ShortFriendlyName = shortFriendlyName;
            LongFriendlyName = longFriendlyName;
            _getVesselValues = getVesselValues;
            _getPartValues = getPartValues;
            _getPartCurrent = getPartCurrent;
            _getPartCurrentString = getPartCurrentString;
        }

        public Dictionary<Variable, double> GetVesselValues(Vessel vessel)
        {
            return _getVesselValues(vessel);
        }

        public Dictionary<Variable, double> GetPartValues(Part part)
        {
            return _getPartValues(part);
        }

        public double GetPartCurrent(Part part)
        {
            return _getPartCurrent(part);
        }

        public string GetPartCurrentString(Part part, Unit unit)
        {
            return _getPartCurrentString(part, unit);
        }

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

        #endregion
    }
}
