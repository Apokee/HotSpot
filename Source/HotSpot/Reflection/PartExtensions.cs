using System.Reflection;

namespace HotSpot.Reflection
{
    internal static class PartExtensions
    {
        private static readonly FieldInfo MaterialColorUpdaterField;

        static PartExtensions()
        {
            var field = typeof(Part).GetField("\u0006", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(MaterialColorUpdater))
            {
                MaterialColorUpdaterField = field;
            }
            else
            {
                Log.Warning("Could not find Part MaterialColorUpdater field");
            }
        }

        public static MaterialColorUpdater TryGetMaterialColorUpdater(this Part part)
        {
            return (MaterialColorUpdater) MaterialColorUpdaterField?.GetValue(part);
        }
    }
}
