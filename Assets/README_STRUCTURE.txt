UnityGameProject/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/               # Singletons, managers, event systems, player states
│   │   │   ├── PlayerState.cs
│   │   │   ├── PlayerController.cs
│   │   │   ├── LocomotionState.cs
│   │   │   ├── DashingState.cs
│   │   │   ├── AirborneState.cs
│   │   │   ├── AttackingState.cs
│   │   │   ├── StunnedState.cs
│   │   │   ├── BlockingState.cs
│   │   │   ├── ExecutingState.cs
│   │   │   ├── Health.cs
│   │   │   └── (other core systems)
│   │   ├── Combat/             # Combat logic, hitboxes, combo systems
│   │   │   ├── ComboManager.cs
│   │   │   ├── Hitbox.cs
│   │   │   ├── Hurtbox.cs
│   │   │   └── CombatSystem.cs
│   │   ├── WorldGeneration/    # Island generation, streaming, ocean system
│   │   │   └── IslandManager.cs
│   │   ├── Quest/              # Quest tracking, dialogue, NPC interactions
│   │   │   └── QuestManager.cs
│   │   ├── Input/              # Input system wrappers and actions (using New Input System)
│   │   └── Camera/             # Cinemachine setup and camera controllers
│   ├── Prefabs/                # Reusable prefabs (characters, weapons, islands, etc.)
│   ├── Models/                 # 3D character and environment models
│   ├── Animations/             # Animation controllers and clips
│   ├── Materials/              # URP materials and shaders
│   └── ScriptableObjects/
│       ├── Character/          # Stats, combo strings, weapon profiles
│       ├── Combat/             # Damage profiles, hitbox definitions
│       ├── World/              # Island biome data, spawn tables
│       │   └── IslandData.cs
│       └── Quest/              # Quest definitions, dialogue trees
│           └── QuestData.cs