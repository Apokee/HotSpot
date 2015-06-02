using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using EnhancedThermalData.Configuration.Model;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;
using UnityEngine;

namespace EnhancedThermalData.Configuration
{
    internal sealed class Config : IConfigNode
    {
        #region Singleton

        private static readonly object InstanceLock = new object();
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            var config = new Config();
                            config.Load(
                                GameDatabase.Instance.GetConfigNodes("ENHANCED_THERMAL_DATA").SingleOrDefault()
                            );

                            _instance = config;

                            OnInitialLoad();
                        }
                    }
                }

                return _instance;
            }
        }

        private static void OnInitialLoad()
        {
            Log.Level = Instance.Diagnostics.LogLevel;
        }

        #endregion

        public ContextMenuNode ContextMenu { get; } = new ContextMenuNode();
        public OverlayNode Overlay { get; } = new OverlayNode();
        public DiagnosticsNode Diagnostics { get; } = new DiagnosticsNode();

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                ContextMenu.Load(node.GetNode("CONTEXT_MENU"));
                Overlay.Load(node.GetNode("OVERLAY"));
                Diagnostics.Load(node.GetNode("DIAGNOSTICS"));
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        public sealed class ContextMenuNode : IConfigNode
        {
            public TemperatureNode Temperature { get; } = new TemperatureNode();
            public ContextMenuItemNode ThermalRateInternal { get; } = new ContextMenuItemNode();
            public ContextMenuItemNode ThermalRateConductive { get; } = new ContextMenuItemNode();
            public ContextMenuItemNode ThermalRateConvective { get; } = new ContextMenuItemNode();
            public ContextMenuItemNode ThermalRateRadiative { get; } = new ContextMenuItemNode();
            public ContextMenuItemNode ThermalRate { get; } = new ContextMenuItemNode();

            public void Load(ConfigNode node)
            {
                if (node != null)
                {
                    Temperature.Load(node.GetNode("TEMPERATURE"));
                    ThermalRateInternal.Load(node.GetNode("THERMAL_RATE_INTERNAL"));
                    ThermalRateConductive.Load(node.GetNode("THERMAL_RATE_CONDUCTIVE"));
                    ThermalRateConvective.Load(node.GetNode("THERMAL_RATE_CONVECTIVE"));
                    ThermalRateRadiative.Load(node.GetNode("THERMAL_RATE_RADIATIVE"));
                    ThermalRate.Load(node.GetNode("THERMAL_RATE"));
                }
            }

            public void Save(ConfigNode node)
            {
                throw new NotImplementedException();
            }

            public class ContextMenuItemNode : IConfigNode
            {
                public bool Enable { get; private set; } = true;

                public virtual void Load(ConfigNode node)
                {
                    Enable = node.Parse<bool>("enable");
                }

                public virtual void Save(ConfigNode node)
                {
                    throw new NotImplementedException();
                }
            }

            public sealed class TemperatureNode : ContextMenuItemNode
            {
                public TemperatureUnit Unit { get; private set; } = TemperatureUnit.Kelvin;

                public override void Load(ConfigNode node)
                {
                    base.Load(node);

                    Unit = node.Parse<TemperatureUnit>("unit");
                }
            }
        }

        public sealed class OverlayNode : IConfigNode
        {
            private readonly Dictionary<OverlayMode, ModeNode> _modeNodes = new Dictionary<OverlayMode, ModeNode>();

            public bool Enable { get; private set; } = true;
            public OverlayMode Mode { get; private set; } = OverlayMode.Temperature;
            public IEnumerable<ModeNode> Modes => _modeNodes.Values;

            public ModeNode this[OverlayMode mode] => _modeNodes[mode];

            public OverlayNode()
            {
                foreach (OverlayMode mode in Enum.GetValues(typeof(OverlayMode)))
                {
                    _modeNodes[mode] = new ModeNode();
                }
            }

            public void Load(ConfigNode node)
            {
                if (node != null)
                {
                    Enable = node.Parse<bool>("enable");
                    Mode = node.Parse<OverlayMode>("mode");

                    foreach (var modeNode in node.GetNodes("MODE").Select(i => new ModeNode(i)))
                    {
                        _modeNodes[modeNode.Name] = modeNode;
                    }
                }
            }

            public void Save(ConfigNode node)
            {
                throw new NotImplementedException();
            }

            public enum OverlayMode
            {
                Temperature,
                ThermalRateInternal
            }

            public sealed class ModeNode : IConfigNode
            {
                public OverlayMode Name { get; private set; }
                public GradientNode Gradient { get; } = new GradientNode();

                public ModeNode() { }

                public ModeNode(ConfigNode node)
                {
                    Load(node);
                }

                public void Load(ConfigNode node)
                {
                    if (node != null)
                    {
                        Name = node.Parse<OverlayMode>("name");
                        Gradient.Load(node.GetNode("GRADIENT"));
                    }
                }

                public void Save(ConfigNode node)
                {
                    throw new NotImplementedException();
                }

                internal sealed class GradientNode : IConfigNode
                {
                    public List<StopNode> Stops { get; } = new List<StopNode>();

                    public void Load(ConfigNode node)
                    {
                        if (node != null)
                        {
                            Stops.AddRange(node.GetNodes("STOP").Select(i =>
                            {
                                var stopNode = new StopNode();
                                stopNode.Load(i);

                                return stopNode;
                            }));
                        }
                    }

                    public void Save(ConfigNode node)
                    {
                        throw new NotImplementedException();
                    }

                    public sealed class StopNode : IConfigNode
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

                        public GradientColorKey? TryConvertToColorKey(double minValue, double maxValue)
                        {
                            if (At != null && Color != null)
                            {
                                switch (At.Value.Unit)
                                {
                                    case Model.At.AtUnit.Percentage:
                                        return new GradientColorKey(Color.Value, At.Value.Value / 100);
                                    case Model.At.AtUnit.Kelvin:
                                        return new GradientColorKey(Color.Value, (float)((At.Value.Value - minValue) / (maxValue - minValue)));
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(At.Value.Unit));
                                }
                            }

                            return null;
                        }

                        public GradientAlphaKey? TryConvertToAlphaKey(double minValue, double maxValue)
                        {
                            if (At != null && Alpha != null)
                            {
                                switch (At.Value.Unit)
                                {
                                    case Model.At.AtUnit.Percentage:
                                        return new GradientAlphaKey(Alpha.Value, At.Value.Value / 100);
                                    case Model.At.AtUnit.Kelvin:
                                        return new GradientAlphaKey(Alpha.Value, (float)((At.Value.Value - minValue) / (maxValue - minValue)));
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

                        private static At.AtUnit? TryParseAtUnit(string str)
                        {
                            switch (str)
                            {
                                case "%":
                                    return Model.At.AtUnit.Percentage;
                                case "K":
                                    return Model.At.AtUnit.Kelvin;
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
            }
        }

        internal sealed class DiagnosticsNode : IConfigNode
        {
            public LogLevel LogLevel { get; private set; } = LogLevel.Info;

            public void Load(ConfigNode node)
            {
                if (node != null)
                {
                    LogLevel = node.Parse<LogLevel>("logLevel");
                }
            }

            public void Save(ConfigNode node)
            {
                throw new NotImplementedException();
            }
        }
    }
}
