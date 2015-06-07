using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using EnhancedThermalData.Model;
using UnityEngine;

namespace EnhancedThermalData.Configuration.Overlay
{
    internal sealed class StopNode : IConfigNode
    {
        private static readonly Regex ColorRegex =
            new Regex(@"^#(?<color>[0-9A-F]{6})$", RegexOptions.IgnoreCase);

        public string Name { get; private set; }
        public Expression Value { get; private set; }
        public double Factor { get; private set; }
        public Color? Color { get; private set; }
        public float? Alpha { get; private set; }

        public StopNode(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                Name = node.GetValue("name");
                Value = Expression.Parse(node.GetValue("value"));
                Factor = node.HasValue("factor") ? node.Parse<double>("factor") : 1.0;
                Color = TryParseColor(node.GetValue("color"));
                Alpha = TryParseSingle(node.GetValue("alpha"));

                if (Color == null && Alpha == null)
                {
                    Log.Warning($"STOP node does not contain an `color` or `alpha` property: {node}");
                }
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        public EvaluatedStopNode Evaluate(Dictionary<Variable, double> variables)
        {
            return new EvaluatedStopNode(this, variables);
        }

        public override string ToString()
        {
            var node = new ConfigNode();
            Save(node);

            return node.GetNode("STOP").ToString();
        }

        #region Helpers

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
