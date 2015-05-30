using System;
using EnhancedThermalData.Extensions;
using EnhancedThermalData.Model;

namespace EnhancedThermalData.Configuration
{
    internal sealed class TemperatureNode : IConfigNode
    {
        public bool Enable { get; private set; } = true;
        public TemperatureUnit Unit { get; private set; } = TemperatureUnit.Kelvin;

        public void Load(ConfigNode node)
        {
            Enable = node.Parse<bool>("enable");
            Unit = node.Parse<TemperatureUnit>("unit");
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}