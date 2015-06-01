using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration
{
    internal sealed class TemperatureNode : ContextMenuItemNode
    {
        public TemperatureUnit Unit { get; private set; } = TemperatureUnit.Kelvin;

        public override void Load(ConfigNode node)
        {
            base.Load(node);

            Unit = node.Parse<TemperatureUnit>("unit");
        }
    }
}