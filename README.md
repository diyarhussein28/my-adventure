# Seas of Legends

**Seas of Legends** is a clean Unity 2022.3 LTS foundation for a stylized 3D action-adventure fighting RPG. It separates runtime logic into data, input, player state, combat, world-streaming, camera, and quest layers so the project can grow from a playable vertical slice into a large ocean-world game without collapsing into a monolithic controller.

> This repository is now a single Unity project rooted at the repository root. The former duplicate asset trees and placeholder bootstrap scripts were removed because they were structurally inconsistent and contained incompatible prototype implementations.

## Project layout

```text
.
├── Assets/
│   ├── Animations/                         # Animator controllers and clips
│   ├── Audio/                              # Music, SFX, mixer assets
│   ├── Input/Adventure.inputactions        # New Input System action asset
│   ├── Materials/                          # URP materials and shaders
│   ├── Models/                             # Imported character and environment meshes
│   ├── Prefabs/
│   │   ├── Characters/                     # Player, NPC, and enemy prefabs
│   │   ├── Combat/                         # Weapon hitbox and VFX prefabs
│   │   ├── UI/                             # HUD and menu prefabs
│   │   └── World/                          # Island and ocean prefabs
│   ├── Scenes/                             # Boot, OceanWorld, Island, and Arena scenes
│   ├── ScriptableObjects/
│   │   ├── Characters/                     # CharacterDefinition assets
│   │   ├── Combat/                         # AttackDefinition, ComboDefinition, and WeaponDefinition assets
│   │   ├── Quests/                         # QuestDefinition assets
│   │   └── World/                          # IslandDefinition and IslandBiomeDefinition assets
│   ├── Scripts/
│   │   ├── Camera/                         # Cinemachine mode director
│   │   ├── Combat/                         # Hitboxes, hit-stop, combos, combatants
│   │   ├── Core/                           # Decoupled event contracts
│   │   ├── Data/                           # ScriptableObject schema types
│   │   ├── Input/                          # New Input System adapter
│   │   ├── Player/                         # Rigidbody motor and hierarchical FSM
│   │   ├── Quest/                          # Quest log and narrative signals
│   │   └── World/                          # Island streaming
│   └── VFX/                                # VFX Graph assets and particle textures
├── Docs/ARCHITECTURE.md                    # Detailed architectural decision record
├── Packages/manifest.json                  # Required Unity packages
└── ProjectSettings/ProjectVersion.txt      # Unity 2022.3 LTS baseline
```

## Play the starter island

Open `Assets/Scenes/StarterIsland.unity` and press **Play**. The scene is also registered as the build entry point. It creates a tropical island, a controllable Tide Warden, a pursuing Crimson Raider, camera, HUD, and a two-hit combat loop without requiring manual prefab setup. See [`Docs/PLAYABLE_SLICE.md`](Docs/PLAYABLE_SLICE.md) for the exact controls and a replacement path from prototype primitives to final game assets.

## Open the project

Open the repository root in **Unity Hub** using Unity **2022.3.20f1** or a compatible 2022.3 LTS editor. Unity restores the declared Input System, Cinemachine, URP, and VFX Graph packages from `Packages/manifest.json`. In **Project Settings → Player**, ensure **Active Input Handling** is set to **Input System Package (New)** or **Both**.

| Area | Required setup | Important details |
| --- | --- | --- |
| Player prefab | `Rigidbody`, `CapsuleCollider`, `Animator`, `PlayerInput`, `PlayerInputReader`, `PlayerController`, `Combatant`, `ComboManager` | Freeze Rigidbody rotation. Configure `PlayerInput` to use `Adventure.inputactions` and **Invoke Unity Events**. |
| Weapon child | Trigger `Collider` and `Hitbox` | Keep the collider disabled in the prefab. `CombatSystem` activates it only in active frames. |
| Combat systems object | `CombatSystem` | Drag this reference into `PlayerController`. |
| Camera rig | Main Camera + `CinemachineBrain`, then exploration, combat, arena, and cinematic virtual cameras | Assign each camera in `AdventureCameraDirector`. |
| Ocean systems object | `IslandManager` | Assign ship/player as `streamingFocus` and author one `IslandDefinition` per island. |
| Quest systems object | `QuestManager` | Populate its catalog with `QuestDefinition` assets. |

## Player state machine

The player uses reusable states rather than allocating a new state on each transition. `Locomotion` and `Airborne` are the movement states, while `Dashing`, `WallRunning`, `Attacking`, `Blocking`, `Stunned`, and `Executing` are focused action states. `PlayerController` owns physics, ground/wall sensing, animation parameters, and state-machine lifetime; state classes decide **when** to transition, not how to access raw components.

The movement motor uses velocity control rather than moving transforms directly. Its planar acceleration is the bounded velocity change `Δv = a × Δt`, implemented with `Vector3.MoveTowards`. This preserves collision response while ensuring player tuning values retain physical units: speed is metres/second and acceleration is metres/second². Extra falling gravity applies only when airborne, and vertical speed is clamped to keep jump arcs stable.

## Frame-data combat

Each `AttackDefinition` stores startup, active, and recovery frames at 60 FPS. A time value is computed as `frames ÷ 60`; for example, an 8-frame startup becomes `8 / 60 = 0.133 seconds`. During startup the hitbox remains disabled, during active frames it is armed, and during recovery the next valid input can be buffered by `ComboManager`.

Damage is data-driven. Combo scaling uses `baseDamage × comboScale^(hitCount - 1)`, so long strings naturally decay without special cases. Each armed `Hitbox` stores targets it already hit, preventing a multi-collider enemy from receiving duplicate damage. On a confirmed hit, `CombatSystem` briefly changes `Time.timeScale` while waiting in realtime; this produces impact emphasis without extending the pause indefinitely.

## World, camera, and narrative

`IslandManager` uses squared distance and separate load/unload radii. Squared distances avoid a square-root per streamed island, while hysteresis prevents rapid instantiate/destroy cycles if the ship hovers near a boundary. The current implementation uses prefabs and is intentionally isolated behind `Load` and `Unload`; the production migration path is to substitute Addressables or additive scenes there.

`AdventureCameraDirector` selects one high-priority Cinemachine virtual camera for exploration, locked combat, arena duels, or cinematic executions. `QuestManager` tracks prerequisite gates and individual objective amounts, emitting dialogue keys and global completion events instead of hard-wiring UI or NPC references.

## Controls

| Action | Keyboard / Mouse | Gamepad recommendation |
| --- | --- | --- |
| Move | WASD | Left stick |
| Look | Mouse | Right stick |
| Jump | Space | South button |
| Dash | Left Shift | Right trigger / shoulder |
| Block | Right mouse | Left trigger |
| Light attack | Left mouse | West button |
| Heavy attack | Q | North button |
| Special attack | E | East button |
| Interact | F | South button |

The included `.inputactions` file gives keyboard and initial gamepad bindings. Complete the gamepad bindings and connect every action to the matching public callback on `PlayerInputReader` through the PlayerInput Unity Events inspector.

## Implementation map

| Deliverable | Source files |
| --- | --- |
| Architecture and patterns | `Docs/ARCHITECTURE.md`, `Assets/Scripts/Core/GameEvents.cs`, `Assets/Scripts/Data/WeaponAndBiomeDefinitions.cs` |
| Player FSM and movement | `Assets/Scripts/Player/PlayerController.cs`, `PlayerState.cs`, `PlayerStateMachine.cs`, `PlayerStates.cs` |
| Combat, combo buffer, hitboxes, hit-stop | `Assets/Scripts/Combat/CombatSystem.cs`, `ComboManager.cs`, `Hitbox.cs`, `Combatant.cs`, `Assets/Scripts/Data/AttackDefinition.cs` |
| Starter island vertical slice | `Assets/Scenes/StarterIsland.unity`, `Assets/Scripts/Core/PrototypeSceneBootstrap.cs`, `Docs/PLAYABLE_SLICE.md` |
| Island streaming | `Assets/Scripts/World/IslandManager.cs`, `Assets/Scripts/Data/WorldAndQuestDefinitions.cs` |
| Narrative and quests | `Assets/Scripts/Quest/QuestManager.cs`, `Assets/Scripts/Data/WorldAndQuestDefinitions.cs` |
| Input and camera | `Assets/Scripts/Input/PlayerInputReader.cs`, `Assets/Scripts/Camera/AdventureCameraDirector.cs` |

## References

[1]: https://docs.unity3d.com/Manual/com.unity.inputsystem.html "Unity Input System documentation"
[2]: https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/index.html "Cinemachine 2.9 documentation"
[3]: https://docs.unity3d.com/Manual/VisualEffectGraph.html "Unity Visual Effect Graph documentation"

The package and setup choices in this repository align with the Unity documentation for the Input System, Cinemachine, and VFX Graph.[1] [2] [3]
