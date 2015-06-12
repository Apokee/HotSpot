using System;
using System.Linq;
using HotSpot.Model;
using HotSpot.Reflection;
using UnityEngine;

namespace HotSpot
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public sealed class HotSpotGuiBehavior : MonoBehaviour
    {
        private const int ConfigWindowId = 0x36F2911D; // Random

        private readonly ScreenMessage _screenMessage =
            new ScreenMessage(string.Empty, 4, ScreenMessageStyle.LOWER_CENTER);

        private bool _lastThermalColorsDebug;
        private ApplicationLauncherButton _applicationLauncherButton;

        private bool _showConfigWindow;
        private Rect _configWindowRect;
        private ConfigWindowTab _configWindowTabActive = ConfigWindowTab.Context;
        private bool _configWindowContextShowTemperatureUnit;
        private bool _configWindowOverlayShowMetrics;
        private bool _configWindowOverlayShowSchemes;

        #region MonoBehaiour

        public void Start()
        {
            Log.Trace("Entering HotSpotGuiBehavior.Start()");

            var buttonTexture = GameDatabase
                .Instance
                .GetTexture(Config.Instance.Gui.ButtonTexture, asNormalMap: false);

            if (buttonTexture != null)
            {
                Log.Debug($"Found button texture at: {Config.Instance.Gui.ButtonTexture}");

                _applicationLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    () => OnAppLauncherEvent(AppLauncherEvent.OnTrue),
                    () => OnAppLauncherEvent(AppLauncherEvent.OnFalse),
                    () => OnAppLauncherEvent(AppLauncherEvent.OnHover),
                    () => OnAppLauncherEvent(AppLauncherEvent.OnHoverOut),
                    () => OnAppLauncherEvent(AppLauncherEvent.OnEnable),
                    () => OnAppLauncherEvent(AppLauncherEvent.OnDisable),
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    buttonTexture
                );
            }
            else
            {
                Log.Warning(
                    $"Could not find button texture at: {Config.Instance.Gui.ButtonTexture} not creating" +
                    " Application Launcher button."
                );
            }

            Log.Trace("Leaving HotSpotGuiBehavior.Start()");
        }

        public void OnDestroy()
        {
            ApplicationLauncher.Instance.RemoveModApplication(_applicationLauncherButton);
        }

        // ReSharper disable once InconsistentNaming
        public void OnGUI()
        {
            if (_showConfigWindow)
            {
                if (_configWindowRect == default(Rect))
                {
                    const int width = 300;
                    const int height = 400;

                    var x = (Screen.width / 2) - (width / 2);
                    var y = (Screen.height / 2) - (height / 2);

                    _configWindowRect = new Rect(x, y, width, height);
                }

                GUI.skin = HighLogic.Skin;
                _configWindowRect = GUILayout.Window(ConfigWindowId, _configWindowRect, OnConfigWindow, "Hot Spot");
            }
        }

        public void LateUpdate()
        {
            Log.Trace("Entering HotSpotGuiBehavior.LateUpdate()");

            if (Config.Instance.Overlay.Enable)
            {
                LateUpdateScreenMessage();
            }

            Log.Trace("Leaving HotSpotGuiBehavior.LateUpdate()");
        }

        private void LateUpdateScreenMessage()
        {
            if (_lastThermalColorsDebug != PhysicsGlobals.ThermalColorsDebug)
            {
                Log.Debug("PhysicsGlobals.ThermalColorsDebug has been toggled");

                var flightOverlays = FindObjectOfType<FlightOverlays>();

                if (flightOverlays != null)
                {
                    Log.Debug("Found FlightOverlays, removing its ScreenMessage");

                    ScreenMessages.RemoveMessage(flightOverlays.TryGetScreenMessage());
                }

                if (Config.Instance.Overlay.EnableScreenMessage)
                {
                    var scheme = Config.Instance.Overlay.GetActiveMetric().GetActiveScheme();

                    var metricMsg = Config.Instance.Overlay.Metric.FriendlyName;
                    var schemMsg = scheme.FriendlyName == null ? string.Empty : $" ({scheme.FriendlyName})";
                    var stateMsg = PhysicsGlobals.ThermalColorsDebug ? "Enabled" : "Disabled";

                    ScreenMessages.PostScreenMessage($"{metricMsg}{schemMsg} Overlay: {stateMsg}", _screenMessage);
                }
            }

            _lastThermalColorsDebug = PhysicsGlobals.ThermalColorsDebug;
        }

        #endregion

        #region Event Handlers

        private void OnAppLauncherEvent(AppLauncherEvent appLauncherEvent)
        {
            Log.Trace("Entering PlaneMode.OnAppLauncherEvent()");

            switch (appLauncherEvent)
            {
                case AppLauncherEvent.OnTrue:
                    _showConfigWindow = true;
                    break;
                case AppLauncherEvent.OnFalse:
                    _showConfigWindow = false;
                    break;
                case AppLauncherEvent.OnHover:
                    break;
                case AppLauncherEvent.OnHoverOut:
                    break;
                case AppLauncherEvent.OnEnable:
                    break;
                case AppLauncherEvent.OnDisable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(appLauncherEvent));
            }

            Log.Trace("Leaving PlaneMode.OnAppLauncherEvent()");
        }

        private void OnConfigWindow(int windowId)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Context"))
            {
                _configWindowTabActive = ConfigWindowTab.Context;
            }
            if (GUILayout.Button("Overlay"))
            {
                _configWindowTabActive = ConfigWindowTab.Overlay;
            }
            GUILayout.EndHorizontal();

            switch (_configWindowTabActive)
            {
                case ConfigWindowTab.Context:
                    OnContextTab();
                    break;
                case ConfigWindowTab.Overlay:
                    OnOverlayTab();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GUI.DragWindow();
        }

        private void OnContextTab()
        {
            GUILayout.BeginVertical();
            foreach (var metricNode in Config.Instance.ContextMenu.Metrics)
            {
                GUILayout.BeginHorizontal();

                metricNode.Enable = GUILayout.Toggle(metricNode.Enable, metricNode.Name.FriendlyName);

                var isTemperatureMetric = metricNode.Name.Name == "Temperature";

                if (isTemperatureMetric)
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Unit"))
                    {
                        _configWindowContextShowTemperatureUnit = !_configWindowContextShowTemperatureUnit;
                    }
                }

                GUILayout.EndHorizontal();

                if (_configWindowContextShowTemperatureUnit && isTemperatureMetric)
                {
                    var temperatureMetricNode = Config
                        .Instance
                        .ContextMenu
                        .Metrics
                        .SingleOrDefault(i => i.Name.Name == "Temperature");

                    if (temperatureMetricNode != null)
                    {
                        GUILayout.BeginVertical();
                        var units = new[] { Unit.Kelvin, Unit.Rankine, Unit.Celsius, Unit.Fahrenheit };

                        int unitIndex;
                        switch (temperatureMetricNode.Unit)
                        {
                            case Unit.Kelvin:
                                unitIndex = 0;
                                break;
                            case Unit.Rankine:
                                unitIndex = 1;
                                break;
                            case Unit.Celsius:
                                unitIndex = 2;
                                break;
                            case Unit.Fahrenheit:
                                unitIndex = 3;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var newUnitIndex = GUILayout.SelectionGrid(unitIndex, units.Select(i => i.ToString()).ToArray(), 2);

                        temperatureMetricNode.Unit = units[newUnitIndex];

                        GUILayout.EndVertical();
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private void OnOverlayTab()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Metric:",
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.Width(55));
            GUILayout.Label(Config.Instance.Overlay.Metric.FriendlyName);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select", GUILayout.Width(50)))
            {
                _configWindowOverlayShowMetrics = !_configWindowOverlayShowMetrics;
            }

            GUILayout.EndHorizontal();

            if (_configWindowOverlayShowMetrics)
            {
                var metrics = Config.Instance.Overlay.Metrics;

                var metricIndex = 0;
                for (var i = 0; i < metrics.Length; i++)
                {
                    if (Config.Instance.Overlay.Metric.Name == metrics[i].Name.Name)
                    {
                        metricIndex = i;
                        break;
                    }
                }

                var newMetricIndex = GUILayout.SelectionGrid(
                    metricIndex, metrics.Select(i => i.Name.FriendlyName).ToArray(), 1
                );

                Config.Instance.Overlay.Metric = metrics[newMetricIndex].Name;
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label("Scheme:",
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.Width(55));
            GUILayout.Label(Config.Instance.Overlay.GetActiveMetric().GetActiveScheme().FriendlyName);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select", GUILayout.Width(50)))
            {
                _configWindowOverlayShowSchemes = !_configWindowOverlayShowSchemes;
            }

            GUILayout.EndHorizontal();

            if (_configWindowOverlayShowSchemes)
            {
                var scheme = Config.Instance.Overlay.GetActiveMetric().GetActiveScheme();
                var schemes = Config.Instance.Overlay.GetActiveMetric().Schemes;

                var schemeIndex = 0;
                for (var i = 0; i < schemes.Length; i++)
                {
                    if (scheme.Name == schemes[i].Name)
                    {
                        schemeIndex = i;
                        break;
                    }
                }

                var newSchemeIndex = GUILayout.SelectionGrid(
                    schemeIndex, schemes.Select(i => i.FriendlyName).ToArray(), 1
                );

                Config.Instance.Overlay.GetActiveMetric().Scheme = schemes[newSchemeIndex].Name;
            }

            GUILayout.EndVertical();
        }

        #endregion

        #region NestedTypes

        private enum AppLauncherEvent
        {
            OnTrue,
            OnFalse,
            OnHover,
            OnHoverOut,
            OnEnable,
            OnDisable,
        }

        private enum ConfigWindowTab
        {
            Context,
            Overlay
        }

        #endregion
    }
}
