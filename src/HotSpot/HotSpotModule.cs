namespace HotSpot
{
    public sealed class HotSpotModule : PartModule
    {
        [KSPField(guiActive = false)]
        public string TemperatureInternal;

        [KSPField(guiActive = false)]
        public string TemperatureSkin;

        [KSPField(guiActive = false)]
        public string TemperatureCore;

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

        [KSPField(guiActive = false)]
        public string ThermalRateSkinToInternal;

        [KSPField(guiActive = false)]
        public string ThermalRateInternalToSkin;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (Config.Instance.Gui.ContextMenu.DisableStockCoreTemp)
                DisableStockCoreTempDisplay();
        }

        public override void OnUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;

            foreach (var metricNode in Config.Instance.ContextMenu.Metrics)
            {
                var metric = metricNode.Name;

                if (metric.IsApplicable(part))
                {
                    var value = metric.GetPartCurrentString(part, metricNode.Unit, metricNode.Prefix);

                    if (value != null)
                    {
                        var field = Fields[metric.Name];

                        if (field != null)
                        {
                            field.guiName = metric.ShortFriendlyName;
                            field.guiActive = metricNode.Enable;

                            field.SetValue(value, this);
                        }
                        else
                        {
                            Log.Warning($"Could not find field for metric `{metric.Name}`.");
                        }
                    }
                    else
                    {
                        Log.Warning(
                            $"Received null value for for applicable metric `{metric.Name}` on part `{part.name}`."
                        );
                    }
                }
            }
        }

        private void DisableStockCoreTempDisplay()
        {
            var coreTempDisplayField = part
                .FindModuleImplementing<ModuleOverheatDisplay>()
                ?.Fields["coreTempDisplay"];

            if (coreTempDisplayField != null)
                coreTempDisplayField.guiActive = false;
        }
    }
}
