using System;

namespace HotSpot
{
    internal static class ConfigNodeExtensions
    {
        public static T Parse<T>(this ConfigNode node, string name) where T : struct
        {
            var value = node.GetValue(name);
            var parsed = value.TryParse<T>();

            if (parsed != null)
            {
                return parsed.Value;
            }
            else
            {
                throw new FormatException($"Could not parse node value of {name}: {value}");
            }
        }

        public static T? TryParse<T>(this ConfigNode node, string name) where T : struct
        {
            return node.GetValue(name).TryParse<T>();
        }
    }
}
