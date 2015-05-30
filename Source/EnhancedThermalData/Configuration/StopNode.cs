using System;
using System.Globalization;
using System.Text.RegularExpressions;
using EnhancedThermalData.Diagnostics;
using UnityEngine;

namespace EnhancedThermalData.Configuration
{
    internal sealed class StopNode : IConfigNode
    {
        private static readonly Regex AtRegex =
            new Regex(@"^(?<value>\d+(?:\.\d+)?)(?<unit>K|%)$");

        private static readonly Regex ColorRegex =
            new Regex(@"^#(?<color>[0-9A-F]{6})$", RegexOptions.IgnoreCase);

        // ReSharper disable MemberCanBePrivate.Global
        public string Name { get; private set; }
        public At? At { get; private set; }
        public Color? Color { get; private set; }
        public float? Alpha { get; private set; }
        // ReSharper restore MemberCanBePrivate.Global

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = node.GetValue("name");
                At = TryParseAt(node.GetValue("at"));
                Color = TryParseColor(node.GetValue("color"));
                Alpha = TryParseSingle(node.GetValue("alpha"));

                if (At == null)
                {
                    Log.Warning($"STOP node does not contain an `at` property: {node}");
                }

                if (Color == null && Alpha == null)
                {
                    Log.Warning($"STOP node does not contain an `color` or `alpha` property: {node}");
                }
            }
        }

        public void Save(ConfigNode node)
        {
            var stopNode = node.AddNode("STOP");

            if (Name != null)
            {
                stopNode.AddValue("name", Name);
            }

            if (At != null)
            {
                stopNode.AddValue("at", At.Value);
            }

            if (Color != null)
            {
                stopNode.AddValue("color", Color.Value);
            }

            if (Alpha != null)
            {
                stopNode.AddValue("alpha", Alpha.Value);
            }
        }

        public GradientColorKey? TryConvertToColorKey(double maxValue)
        {
            if (At != null && Color != null)
            {
                switch (At.Value.Unit)
                {
                    case AtUnit.Percentage:
                        return new GradientColorKey(Color.Value, At.Value.Value / 100);
                    case AtUnit.Kelvin:
                        return new GradientColorKey(Color.Value, (float)(At.Value.Value / maxValue));
                    default:
                        throw new ArgumentOutOfRangeException(nameof(At.Value.Unit));
                }
            }

            return null;
        }

        public GradientAlphaKey? TryConvertToAlphaKey(double maxValue)
        {
            if (At != null && Alpha != null)
            {
                switch (At.Value.Unit)
                {
                    case AtUnit.Percentage:
                        return new GradientAlphaKey(Alpha.Value, At.Value.Value / 100);
                    case AtUnit.Kelvin:
                        return new GradientAlphaKey(Alpha.Value, (float)(At.Value.Value / maxValue));
                    default:
                        throw new ArgumentOutOfRangeException(nameof(At.Value.Unit));
                }
            }

            return null;
        }

        public override string ToString()
        {
            var node = new ConfigNode();
            Save(node);

            return node.GetNode("STOP").ToString();
        }

        #region Helpers

        private static At? TryParseAt(string str)
        {
            var match = AtRegex.Match(str);

            if (match.Success)
            {
                var value = TryParseSingle(match.Groups["value"].Value);
                var unit = TryParseAtUnit(match.Groups["unit"].Value);

                if (value != null && unit != null)
                {
                    return new At(value.Value, unit.Value);
                }
            }

            Log.Warning($"Could not prase `at` property: {str}");
            return null;
        }

        private static AtUnit? TryParseAtUnit(string str)
        {
            switch (str)
            {
                case "%":
                    return AtUnit.Percentage;
                case "K":
                    return AtUnit.Kelvin;
                default:
                    return null;
            }
        }

        private static Color? TryParseColor(string str)
        {
            var match = ColorRegex.Match(str);

            if (match.Success)
            {
                return TryConvertColor(match.Groups["color"].Value);
            }

            Log.Warning($"Could not parse `color` property: {str}");
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

        private static float? TryParseSingle(string str)
        {
            float value;
            return float.TryParse(str, out value) ? value : (float?)null;
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
