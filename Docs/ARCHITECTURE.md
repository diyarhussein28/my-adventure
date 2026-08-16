# Seas of Legends Architecture

## Intent

This repository provides the foundation for an open-ocean action RPG with responsive traversal, frame-data combat, elite arena duels, elemental effects, streamed islands, and quest-driven narrative. The architecture favours **small composition roots**, **ScriptableObject-authored data**, and **message-based boundaries** over global scene searches or giant manager classes.

> Runtime behaviour should be assembled from prefabs and data assets. C# scripts own rules and orchestration; they must not embed character-specific attack numbers, island positions, or quest copy.

## Runtime boundaries

| Layer | Responsibility | Primary types |
| --- | --- | --- |
| Data | Immutable authored parameters and content identity | `CharacterDefinition`, `AttackDefinition`, `ComboDefinition`, `IslandDefinition`, `QuestDefinition` |
| Input | Converts Input System actions to game intent | `PlayerInputReader` |
| Player | Sensing, Rigidbody motor, state transition policy, animator parameters | `PlayerController`, `PlayerStateMachine`, `PlayerState` subclasses |
| Combat | Frame phases, input buffering, hit confirmation, damage and hit-stop | `CombatSystem`, `ComboManager`, `Hitbox`, `Combatant` |
| World | Island loading lifecycle based on focus distance | `IslandManager` |
| Camera | Cinemachine priority selection and combat target framing | `AdventureCameraDirector` |
| Narrative | Quest state, prerequisite checks, objective updates, dialogue keys | `QuestManager` |
| Core | Loose coupling between systems through compact payloads | `GameEvents` |

## Design patterns

The framework applies a **state pattern** to character control. State objects are created once by `PlayerStateMachine`, then reused. This avoids per-frame allocations and gives each state one job: decide valid transitions and invoke controller-level motor methods. The `PlayerController` acts as a narrow façade over Rigidbody, Animator, and sensor data so states never duplicate physics setup.

The framework applies a **data-driven command pattern** to combat. An input maps to a `ComboDefinition`, which yields an `AttackDefinition`; the combat system then schedules that definition’s startup, active, and recovery windows. Replacing a combo asset changes combat content without changing combat code. A `Hitbox` reports collision candidates, while `Combatant` owns the result of receiving a hit.

The framework applies an **observer pattern** through `GameEvents`. Combat, streaming, and quest-completion messages are payloads rather than direct references to UI, audio, VFX, or analytics. Subscriber lifetime remains explicit: components subscribe in `OnEnable` and unsubscribe in `OnDisable` when they need events.

## State transition model

```text
Locomotion ──jump/fall──> Airborne ──land──> Locomotion
     │                         │
     ├──dash──> Dashing ───────┘
     ├──attack──> Attacking ───┘
     └──block──> Blocking ─────┘

Airborne ──wall contact + forward input──> WallRunning
WallRunning ──jump/release/timeout──> Airborne
Any actionable state ──confirmed incoming hit──> Stunned
Locomotion ──finisher trigger──> Executing ──timeline end──> Locomotion
```

The state machine is intentionally conservative: a state exits before the next state enters, and all temporary animation flags are cleared in `Exit`. This means a failed transition cannot leave the player blocking while dashing or with an armed weapon hitbox.

## Combat timing contract

| Phase | Duration source | Game behaviour |
| --- | --- | --- |
| Startup | `AttackDefinition.StartupFrames` | Animation wind-up; hitbox disabled |
| Active | `AttackDefinition.ActiveFrames` | Weapon `Hitbox` armed; each target can be hit once |
| Recovery | `AttackDefinition.RecoveryFrames` | Attack animation completes; combo input may be buffered |
| Hit-stop | `AttackDefinition.HitStopSeconds` | Controlled global time-scale slowdown on confirmed contact |

Attack values are authored in frames for design consistency and converted to seconds through `frames / 60`. Animation clips should match the authored total duration, or animation events can later call the same arm/disarm methods for even tighter synchronization. The current frame-data scheduler is deterministic relative to Unity’s scaled time; its realtime pause restoration avoids being trapped by an intentionally slowed time scale.

## Open-world streaming contract

An `IslandDefinition` identifies a prefab, world location, biome label, load radius, and unload radius. `IslandManager` polls the ship or player at a configurable interval. It instantiates when `distance² ≤ loadRadius²` and removes when `distance² ≥ unloadRadius²`. The margin between radii is hysteresis.

| Prototype phase | Production transition |
| --- | --- |
| Instantiate island prefab | Load Addressable prefab asynchronously |
| Destroy island prefab | Release Addressable instance after fade-out |
| One `IslandDefinition` per island | Add tiles/points of interest under each island root |
| Polling around ship transform | Add travel direction prefetch and memory budgets |

## Scene composition

Use a small persistent **Boot** scene to initialize audio, systems, and the Cinemachine brain, then load **OceanWorld** as the primary scene. Islands can be instantiated from streamed prefabs during the prototype stage. Elite encounters should be handled by an additive **ArenaDuel** scene or a local arena-volume that calls `AdventureCameraDirector.SetMode(CameraMode.Arena, bossTransform)`.

## Extension rules

New gameplay features should follow the existing boundary rather than attach unrelated responsibilities to `PlayerController` or `CombatSystem`. A ship motor belongs in `Scripts/World`, enemy state logic belongs in `Scripts/AI` when introduced, and VFX Graph launchers belong in `Scripts/VFX`. Any new content tuning value should first be evaluated as a ScriptableObject field.

## References

[1]: https://docs.unity3d.com/Manual/com.unity.inputsystem.html "Unity Input System documentation"
[2]: https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/index.html "Cinemachine 2.9 documentation"
[3]: https://docs.unity3d.com/Manual/class-ScriptableObject.html "Unity ScriptableObject documentation"

The boundary between authorable data and runtime logic is built around Unity ScriptableObject assets, while control and camera integrations target Unity’s documented Input System and Cinemachine workflows.[1] [2] [3]
