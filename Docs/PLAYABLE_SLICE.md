# Starter Island Vertical Slice

`Assets/Scenes/StarterIsland.unity` is now the default playable scene. It contains one bootstrap component; pressing **Play** builds the island, player, enemy, UI, camera, and combat data in memory. This makes the first slice runnable without manually assembling starter prefabs or assets.

## Included gameplay loop

| Step | Player experience | Implemented system |
| --- | --- | --- |
| 1 | Spawn on a stylized tropical island surrounded by an ocean plane | `PrototypeSceneBootstrap` environment generation |
| 2 | Use a smooth third-person camera while moving and jumping | `ThirdPersonCameraRig` and `PlayerController` |
| 3 | Approach the Crimson Raider, who pursues and attacks | `EnemyController` with NavMesh-free pursuit fallback |
| 4 | Chain two light attacks, block, and dash through the encounter | `ComboManager`, `CombatSystem`, `Hitbox`, and player FSM |
| 5 | Defeat the enemy and see the completion state in the HUD | `Combatant` and `PrototypeHud` |

## Controls

| Action | Control |
| --- | --- |
| Move | WASD |
| Look | Mouse |
| Jump | Space |
| Dash | Left Shift |
| Light attack / continue combo | Left mouse button |
| Block | Right mouse button |
| Heavy / special / ultimate test input | Q / E / R |
| Release mouse cursor | Escape |

## Run instructions

Open the repository root in Unity Hub with Unity **2022.3 LTS**. Let Unity import packages, open `Assets/Scenes/StarterIsland.unity`, and press **Play**. The same scene is registered in `ProjectSettings/EditorBuildSettings.asset`, so it is also the build entry point.

## Replacement plan

The bootstrap intentionally uses runtime primitive meshes and transient ScriptableObject instances. Replace these incrementally, without changing the core gameplay contracts:

1. Replace primitive island objects with authored URP environment prefabs.
2. Replace `CreateCharacterVisual` with player and enemy prefabs holding Animator Controllers.
3. Replace runtime attack/combos with assets stored under `Assets/ScriptableObjects/Combat/`.
4. Replace `PrototypeHud` with UI Toolkit or uGUI while continuing to read `Combatant.CurrentHealth` and `MaximumHealth`.
5. Replace `ThirdPersonCameraRig` with the existing Cinemachine `AdventureCameraDirector` once virtual cameras and lock-on targets are authored.

The vertical slice is designed as a playable foundation, not final art direction. It establishes a complete movement-to-combat-to-victory loop while maintaining the modular architecture documented in `ARCHITECTURE.md`.
