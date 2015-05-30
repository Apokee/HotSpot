namespace EnhancedThermalData.Configuration
{
    internal static class Config
    {
        private static readonly object InstanceLock = new object();
        private static EnhancedThermalDataNode _instance;

        public static EnhancedThermalDataNode Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            var enhancedThermalDataNode = new EnhancedThermalDataNode();
                            enhancedThermalDataNode.Load(
                                GameDatabase.Instance.GetConfigNode("ENHANCED_THERMAL_DATA")
                            );

                            _instance = enhancedThermalDataNode;
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
