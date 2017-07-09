using System;

namespace HotSpot.Model
{
    internal sealed class VariableBag
    {
        public double PartCurrent { get; set; }
        public double PartAbsoluteMinimum { get; set; }
        public double PartAbsoluteMaximum { get; set; }
        public double PartIdeal { get; set; }
        public double VesselCurrentMinimum { get; set; }
        public double VesselCurrentMaximum { get; set; }
        public double VesselAbsoluteMinimum { get; set; }
        public double VesselAbsoluteMaximum { get; set; }
        public double GradientMinimum { get; set; }
        public double GradientMaximum { get; set; }

        public double GetValue(Variable variable)
        {
            switch (variable)
            {
                case Variable.PartCurrent:
                    return PartCurrent;
                case Variable.PartAbsoluteMinimum:
                    return PartAbsoluteMinimum;
                case Variable.PartAbsoluteMaximum:
                    return PartAbsoluteMaximum;
                case Variable.PartIdeal:
                    return PartIdeal;
                case Variable.VesselCurrentMinimum:
                    return VesselCurrentMinimum;
                case Variable.VesselCurrentMaximum:
                    return VesselCurrentMaximum;
                case Variable.VesselAbsoluteMinimum:
                    return VesselAbsoluteMinimum;
                case Variable.VesselAbsoluteMaximum:
                    return VesselAbsoluteMaximum;
                case Variable.GradientMinimum:
                    return GradientMinimum;
                case Variable.GradientMaximum:
                    return GradientMaximum;
                default:
                    throw new ArgumentOutOfRangeException(nameof(variable), variable, null);
            }
        }

        public void ResetAll()
        {
            ResetPartSpecific();
            ResetVesselSpecific();
            
            GradientMinimum = double.NaN;
            GradientMaximum = double.NaN;
        }

        public void ResetVesselSpecific()
        {
            VesselCurrentMinimum = double.NaN;
            VesselCurrentMaximum = double.NaN;
            VesselAbsoluteMinimum = double.NaN;
            VesselAbsoluteMaximum = double.NaN;
        }

        public void ResetPartSpecific()
        {
            PartCurrent = double.NaN;
            PartAbsoluteMinimum = double.NaN;
            PartAbsoluteMaximum = double.NaN;
            PartIdeal = double.NaN;
        }
    }
}
