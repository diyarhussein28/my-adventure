# Starter Island Art Pass

The starter scene now uses an original visual identity built around a warm tropical maritime world, a teal-and-coral hero palette, and a contrasting crimson raider faction. This is the first **art-integration pass**: gameplay actors retain their prototype collision and combat components while presentation is upgraded through generated character concept panels and real textured environment materials.

| Asset | Unity location | Current role |
| --- | --- | --- |
| Tide Warden concept | `Assets/Resources/Art/Characters/tide_warden.png` | Camera-facing player presentation panel in the starter scene |
| Crimson Raider concept | `Assets/Resources/Art/Characters/crimson_raider.png` | Camera-facing enemy presentation panel in the starter scene |
| Tropical island ground | `Assets/Resources/Art/Environment/tropical_island_ground.png` | Tiled material texture on the island and beach primitives |
| Ocean water | `Assets/Resources/Art/Environment/ocean_water.png` | Tiled material texture on the ocean plane |
| Starter-island key art | `Assets/Art/Concepts/starter_island_keyart.png` | Environmental target and world-building reference |
| Tide Warden reference | `Assets/Art/Concepts/tide_warden_style_reference.png` | Stable hero identity and style reference |

The runtime bridge is intentionally temporary. `PrototypeSceneBootstrap` loads the artwork with `Resources.Load`, applies environment textures to the existing gameplay geometry, and adds `CharacterArtBillboard` to each transparent character-art panel. The final production path is to replace the billboards with rigged character prefabs, Animator Controllers, and game-ready 3D meshes while retaining the same controller, combat, and quest scripts.

> The generated visual assets establish an original game style; they are not a substitute for optimised production 3D models, normal maps, or final VFX assets.
