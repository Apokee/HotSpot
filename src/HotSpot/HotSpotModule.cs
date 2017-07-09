using UnityEngine;

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

        private float _timeSinceLastUpdate = float.PositiveInfinity;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (Config.Instance.Gui.ContextMenu.DisableStockCoreTemp)
                DisableStockCoreTempDisplay();
        }

        public override void OnUpdate()
        {
            if (ShouldUpdate())
            {
                var metrics = Config.Instance.ContextMenu.Metrics;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < metrics.Length; i++)
                {
                    var metricNode = metrics[i];

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

                _timeSinceLastUpdate = 0;
            }
            else
            {
                _timeSinceLastUpdate += Time.deltaTime;
            }
        }

        private bool ShouldUpdate()
        {
            return HighLogic.LoadedSceneIsFlight && _timeSinceLastUpdate >= Config.Instance.ContextMenu.UpdatePeriod;
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
