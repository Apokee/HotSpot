using System;

namespace EnhancedThermalData.Configuration
{
    internal sealed class EnhancedThermalDataNode : IConfigNode
    {
        public ContextMenuNode ContextMenu { get; } = new ContextMenuNode();
        public ThermalOverlayNode ThermalOverlay { get; } = new ThermalOverlayNode();
        public DiagnosticsNode Diagnostics { get; } = new DiagnosticsNode();

        public void Load(ConfigNode node)
        {
            if (node != null)
            {
                ContextMenu.Load(node.GetNode("CONTEXT_MENU"));
                ThermalOverlay.Load(node.GetNode("THERMAL_OVERLAY"));
                Diagnostics.Load(node.GetNode("DIAGNOSTICS"));
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
