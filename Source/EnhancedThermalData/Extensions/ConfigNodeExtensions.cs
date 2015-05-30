using System;

namespace EnhancedThermalData.Extensions
{
    internal static class ConfigNodeExtensions
    {
        public static T Parse<T>(this ConfigNode node, string name)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), node.GetValue(name));
            }
            else
            {
                return (T)Convert.ChangeType(node.GetValue(name), typeof(T));
            }
        }
    }
}
