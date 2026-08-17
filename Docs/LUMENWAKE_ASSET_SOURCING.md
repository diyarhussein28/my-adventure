# Lumenwake Reef — Asset Sourcing Decision

## Decision

Lumenwake Reef will use a **free-only, license-documented asset pipeline**. Paid Unity Asset Store packages are excluded from the vertical slice. No payment was submitted and no paid environment package was acquired.

The earlier review of a commercial seaside-town kit confirmed that it would have been technically compatible but it is not part of this repository or production plan. The free implementation instead combines CC0 scanned environment models and PBR textures from Poly Haven with the user’s free Mixamo character/animation workflow.

| Need | Selected free source | License and technical status | Intended Lumenwake role |
| --- | --- | --- | --- |
| Coastal formations | Poly Haven Coast Rocks 05 | CC0; 1K FBX with declared PBR dependency set; imported. | Shoreline silhouette, cover, reef boundary. |
| Maritime dock | Poly Haven Modular Wooden Pier | CC0; 1K FBX with declared PBR dependency set; imported. | Fisher dock and Brine Tax supply route. |
| Harbor landmark | Poly Haven Ship Pinnace | CC0; 1K FBX with declared PBR dependency set; imported. | Moored offshore narrative landmark. |
| Port dressing | Poly Haven Wine Barrel 01 | CC0; 1K FBX with declared PBR dependency set; imported. | Market/harbor storytelling and small-scale encounter cover. |
| Tide-gate door | Poly Haven Large Castle Door | CC0; 1K FBX with declared PBR dependency set; imported. | Reef-house doors and Breathing Eye mechanism. |
| Sand | Poly Haven Aerial Beach 01 | CC0; existing project material source. | Beach terrain layer. |
| Nahlia base | Mixamo Erika Archer | User-acquired free FBX; Unity Humanoid import path. | Temporary rigged heroine base until a bespoke free mesh is identified. |
| Animation set | Mixamo motions | User-approved free acquisition path. | Locomotion, combat, recovery, and traversal clips. |

## Import rules

The project owns the downloaded source files and imports them through `MixamoImportPostprocessor`. Character meshes are stored under `Assets/Resources/Lumenwake/Characters/`; environment assets and their declared texture dependencies are stored under `Assets/Resources/Lumenwake/Environment/`. The exact Poly Haven download URLs remain in `Assets/Resources/Lumenwake/Environment/source_manifest.json`.

> A free asset may be technically usable without being visually appropriate. The source standard is therefore not merely “free”; every asset must provide a readable silhouette, coherent material response, and a purpose in Lumenwake’s reef-town story.

## Production limitation

Free source assets permit a credible, asset-driven vertical slice, but they do not automatically create a unique hero, bespoke enemy faction, or full AAA animation library. The vertical slice must label stock base meshes correctly and preserve the replacement path for custom art later. This is a production constraint, not a reason to retain capsules, billboards, or visible primitive placeholder geometry.
