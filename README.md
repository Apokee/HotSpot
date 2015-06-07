# Hot Spot [![Build status][build-badge]][build]

**Hot Spot** is a Kerbal Space Program mod that displays better thermal data to the user. It currently supports the
following features:

### Improved Thermal Overlay Colors
Replaces the color gradient used for thermal overlays to be more intuitive.

- -273.15°C (Purple) to 0.00°C (Blue)
- 0.00°C (Blue) to 14.40°C (Transparent)
- 14.40°C (Transparent) to 100.00°C (Yellow)
- 100.00°C (Yellow) to 67% of Maximum (Orange)
- 67% of Maximum (Orange) to Maximum (Red)

### Part Temperature in Context Menu
Adds the current and maximum temperature to the context menu of all parts.

## Usage
To install, extract the contents of the archive to your KSP directory. This should create an `HotSpot` directory under
the `<KSP>/GameData` directory. Hot Spot requires [Module Manager][module-manager] to be installed.

Once installed press the `TOGGLE_TEMP_OVERLAY` key (`F11` by default) to enable temperature overlays. Right clicking
on parts will also display their current and maximum temperatures.

## Configuration
Hot Spot can be configured by creating Module Manager patches against the default settings stored in
`<KSP>/GameData/HotSpot/Configuration/HotSpot.cfg`. How to use Module Manager is outside the scope of this README,
please see the Module Manager documentation for more information.

[build]: https://ci.appveyor.com/project/Apokee/enhancedthermaldata
[build-badge]: https://ci.appveyor.com/api/projects/status/6mbuc9x563rwup5a?svg=true
[module-manager]: http://forum.kerbalspaceprogram.com/threads/55219
