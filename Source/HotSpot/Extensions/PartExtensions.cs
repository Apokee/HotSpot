namespace HotSpot
{
    internal static class PartExtensions
    {
        public static double GetThermalFlux(this Part part)
        {
            return part.thermalInternalFlux
                + part.thermalConductionFlux
                + part.thermalConvectionFlux
                + part.thermalRadiationFlux
                + part.thermalRadiationFlux;
        }
    }
}
