using System;
using EnhancedThermalData.Configuration;
using EnhancedThermalData.Model;

namespace EnhancedThermalData
{
    // ReSharper disable once UnusedMember.Global
    public sealed class EnhancedThermalDataModule : PartModule
    {
        [KSPField(guiActive = false, guiName = "Temperature")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Temperature;

        [KSPField(guiActive = false, guiName = "Thermal Rate [I]")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string ThermalRateInternal;

        [KSPField(guiActive = false, guiName = "Thermal Rate [Cd]")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string ThermalRateConductive;

        [KSPField(guiActive = false, guiName = "Thermal Rate [Cv]")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string ThermalRateConvective;

        [KSPField(guiActive = false, guiName = "Thermal Rate [R]")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string ThermalRateRadiative;

        [KSPField(guiActive = false, guiName = "Thermal Rate")]
        // ReSharper disable once MemberCanBePrivate.Global
        public string ThermalRate;

        public override void OnUpdate()
        {
            UpdateTemperature();
            UpdateThermalRateInternal();
            UpdateThermalRateConductive();
            UpdateThermalRateConvective();
            UpdateThermalRateRadiative();
            UpdateThermalRate();
        }

        #region Updaters

        private void UpdateTemperature()
        {
            double temp;
            double maxTemp;
            string unit;

            switch (Config.Instance.ContextMenu.Temperature.Unit)
            {
                case TemperatureUnit.Kelvin:
                    temp = part.temperature;
                    maxTemp = part.maxTemp;
                    unit = "K";
                    break;
                case TemperatureUnit.Rankine:
                    temp = ConvertKelvinToRankine(part.temperature);
                    maxTemp = ConvertKelvinToRankine(part.maxTemp);
                    unit = "°R";
                    break;
                case TemperatureUnit.Celsius:
                    temp = ConvertKelvinToCelsius(part.temperature);
                    maxTemp = ConvertKelvinToCelsius(part.maxTemp);
                    unit = "°C";
                    break;
                case TemperatureUnit.Fahrenheit:
                    temp = ConvertKelvinToFahrenheit(part.temperature);
                    maxTemp = ConvertKelvinToFahrenheit(part.maxTemp);
                    unit = "°F";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Fields["Temperature"].guiActive = Config.Instance.ContextMenu.Temperature.Enable;
            Temperature = Config.Instance.ContextMenu.Temperature.Enable ?
                $"{temp:F2}{unit} / {maxTemp:F2}{unit}" :
                null;
        }

        private void UpdateThermalRateInternal()
        {
            Fields["ThermalRateInternal"].guiActive = Config.Instance.ContextMenu.ThermalRateInternal.Enable;
            ThermalRateInternal = Config.Instance.ContextMenu.ThermalRateInternal.Enable ?
                $"{part.thermalInternalFlux:F2}kW" :
                null;
        }

        private void UpdateThermalRateConductive()
        {
            Fields["ThermalRateConductive"].guiActive = Config.Instance.ContextMenu.ThermalRateConductive.Enable;
            ThermalRateConductive = Config.Instance.ContextMenu.ThermalRateConductive.Enable ?
                $"{part.thermalConductionFlux:F2}kW" :
                null;
        }

        private void UpdateThermalRateConvective()
        {
            Fields["ThermalRateConvective"].guiActive = Config.Instance.ContextMenu.ThermalRateConvective.Enable;
            ThermalRateConvective = Config.Instance.ContextMenu.ThermalRateConvective.Enable ?
                $"{part.thermalConvectionFlux:F2}kW" :
                null;
        }

        private void UpdateThermalRateRadiative()
        {
            Fields["ThermalRateRadiative"].guiActive = Config.Instance.ContextMenu.ThermalRateRadiative.Enable;
            ThermalRateRadiative = Config.Instance.ContextMenu.ThermalRateRadiative.Enable ?
                $"{part.thermalRadiationFlux:F2}kW" :
                null;
        }

        private void UpdateThermalRate()
        {
            var thermalRate = part.thermalInternalFlux
                + part.thermalConductionFlux
                + part.thermalConvectionFlux
                + part.thermalRadiationFlux;

            Fields["ThermalRate"].guiActive = Config.Instance.ContextMenu.ThermalRate.Enable;
            ThermalRate = Config.Instance.ContextMenu.ThermalRate.Enable ?
                $"{thermalRate:F2}kW" :
                null;
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
