using System;
using System.Linq;
using EnhancedThermalData.Configuration;
using EnhancedThermalData.Diagnostics;
using EnhancedThermalData.Model;

namespace EnhancedThermalData
{
    // ReSharper disable once UnusedMember.Global
    public sealed class EnhancedThermalDataModule : PartModule
    {
        [KSPField(guiActive = false)]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Temperature;

        public override void OnUpdate()
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
