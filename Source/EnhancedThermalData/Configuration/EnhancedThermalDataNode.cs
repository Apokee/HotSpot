using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration
{
    internal sealed class EnhancedThermalDataNode : IConfigNode
    {
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
