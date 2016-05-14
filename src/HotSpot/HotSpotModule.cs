namespace HotSpot
{
    public sealed class HotSpotModule : PartModule
    {
        [KSPField(guiActive = false)]
        public string TemperatureInternal;

        [KSPField(guiActive = false)]
        public string TemperatureSkin;

        [KSPField(guiActive = false)]
        public string ThermalRate;

        [KSPField(guiActive = false)]
        public string ThermalRateInternal;

        [KSPField(guiActive = false)]
        public string ThermalRateConductive;

        [KSPField(guiActive = false)]
        public string ThermalRateConvective;

        [KSPField(guiActive = false)]
        public string ThermalRateRadiative;

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                foreach (var metricNode in Config.Instance.ContextMenu.Metrics)
                {
                    var metric = metricNode.Name;
                    var field = Fields[metric.Name];

                    field.guiName = metric.ShortFriendlyName;
                    field.guiActive = metricNode.Enable;

                    var value = metric.GetPartCurrentString(part, metricNode.Unit);

                    field.SetValue(value, this);
                }
            }
        }
    }
}
