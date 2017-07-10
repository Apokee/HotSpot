## v0.8.0
##### Changed
- Added new setting to `CONTEXT_MENU` node named `updatePeriod`. Its value is the number of seconds to wait between
  updates of context menu values. Defaults to a value of `0.250` (250 milliseconds or 4 times per second).
- Various performance improvements.

## v0.7.3
##### Changed
- Compatibility with KSP 1.3.0.

## v0.7.2
##### Fixed
- [#75](https://github.com/Apokee/HotSpot/issues/75) &mdash; Fixed `ToolbarWrapper` compatibility with Contract Configurator.

## v0.7.1
##### Changed
- Compatibility with KSP 1.2.0.

## v0.7.0
##### Changed
- *Incompatible:* "Dynamic" settings (anything changeable in the GUI) are now stored in
  `HotSpot/PluginData/HotSpot.cfg` instead of `HotSpot/Configuration/HotSpotPlayer.cfg` and override any values in
  "static" settings (anything loaded via the `GameDatabase`). This allows ModuleManager to ignore these settings for
  caching purposes.

## v0.6.0
##### Added
- Added core temperature as a new metric for applicable parts.
- Added skin to internal and internal to skin thermal rate as new metrics.
- Disable stock core temperature display in the part context menu by default.
- Added ideal metric values for applicable metrics (only core temperature as of now).
- Added "Part Ideal" overlay scheme for the core temperature metric which is green when around ideal temperature,
  purple and blue when below ideal temperature, and yellow and red when above ideal temperature.
- Added SI prefix selection for thermal rate metrics. By default, prefix is selected automatically.

##### Changed
- Changed temperature formatting:
  - Unit symbol is only printed at the very end.
  - Only one decimal place is shown instead of two.
  - If there is an ideal temperature it is displayed between the current and maximum temperatures.

##### Deprecated
- Unit has changed from `Kilowatt` to `Watt` to more clearly identify what it represents now that prefixes can be
  changed. `Kilowatt` will automatically be translated to `Watt` for now.

##### Fixed
- "Scheme:" label no longer occupies multiple lines in the GUI.

## v0.5.1
##### Changed
- Compatibility with KSP 1.1.0.

## v0.5.0
##### Added
- Support for [Toolbar](http://forum.kerbalspaceprogram.com/index.php?/topic/55420-/) contributed by
  [nanathan](https://github.com/nanathan).

## v0.4.4
##### Fixed
- Support for [KIS](http://forum.kerbalspaceprogram.com/threads/113111) stackables.

## v0.4.3
##### Fixed
- Add aggregate thermal rate metric to overlay configuration.

## v0.4.2
##### Fixed
- Fix aggregate thermal rate calculation. It was double counting radiative thermal rate.

## v0.4.1
##### Fixed
- Fix for configuration settings reverting to default after two reloads of KSP.

## v0.4.0
##### Added
- Configuration settings are now persisted on scene change.

## v0.3.0
##### Added
- Skin temperature added as a new metric.

##### Changed
- *Incompatible:* Configuration structure for temperature metrics has changed.
- "Temperature" in context menus abbreviated to "Temp."

##### Fixed
- Fix Internal Thermal Rate metric always being 0kW.

## v0.2.0
##### Added
- Added thermal rate metrics to both overlays and context menu. These measure the change in thermal energy of a part
  over time in units of energy/time, i.e. power. There are four discrete thermal rates: internal, conductive,
  convective, and radiative. There is also a general thermal rate metric which is the combination of the previous
  four. The overlays for thermal rate are purely relative, i.e. the part with the lowest will be purple and the part
  with the highest thermal rate will be red, regardless of their absolute values.
- The screen message displayed when overlays are enabled/disabled are now customized based on the current metric and
  scheme. The screen message can also be disabled entirely in the configuration.
- Added a GUI which allows changing various options dynamically, including: context menu metrics to enable/disable,
  the temperature unit, the overlay metric, and the gradient scheme for the metric. Currently this options are not
  persisted between loads.

##### Changed
- *Incompatible:* Configuration structure has changed significantly.

## v0.1.1
##### Fixed
- Fix settings being loaded multiple times which would eventually cause the thermal overlays to fail.

## v0.1.0
##### Added
- Replace thermal overlay gradient colors with more intuitive scheme.
- Add display of temperature and max temperature values to part context menu.
