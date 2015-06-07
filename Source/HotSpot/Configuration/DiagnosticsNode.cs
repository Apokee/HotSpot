using System;

namespace HotSpot.Configuration
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
