## v0.2.0-alpha
##### New
- Added thermal rate metrics to both overlays and context menu. These measure the change in thermal energy of a part
  over time in units of energy/time, i.e. power. There are four discrete thermal rates: internal, conductive,
  convective, and radiative. There is also a general thermal rate metric which is the combination of the previous
  four. The overlays for thermal rate are purely relative, i.e. the part with the lowest will be purple and the part
  with the highest thermal rate will be red, regardless of their absolute values.
- The screen message displayed when overlays are enabled/disabled are now customized based on the current metric and
  scheme. The screen message can also be disabled entirely in the configuration.

##### Change
- *Incompatible:* Configuration structure has changed significantly.

## v0.1.1
##### Fix
- Fix settings being loaded multiple times which would eventually cause the thermal overlays to fail.

## v0.1.0
##### New
- Replace thermal overlay gradient colors with more intuitive scheme.
- Add display of temperature and max temperature values to part context menu.
