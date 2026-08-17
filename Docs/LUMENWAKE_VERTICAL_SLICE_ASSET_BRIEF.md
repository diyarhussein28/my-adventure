# Lumenwake Reef — Asset-Driven Vertical Slice Brief

## Purpose

This document defines the first production-quality playable slice for **Seas of Legends**. It replaces every player-visible primitive, generated palm, runtime placeholder material, concept-art panel, and duplicate character presentation in the former starter scene. The slice is not a generic tropical island; it is the Tide Warden Nahlia Vey’s civic reef at the moment the Brine Tax seizes its waterworks.

## Acceptance standard

A playable build passes visual review only when the player sees a distinct rigged heroine, differentiated Brine Tax adversaries, a coherent reef-town location, authored props at every navigation distance, physically credible terrain/material response, and a boss arena with an identifiable story purpose. A capsule, cube weapon, billboard portrait, flat fog backdrop, or repeated single character mesh is a blocking defect.

| Layer | Required asset outcome | Explicitly rejected |
| --- | --- | --- |
| Hero | Nahlia-specific rigged humanoid mesh, Humanoid Avatar, idle/walk/run/jump/attack/block/hit animation set, glaive mesh, and water-combat VFX anchors. | Vanguard mesh, T-pose, generic concept panel, or color-tinted duplicate mesh. |
| Enemies | At least two Brine Tax silhouettes: Salt Runner and Reefbreaker; unique materials, weapons, combat rhythms, and hit reactions. | One hero mesh reused as every enemy. |
| Environment | Reefstone terraces, weathered docks, tide gate, coral outcrops, fishing structures, wet stone, beach, shallow water, deep water, and coastal vegetation. | A terrain plane with cylinders, cubes, or untextured primitive vegetation. |
| Lighting | Golden-hour sun, cool sky fill, wet-surface reflections, local lantern contrast, post-processing tone map, and readable boss-space lighting. | Flat directional light, uniform fog, or ungraded default sky. |
| Arena | The Breathing Eye tide-gate chamber: circular civic machinery, visible reservoir below, breakable reef barricades, water-level changes, and clear combat perimeter. | Open empty terrain with a single enemy spawn. |
| Audio/VFX | Footstep materials, water/wind ambience, weapon swing layers, contact hit-stop, water trail and parry effects, pressure-engine boss cues. | Silent placeholder combat. |

## Visual target

The environment should read as a lived-in reef municipality rather than a beach. Low tide exposes coral stairs and barnacle-covered retaining walls. Blue ceramic roof tiles and weathered indigo sailcloth add human scale between stone terraces. The Brine Tax’s brass pressure equipment, black rubber capes, and cobalt water tanks provide a clear industrial intrusion against the pale coral and turquoise lagoon. The Breathing Eye must be visible as a distant luminous sinkhole landmark before it becomes the boss arena.

Nahlia’s asset identity needs five stable anchors: a practical adult reef guardian; indigo layered sailcloth; pearl-thread shell armor; close braided hair suitable for diving; and a collapsible crescent glaive. Her animation emphasizes low center-of-gravity footwork, athletic spear vaults, water-assisted side steps, and decisive parries. It must never read as a generic bow character, sci-fi soldier, or unarmed survivor.

## Asset acquisition manifest

| Priority | Asset type | Minimum technical specification | Destination |
| --- | --- | --- | --- |
| P0 | Nahlia hero mesh | Licensed FBX/GLB, adult female humanoid, rigged, PBR materials, 10k–80k triangles, Unity Humanoid compatible. | `Assets/Art/Lumenwake/Characters/Nahlia/` |
| P0 | Hero animation suite | Idle, walk, run, strafe, jump, fall, land, light attack 1–3, block, hit, dodge, execution. FBX for Unity, 30 FPS, in-place except authored root-motion attacks. | `Assets/Art/Lumenwake/Animations/Nahlia/` |
| P0 | Brine Tax meshes | Two or more differentiated licensed humanoid mesh/prefab variants with weapons. | `Assets/Art/Lumenwake/Characters/BrineTax/` |
| P0 | Reef environment kit | Modular reefstone, docks, houses, gates, coral, boats, foliage, prop set, and compatible materials. | `Assets/Art/Lumenwake/Environment/` |
| P1 | Water system | Built-in pipeline-compatible water material or verified URP conversion path, foam, shore interaction, and caustic support. | `Assets/Art/Lumenwake/Water/` |
| P1 | Sound/VFX | Water combat, shell/metal impacts, tide gate ambience, local wind, pressure-engine loops. | `Assets/Audio/Lumenwake/`, `Assets/VFX/Lumenwake/` |

## Production sequence

The slice starts with a visual validation scene containing only the hero, one Brine Tax enemy, a reefstone terrace, a dock, lagoon water, and the Breathing Eye landmark. The camera must be validated against a captured gameplay image before quest systems or boss scripting are added. Once the base silhouette and material response pass review, the environment is expanded in three rings: player route, combat landmark, and distant skyline.

The existing `PrototypeSceneBootstrap`, `CinematicIslandBuilder`, generated terrain, primitive palm generator, concept-panel fallback, and duplicate Vanguard enemy path are temporary compatibility code. They must not remain in the final Lumenwake scene. Gameplay components may be retained only after being attached to authored prefabs and Animator Controllers.
