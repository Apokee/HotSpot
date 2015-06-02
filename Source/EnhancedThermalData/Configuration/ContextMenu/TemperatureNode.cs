using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration.ContextMenu
{
    internal sealed class TemperatureNode : ContextMenuItemNode
    {
        public Unit Unit { get; private set; } = Unit.Kelvin;

        public override void Load(ConfigNode node)
        {
            base.Load(node);

            Unit = node.Parse<Unit>("unit");
        }
    }
}
