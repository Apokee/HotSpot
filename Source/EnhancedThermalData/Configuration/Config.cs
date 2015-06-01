using System.Linq;
using EnhancedThermalData.Diagnostics;

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
                                GameDatabase.Instance.GetConfigNodes("ENHANCED_THERMAL_DATA").SingleOrDefault()
                            );

                            _instance = enhancedThermalDataNode;

                            OnInitialLoad();
                        }
                    }
                }

                return _instance;
            }
        }

        private static void OnInitialLoad()
        {
            Log.Level = Instance.Diagnostics.LogLevel;
        }
    }
}
