using System;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Extensions;

namespace EnhancedThermalData.Configuration
{
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
