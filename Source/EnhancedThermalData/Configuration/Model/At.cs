namespace EnhancedThermalData.Configuration.Model
{
    internal struct At
    {
        public readonly float Value;
        public readonly AtUnit Unit;

        public At(float value, AtUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public override string ToString()
        {
            var unitStr = string.Empty;
            switch (Unit)
            {
                case AtUnit.Percentage:
                    unitStr = "%";
                    break;
                case AtUnit.Kelvin:
                    unitStr = "K";
                    break;
            }

            return $"{Value}{unitStr}";
        }

        public enum AtUnit
        {
            Percentage,
            Kelvin,
        }
    }
}
