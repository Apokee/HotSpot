using System;

namespace HotSpot
{
    public static class StringExtensions
    {
        public static bool TryParseEnum<T>(this string s, out T result)
            where T : struct, IConvertible
        {
            var retValue = s != null && Enum.IsDefined(typeof(T), s);
            result = retValue ?
                (T)Enum.Parse(typeof(T), s) :
                default(T);

            return retValue;
        }
    }
}
