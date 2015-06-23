# Hot Spot [![Build status][build-badge]][build]

**Hot Spot** is a [MIT-licensed](LICENSE.md) Kerbal Space Program mod that displays better thermal metric data to the
user. It currently supports the following metrics:

- Temperature
- Thermal Rate

Thermal rate is the rate at which thermal energy is added or removed from a part. It is measured as energy per unit
time, i.e. [*power*][wiki-power]. Positive thermal rates indicate a part is gaining thermal energy, negative thermal
rates indicate a part is losing thermal energy. There are multiple kinds of thermal rates:

- *Internal Thermal Rate* - The change in thermal energy due to reactions/processes occuring within the part itself.
- *Conductive Thremal Rate* - The change in thermal energy due to being in contact with other parts.
- *Convective Thermal Rate* - The change in thermal energy due to being in contact with a fluid, like the atmosphere.
- *Radiative Thermal Rate* - The change in thermal energy due to the emission or absorption of light.
- *Thermal Rate* - An aggregate value of the four previous rates.

These metrics are can be displayed in one of two ways:

### Context Menu
Hot Spot will add the metrics to the right-click context menu of all parts. Both the current and maximum temperature
of a part are displayed and the unit can be changed between [Kelvin][wiki-kelvin], [Celsius][wiki-celsius],
[Rankine][wiki-rankine], and [Fahrenheit][wiki-fahrenheit]. Thermal rates are always displayed in units of Kilowatts
(kW).

### Part Overlay

Hot Spot will also replace the stock *Thermal Debug Overlay* with one displaying the metric of the user's choice and
will replace the standard black body color gradient with one more intuitive.

#### Temperature
The color gradient used for temperature is as follows:

- Purple (0K)
- Blue (273.2K)
- Transparent (287.5K)
- Yellow (373.13K)
- Orange (0.67×*Maximum*)
- Red (*Maximum*)

The *Maximum* value depends on the scheme used:

- *Part Absolute* - The maximum temperature of the current part.
- *Vessel Current* - The maximum current temperature of any part in the vessel.
- *Vessel Absolute* - The maximum temperature of any part in the vessel.

## Installation
To install, extract the contents of the archive to your KSP directory. This should create an `HotSpot` directory under
the `<KSP>/GameData` directory. All dependencies should also be installed.

Dependencies:
- [Module Manager][module-manager]

## Usage
Right-clicking on parts will display enabled metrics. Pressing the `TOGGLE_TEMP_OVERLAY` key (`F11` by default)
enables metric overlays. Dynamic configuration can be done by pressing the Hot Spot button in the application
launcher.

## Configuration
More features of Hot Spot can be configured by creating Module Manager patches against the default settings stored in
`<KSP>/GameData/HotSpot/Configuration/HotSpot.cfg`. How to use Module Manager is outside the scope of this README,
please see the Module Manager documentation for more information.

[build]: https://ci.appveyor.com/project/Apokee/hotspot/branch/develop
[build-badge]: https://ci.appveyor.com/api/projects/status/ik9la5jusinnpu5n/branch/develop?svg=true
[module-manager]: http://forum.kerbalspaceprogram.com/threads/55219
[wiki-celsius]: https://en.wikipedia.org/wiki/Celsius
[wiki-fahrenheit]: https://en.wikipedia.org/wiki/Fahrenheit
[wiki-kelvin]: https://en.wikipedia.org/wiki/Kelvin
[wiki-power]: https://en.wikipedia.org/wiki/Power_%28physics%29
[wiki-rankine]: https://en.wikipedia.org/wiki/Rankine_scale
