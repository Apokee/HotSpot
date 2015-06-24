using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotSpot.Model
{
    internal sealed class Metric
    {
        public static readonly Metric Temperature = new Metric("Temperature",
            "Temperature",
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
            part => part.temperature
        );

        public static readonly Metric ThermalRate = new Metric("ThermalRate",
            "Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.GetThermalFlux()),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.GetThermalFlux())
            },
            part => new Dictionary<Variable, double>(),
            part => part.GetThermalFlux()
        );

        public static readonly Metric ThermalRateInternal = new Metric("ThermalRateInternal",
            "Internal Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalInternalFluxPrevious),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalInternalFluxPrevious)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalInternalFluxPrevious
        );

        public static readonly Metric ThermalRateConductive = new Metric("ThermalRateConductive",
            "Conductive Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalConductionFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalConductionFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalConductionFlux
        );

        public static readonly Metric ThermalRateConvective = new Metric("ThermalRateConvective",
            "Convective Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalConvectionFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalConvectionFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalConvectionFlux
        );

        public static readonly Metric ThermalRateRadiative = new Metric("ThermalRateRadiative",
            "Radiative Thermal Rate",
            vessel => new Dictionary<Variable, double>
            {
                [Variable.VesselCurrentMinimum] = vessel.Parts.Min(i => i.thermalRadiationFlux),
                [Variable.VesselCurrentMaximum] = vessel.Parts.Max(i => i.thermalRadiationFlux)
            },
            part => new Dictionary<Variable, double>(),
            part => part.thermalRadiationFlux
        );

        private readonly Func<Vessel, Dictionary<Variable, double>> _getVesselValues;
        private readonly Func<Part, Dictionary<Variable, double>> _getPartValues;
        private readonly Func<Part, double> _getPartCurrent;

        public string Name { get; }
        public string FriendlyName { get; }

        private Metric(string name, string friendlyName,
            Func<Vessel, Dictionary<Variable, double>> getVesselValues,
            Func<Part, Dictionary<Variable, double>> getPartValues,
            Func<Part, double> getPartCurrent
        )
        {
            Name = name;
            FriendlyName = friendlyName;
            _getVesselValues = getVesselValues;
            _getPartValues = getPartValues;
            _getPartCurrent = getPartCurrent;
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
    }
}
