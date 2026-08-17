# Lumenwake Reef — Free Asset Manifest

## License policy

All imported third-party assets in this manifest must be free to use under their documented license. This slice uses **Poly Haven CC0** assets for environmental models and materials. CC0 assets may be used without a paid purchase, but their source and intended use remain documented for production traceability.

| Asset | Source | License | Imported repository location | Lumenwake use |
| --- | --- | --- | --- | --- |
| Coast Rocks 05 | https://polyhaven.com/a/coast_rocks_05 | CC0 | `Assets/Resources/Lumenwake/Environment/coast_rocks_05/` | Reef boundary formations, low-tide boulders, and combat cover. |
| Coastal Cliff 04 | https://polyhaven.com/a/coastal_cliff_04 | CC0 | Not imported: exceeds the project’s commit-safe source-size limit. | Breathing Eye rim, distant skyline, and vertical traversal silhouette. |
| Modular Wooden Pier | https://polyhaven.com/a/modular_wooden_pier | CC0 | `Assets/Resources/Lumenwake/Environment/modular_wooden_pier/` | Fisher dock, tide-gate approach, and Brine Tax loading route. |
| Wine Barrel 01 | https://polyhaven.com/a/wine_barrel_01 | CC0 | `Assets/Resources/Lumenwake/Environment/wine_barrel_01/` | Port clutter, cover, and market storytelling. |
| Large Castle Door | https://polyhaven.com/a/large_castle_door | CC0 | `Assets/Resources/Lumenwake/Environment/large_castle_door/` | Reused and rescaled as the weathered tide-gate control door. |
| Aerial Beach 01 | https://polyhaven.com/a/aerial_beach_01 | CC0 | `Assets/Resources/Environment/PBR/Beach/` | Shoreline sand material and terrain layer. |
| Small Harbor 01 HDRI | https://polyhaven.com/a/small_harbor_01 | CC0 | Pending acquisition | Optional natural-light sky source for a non-commercial test build. |

The Poly Haven 1K FBX bundles are accompanied by declared diffuse, normal, roughness, ambient-occlusion, and metallic dependencies where the original source provided them. Their exact downloaded URLs and file names are preserved in `Assets/Resources/Lumenwake/Environment/source_manifest.json`. The free **Ship Pinnace** landmark is also imported at `Assets/Resources/Lumenwake/Environment/ship_pinnace/`.

## Character and motion policy

The user’s authenticated Mixamo account remains the approved free source for a Unity Humanoid-compatible Nahlia base mesh and locomotion/combat clips. The selected base is **Erika Archer**, downloaded by the user as an FBX and imported to `Assets/Resources/Lumenwake/Characters/Nahlia/ErikaArcher.fbx`. The active Mixamo library is available at `https://www.mixamo.com/#/?page=1&type=Motion`; it is currently loaded with Erika Archer selected for animation retargeting. The final Nahlia model must be separately documented when acquired. A stock base mesh cannot be represented as an original custom character asset; the project will label it as a temporary licensed production base until custom clothing, material, weapon, and animation work establishes Nahlia’s final identity.
