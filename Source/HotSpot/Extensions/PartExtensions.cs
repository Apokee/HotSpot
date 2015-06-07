using System.Reflection;
using UnityEngine;

namespace HotSpot
{
    internal static class PartExtensions
    {
        // The obfuscated field name is prone to change with updates to KSP
        private static readonly FieldInfo MaterialColorUpdaterField =
            typeof(Part).GetField("\u0006", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void UpdateMaterialColor(this Part part, Color color)
        {
            (MaterialColorUpdaterField.GetValue(part) as MaterialColorUpdater)?.Update(color);
        }

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
