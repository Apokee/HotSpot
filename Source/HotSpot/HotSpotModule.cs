using System;
using HotSpot.Model;

namespace HotSpot
{
    // ReSharper disable once UnusedMember.Global
    public sealed class HotSpotModule : PartModule
    {
        [KSPField(guiActive = false, guiName = "Temperature")]
        // ReSharper disable once NotAccessedField.Global
        public string Temperature;

        [KSPField(guiActive = false, guiName = "Thermal Rate")]
        // ReSharper disable once NotAccessedField.Global
        public string ThermalRate;

        [KSPField(guiActive = false, guiName = "Thermal Rate [I]")]
        // ReSharper disable once NotAccessedField.Global
        public string ThermalRateInternal;

        [KSPField(guiActive = false, guiName = "Thermal Rate [Cd]")]
        // ReSharper disable once NotAccessedField.Global
        public string ThermalRateConductive;

        [KSPField(guiActive = false, guiName = "Thermal Rate [Cv]")]
        // ReSharper disable once NotAccessedField.Global
        public string ThermalRateConvective;

        [KSPField(guiActive = false, guiName = "Thermal Rate [R]")]
        // ReSharper disable once NotAccessedField.Global
        public string ThermalRateRadiative;

        public override void OnUpdate()
        {
            UpdateTemperature();
            UpdateThermalRate();
            UpdateThermalRateInternal();
            UpdateThermalRateConductive();
            UpdateThermalRateConvective();
            UpdateThermalRateRadiative();
        }

        #region Updaters

        private void UpdateTemperature()
        {
            var metric = Config.Instance.ContextMenu.GetMetric(Metric.Temperature);

            double temp;
            double maxTemp;
            string unit;

            switch (metric.Unit)
            {
                case Unit.Kelvin:
                    temp = part.temperature;
                    maxTemp = part.maxTemp;
                    unit = "K";
                    break;
                case Unit.Rankine:
                    temp = ConvertKelvinToRankine(part.temperature);
                    maxTemp = ConvertKelvinToRankine(part.maxTemp);
                    unit = "°R";
                    break;
                case Unit.Celsius:
                    temp = ConvertKelvinToCelsius(part.temperature);
                    maxTemp = ConvertKelvinToCelsius(part.maxTemp);
                    unit = "°C";
                    break;
                case Unit.Fahrenheit:
                    temp = ConvertKelvinToFahrenheit(part.temperature);
                    maxTemp = ConvertKelvinToFahrenheit(part.maxTemp);
                    unit = "°F";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Fields["Temperature"].guiActive = metric.Enable;
            Temperature = metric.Enable ? $"{temp:F2}{unit} / {maxTemp:F2}{unit}" : null;
        }

        private void UpdateThermalRate()
        {
            var metric = Config.Instance.ContextMenu.GetMetric(Metric.ThermalRate);

            Fields["ThermalRate"].guiActive = metric.Enable;
            ThermalRate = metric.Enable ? $"{part.GetThermalFlux():F2}kW" : null;
        }

        private void UpdateThermalRateInternal()
        {
            var metric = Config.Instance.ContextMenu.GetMetric(Metric.ThermalRateInternal);

            Fields["ThermalRateInternal"].guiActive = metric.Enable;
            ThermalRateInternal = metric.Enable ? $"{part.thermalInternalFlux:F2}kW" : null;
        }

        private void UpdateThermalRateConductive()
        {
            var metric = Config.Instance.ContextMenu.GetMetric(Metric.ThermalRateConductive);

            Fields["ThermalRateConductive"].guiActive = metric.Enable;
            ThermalRateConductive = metric.Enable ? $"{part.thermalConductionFlux:F2}kW" : null;
        }

        private void UpdateThermalRateConvective()
        {
            var metric = Config.Instance.ContextMenu.GetMetric(Metric.ThermalRateConvective);

            Fields["ThermalRateConvective"].guiActive = metric.Enable;
            ThermalRateConvective = metric.Enable ? $"{part.thermalConvectionFlux:F2}kW" : null;
        }

        private void UpdateThermalRateRadiative()
        {
            var metric = Config.Instance.ContextMenu.GetMetric(Metric.ThermalRateRadiative);

            Fields["ThermalRateRadiative"].guiActive = metric.Enable;
            ThermalRateRadiative = metric.Enable ? $"{part.thermalRadiationFlux:F2}kW" : null;
        }

        #endregion

        #region Helpers

        private static double ConvertKelvinToRankine(double temp)
        {
            return temp * (9.0 / 5.0);
        }

        private static double ConvertKelvinToCelsius(double temp)
        {
            return temp - 273.15;
        }

        private static double ConvertKelvinToFahrenheit(double temp)
        {
            return temp * (9.0 / 5.0) - 459.67;
        }

        #endregion
    }
}
