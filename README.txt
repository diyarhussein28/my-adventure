Unity Game Project - Ready to Play

To test the game:
1. Open the Unity Editor (2020.3 or later recommended).
2. Open the scene: Assets/Scenes/TestBootstrap.unity
3. Press Play.

Controls:
- WASD / Left Stick: Move
- Space / Bottom Button: Jump
- Left Shift / Right Trigger: Dash (when moving)
- Left Mouse Button / Right Trigger (or configured attack button): Attack
- Right Mouse Button / Left Trigger (hold): Block

The game includes:
- Player with high-speed movement, dashing, jumping, blocking, and basic attack.
- Simple enemy AI that patrols, chases, and attacks.
- Basic combat system with hit detection, hit pause (time slowdown), and camera shake.
- Island streaming system for open-world ocean (procedural island placement).
- Quest manager framework (example quest can be created via Assets -> Create Sample Game Data).
- VFX manager for hit effects (assign prefabs in inspector for better visuals).

To create sample assets:
- Use the top menu: Assets -> Create Sample Game Data
  - Island Data (Tropical/Volcanic)
  - Quest Data (Tutorial)

Assign the created IslandData.asset to the IslandManager in the inspector (or let the Bootstrap create a default manager and assign the island prefab in the IslandData).

For better visuals, assign:
- VFX Manager: hit impact, spark, blood, weapon trail prefabs.
- Camera Controller: assign freeLookCamera and combatCamera (Cinemachine Virtual Cameras).
- Player and Enemy: assign proper models and Animator Controllers.

Note: The Bootstrap script creates placeholder player and enemy meshes if no prefabs are assigned. It also creates default managers if prefabs are not assigned.

Enjoy building your epic adventure!