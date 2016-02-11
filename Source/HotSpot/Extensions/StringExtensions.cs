using System;
using HotSpot.Configuration;

namespace HotSpot
{
    public static class StringExtensions
    {
        public static T? TryParse<T>(this string s) where T : struct
        {
            if (s != null)
            {
                if (typeof(T).IsEnum)
                {
                    return Enum.IsDefined(typeof(T), s) ? (T)Enum.Parse(typeof(T), s) : (T?)null;
                }
                else if (typeof(T) == typeof(AutoBoolean))
                {
                    AutoBoolean result;
                    if (AutoBoolean.TryParse(s, out result))
                        return (T)(object)result;
                    else
                        return null;
                }
                else
                {
                    var parsed = Convert.ChangeType(s, typeof(T));

                    if (parsed != null)
                    {
                        return (T)parsed;
                    }
                }
            }

            return null;
        }
    }
}
