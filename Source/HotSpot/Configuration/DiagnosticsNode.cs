namespace HotSpot.Configuration
{
    internal sealed class DiagnosticsNode
    {
        public LogLevel LogLevel { get; }

        private DiagnosticsNode(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public static DiagnosticsNode GetDefault()
        {
            return new DiagnosticsNode(LogLevel.Info);
        }

        public static DiagnosticsNode TryParse(ConfigNode node)
        {
            if (node != null)
            {
                var logLevel = node.TryParse<LogLevel>("logLevel") ?? LogLevel.Info;

                return new DiagnosticsNode(logLevel);
            }

            Log.Warning("Could not parse missing DIAGNOSTICS node");
            return null;
        }
    }
}
