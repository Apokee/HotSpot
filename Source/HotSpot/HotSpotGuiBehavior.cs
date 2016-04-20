using System;
using System.Collections.Generic;
using System.Linq;
using HotSpot.Compat.Toolbar;
using HotSpot.Configuration;
using HotSpot.Reflection;
using KSP.UI.Screens;
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
        private IButton _toolbarButton;

        private bool _showConfigWindow;
        private Rect _configWindowRect;
        private ConfigWindowTab _configWindowTabActive = ConfigWindowTab.Context;
        private readonly Dictionary<string, bool> _configWindowContextShowUnits = new Dictionary<string, bool>();
        private bool _configWindowOverlayShowMetrics;
        private bool _configWindowOverlayShowSchemes;

        #region MonoBehaiour

        public void Start()
        {
            Log.Trace("Entering HotSpotGuiBehavior.Start()");

            foreach (var metricNode in Config.Instance.ContextMenu.Metrics)
            {
                _configWindowContextShowUnits[metricNode.Name.Name] = false;
            }

            bool enableToolbar;
            bool enableAppLauncher;

            if (Config.Instance.Gui.Toolbar.Enable == AutoBoolean.Auto)
                enableToolbar = ToolbarManager.ToolbarAvailable;
            else if (Config.Instance.Gui.Toolbar.Enable == AutoBoolean.False)
                enableToolbar = false;
            else
                enableToolbar = true;

            if (Config.Instance.Gui.AppLauncher.Enable == AutoBoolean.Auto)
                enableAppLauncher = !enableToolbar;
            else if (Config.Instance.Gui.AppLauncher.Enable == AutoBoolean.False)
                enableAppLauncher = false;
            else
                enableAppLauncher = true;

            if (enableToolbar)
            {
                var buttonTexturePath = Config.Instance.Gui.Toolbar.Texture;
                var buttonTexture = GameDatabase.Instance.GetTexture(buttonTexturePath, asNormalMap: false);

                if (buttonTexture != null)
                {
                    Log.Debug($"Found toolbar button texture at: {buttonTexturePath}");

                    _toolbarButton = ToolbarManager.Instance.add("HotSpot", "config");
                    _toolbarButton.TexturePath = buttonTexturePath;
                    _toolbarButton.ToolTip = "HotSpot Configuration";
                    _toolbarButton.Enabled = true;
                    _toolbarButton.OnClick += e => OnAppLauncherEvent(AppLauncherEvent.OnToggle);
                }
                else
                {
                    Log.Warning(
                        $"Could not find toolbar button texture at: {buttonTexturePath} not creating" +
                        " Toolbar button."
                    );
                }
            }

            if (enableAppLauncher)
            {
                var buttonTexturePath = Config.Instance.Gui.AppLauncher.Texture;
                var buttonTexture = GameDatabase.Instance.GetTexture(buttonTexturePath, asNormalMap: false);

                if (buttonTexture != null)
                {
                    Log.Debug($"Found applauncher button texture at: {buttonTexturePath}");

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
                        $"Could not find applauncher button texture at: {buttonTexturePath} not creating" +
                        " Application Launcher button."
                    );
                }
            }

            Log.Trace("Leaving HotSpotGuiBehavior.Start()");
        }

        public void OnDestroy()
        {
            if(_applicationLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(_applicationLauncherButton);
                _applicationLauncherButton = null;
            }
            if(_toolbarButton != null)
            {
                _toolbarButton.Destroy();
                _toolbarButton = null;
            }
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

                    var screenMessage = flightOverlays.TryGetScreenMessage();
                    if (screenMessage != null)
                    {
                        ScreenMessages.RemoveMessage(screenMessage);
                    }
                }

                if (Config.Instance.Overlay.EnableScreenMessage)
                {
                    var scheme = Config.Instance.Overlay.GetActiveMetric().GetActiveScheme();

                    var metricMsg = Config.Instance.Overlay.Metric.LongFriendlyName;
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
                case AppLauncherEvent.OnToggle:
                    _showConfigWindow = !_showConfigWindow;
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

                metricNode.Enable = GUILayout.Toggle(metricNode.Enable, metricNode.Name.LongFriendlyName);

                if (metricNode.Name.Units.Length > 1)
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Unit"))
                    {
                        _configWindowContextShowUnits[metricNode.Name.Name] =
                            !_configWindowContextShowUnits[metricNode.Name.Name];
                    }
                }

                GUILayout.EndHorizontal();

                if (_configWindowContextShowUnits[metricNode.Name.Name])
                {
                    GUILayout.BeginVertical();

                    var unitIndex = 0;
                    for (var i = 0; i < metricNode.Name.Units.Length; i++)
                    {
                        if (metricNode.Name.Units[i] == metricNode.Unit)
                        {
                            unitIndex = i;
                            break;
                        }
                    }

                    var newUnitIndex = GUILayout.SelectionGrid(unitIndex, metricNode.Name.Units.Select(i => i.ToString()).ToArray(), 2);

                    metricNode.Unit = metricNode.Name.Units[newUnitIndex];

                    GUILayout.EndVertical();
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
            GUILayout.Label(Config.Instance.Overlay.Metric.LongFriendlyName);
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
                    metricIndex, metrics.Select(i => i.Name.LongFriendlyName).ToArray(), 1
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
            OnToggle,
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
