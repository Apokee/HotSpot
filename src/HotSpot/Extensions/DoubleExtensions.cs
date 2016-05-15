using System;
using HotSpot.Model;

namespace HotSpot
{
    internal static class DoubleExtensions
    {
        private static readonly string[] PrefixStrings = {
            "y",
            null,
            null,
            "z",
            null,
            null,
            "a",
            null,
            null,
            "f",
            null,
            null,
            "p",
            null,
            null,
            "n",
            null,
            null,
            "\u03bc",
            null,
            null,
            "m",
            "c",
            "d",
            string.Empty,
            "da",
            "h",
            "k",
            null,
            null,
            "M",
            null,
            null,
            "G",
            null,
            null,
            "T",
            null,
            null,
            "P",
            null,
            null,
            "E",
            null,
            null,
            "Z",
            null,
            null,
            "Y"
        };

        public static Prefix GetBestPrefix(this double value)
        {
            if (Math.Abs(value) > double.Epsilon)
            {
                var actualDegree = (int)Math.Floor(Math.Log10(Math.Abs(value)));
                var actualDegreeAbs = Math.Abs(actualDegree);

                var nearestDegreeAbs = actualDegreeAbs <= 3
                    ? actualDegreeAbs
                    : Math.Min(actualDegreeAbs - actualDegreeAbs % 3, 24);

                return (Prefix)(Math.Sign(actualDegree) * nearestDegreeAbs);
            }
            else
            {
                return Prefix.None;
            }
        }

        public static double ScaleToPrefix(this double value, Prefix prefix)
        {
            return value * Math.Pow(10, -(int)prefix);
        }

        public static string ToQuantityString(this double value, Prefix? prefix, string unit, string format)
        {
            prefix = prefix ?? value.GetBestPrefix();

            return $"{value.ScaleToPrefix(prefix.Value).ToString(format)} {PrefixStrings[(int)prefix + 24]}{unit}";
        }
    }
}
