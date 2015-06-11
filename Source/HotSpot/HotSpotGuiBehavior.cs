using System;
using System.Collections.Generic;
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
        private Dictionary<string, bool> _configWindowContextMetricEnabled = new Dictionary<string, bool>(); 

        #region MonoBehaiour

        public void Start()
        {
            Log.Trace("Entering HotSpotGuiBehavior.Start()");

            foreach (var metricNode in Config.Instance.ContextMenu.Metrics)
            {
                _configWindowContextMetricEnabled[metricNode.Name.Name] = metricNode.Enable;
            }

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
                    _configWindowRect = new Rect(0, 0, 400, 500);
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

            GUILayout.BeginScrollView(new Vector2(), GUI.skin.scrollView);
            switch (_configWindowTabActive)
            {
                case ConfigWindowTab.Context:
                    OnContextTab();
                    break;
                case ConfigWindowTab.Overlay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void OnContextTab()
        {
            GUILayout.BeginVertical();
            foreach (var metricNode in Config.Instance.ContextMenu.Metrics)
            {
                GUILayout.BeginHorizontal();
                metricNode.Enable = GUILayout.Toggle(metricNode.Enable, metricNode.Name.FriendlyName);
                GUILayout.EndHorizontal();
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
