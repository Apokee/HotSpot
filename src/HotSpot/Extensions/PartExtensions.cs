namespace HotSpot
{
    internal static class PartExtensions
    {
        public static double GetThermalFlux(this Part part)
        {
            return part.thermalInternalFluxPrevious
                + part.thermalConductionFlux
                + part.thermalConvectionFlux
                + part.thermalRadiationFlux;
        }
    }
}
