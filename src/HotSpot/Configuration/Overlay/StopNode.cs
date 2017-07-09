using System;
using System.Globalization;
using System.Text.RegularExpressions;
using HotSpot.Model;
using UnityEngine;

namespace HotSpot.Configuration.Overlay
{
    internal sealed class StopNode
    {
        private static readonly Regex ColorRegex =
            new Regex(@"^#(?<color>[0-9A-F]{6})$", RegexOptions.IgnoreCase);

        public string Name { get; }
        public Expression Value { get; }
        public double Factor { get; }
        public Color? Color { get; }
        public float? Alpha { get; }

        private StopNode(string name, Expression value, double factor, Color? color, float? alpha)
        {
            Name = name;
            Value = value;
            Factor = factor;
            Color = color;
            Alpha = alpha;
        }

        public EvaluatedStopNode Evaluate(VariableBag variables)
        {
            return new EvaluatedStopNode(this, variables);
        }

        public static StopNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var name = node.GetValue("name");
                var value = Expression.TryParse(node.GetValue("value"));
                var factor = node.TryParse<double>("factor") ?? 1.0;
                var color = TryParseColor(node.GetValue("color"));
                var alpha = node.TryParse<float>("alpha");

                if (name != null && value != null && (color != null || alpha != null))
                {
                    return new StopNode(name, value, factor, color, alpha);
                }
            }

            Log.Debug($"Could not parse config node:{Environment.NewLine}{node}");
            return null;
        }

        #region Helpers

        private static Color? TryParseColor(string str)
        {
            var match = ColorRegex.Match(str);

            if (match.Success)
            {
                return TryConvertColor(match.Groups["color"].Value);
            }

            Log.Debug($"Could not parse `color` property: {str}");
            return null;
        }

        private static Color? TryConvertColor(string str)
        {
            var rgba = TryParseUInt32Hex(str + "ff");

            return rgba != null ? ConvertColor(rgba.Value) : (Color?)null;
        }

        private static Color ConvertColor(uint rgba)
        {
            var r = (byte)((rgba & 0xff000000) >> 24);
            var g = (byte)((rgba & 0x00ff0000) >> 16);
            var b = (byte)((rgba & 0x0000ff00) >> 8);
            var a = (byte)((rgba * 0x000000ff) >> 0);

            return ConvertColor(r, g, b, a);
        }

        private static Color ConvertColor(byte r, byte g, byte b, byte a)
        {
            const float max = byte.MaxValue;
            return new Color(r / max, g / max, b / max, a);
        }

        private static uint? TryParseUInt32Hex(string str)
        {
            uint value;
            return uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value) ?
                value :
                (uint?)null;
        }

        #endregion
    }
}
