using System;
using EnhancedThermalData.Configuration.ContextMenu;

namespace EnhancedThermalData.Configuration
{
    internal sealed class ContextMenuNode : IConfigNode
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
    }
}
