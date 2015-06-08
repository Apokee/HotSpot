using System;

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
