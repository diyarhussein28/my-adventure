# SEAS OF LEGENDS - Architecture Overview

## Game Concept
A massive 3D Action-Adventure/Fighting RPG combining:
- **One Piece** → Open-world ocean exploration with unique islands
- **Mortal Kombat** → Frame-perfect combo-driven combat with arena duels
- **Demon Slayer** → Hyper-responsive acrobatic movement with elemental VFX

## Engine & Pipeline
- **Unity 2022.3 LTS** with Universal Render Pipeline (URP)
- **Cinemachine** for dynamic camera transitions
- **Unity New Input System** for cross-platform controls
- **Unity VFX Graph** for stylized elemental effects

---

## Design Patterns Used

### 1. Singleton Pattern (Managers)
Used for global systems that must exist exactly once:
- `GameManager` - Overall game state machine
- `EventManager` - Decoupled pub/sub event system
- `QuestManager` - Quest progression tracking
- `CombatSystem` - Central combat orchestration

### 2. State Machine Pattern (Player & AI)
Hierarchical Finite State Machine for predictable, maintainable behavior:
- `PlayerState` base class with `Enter/Execute/Exit`
- Concrete states: `LocomotionState`, `AirborneState`, `AttackingState`, `StunnedState`, `BlockingState`, `DashingState`, `WallRunState`
- Transition rules defined in `PlayerStateMachine`

### 3. Observer Pattern (Events)
`EventManager` using C# Actions for loose coupling:
- `OnPlayerDamaged`, `OnEnemyDefeated`, `OnQuestCompleted`
- Prevents direct references between unrelated systems

### 4. ScriptableObject Pattern (Data)
All configuration data stored as assets:
- `CharacterStatsSO` - HP, speed, attack power
- `ComboDataSO` - Move lists, frame data, inputs
- `WeaponDataSO` - Damage profiles, hitboxes, VFX references
- `IslandBiomeSO` - Terrain generation params, spawn tables

### 5. Object Pooling (Performance)
For projectiles, hit effects, and particle bursts in combat

### 6. Service Locator
Centralized access to core systems without singleton abuse

---

## Folder Structure

```
Assets/
├── Scripts/
│   ├── Core/              # Singletons, events, utilities, pooling
│   ├── Input/             # New Input System actions & wrapper
│   ├── Player/            # Player controller, state machine, movement
│   ├── Combat/            # Hitboxes, combos, frame data, combat manager
│   ├── AI/                # Enemy behavior trees, boss AI
│   ├── World/             # Ocean, islands, biome generation, weather
│   ├── Quest/             # Quest system, dialogue, narrative triggers
│   ├── Cameras/           # Cinemachine transitions, camera states
│   ├── VFX/               # Particle managers, time dilation, hit-stop
│   ├── UI/                # HUD, menus, damage numbers
│   ├── Data/              # ScriptableObject definitions
│   └── Inventory/         # Items, equipment, ship upgrades
├── Prefabs/
│   ├── Characters/        # Player, enemies, NPCs
│   ├── Combat/            # Hitbox prefabs, projectile prefabs
│   ├── World/             # Island chunks, ocean tiles, weather systems
│   └── UI/                # HUD elements, menus
├── ScriptableObjects/
│   ├── Characters/        # Stat profiles for all characters
│   ├── Weapons/           # Weapon definitions & combos
│   ├── Islands/           # Biome data, faction configs
│   └── Quests/            # Quest definitions, dialogue trees
├── Models/                # Character meshes, environment assets
├── Animations/            # Animator controllers, animation clips
├── Materials/             # URP shaders, stylized materials
├── VFX/                   # VFX Graph assets, particle textures
├── Scenes/
│   ├── Boot.unity         # Initialization scene
│   ├── MainMenu.unity
│   ├── OceanWorld.unity   # Main open world
│   └── ArenaDuel.unity    # Boss/elite encounter arenas
└── Resources/             # Runtime-loaded assets
```

---

## Scene Flow

```
Boot → MainMenu → OceanWorld (persistent)
                    ↓
              Island (additive load)
                    ↓
              ArenaDuel (async load for elite fights)
                    ↓
              Return to OceanWorld
```

## Camera States

| State | Behavior | Transition Trigger |
|-------|----------|-------------------|
| Exploration | Free-look 3rd person, wide FOV | Default in ocean/island |
| Combat | Lock-on to nearest threat | Enemy enters aggro range |
| Arena | 2.5D side-view, fixed bounds | Elite/boss encounter starts |
| Cinematic | Scripted path, letterbox | Finishing move / story moment |

---

## Combat Frame Data System

Every attack is defined by:
- **Startup Frames**: Wind-up, no hitbox active
- **Active Frames**: Hitbox active, can deal damage
- **Recovery Frames**: Vulnerable, cannot act
- **Block Advantage**: Frame advantage on block
- **Hit Advantage**: Frame advantage on hit
- **Damage**: Base damage × combo scaling
- **Hitstop**: Time freeze duration on impact

---

## Performance Strategy

1. **Island Streaming**: Load/unload island chunks based on ship distance
2. **LOD Groups**: Aggressive level-of-detail on environment meshes
3. **Occlusion Culling**: Pre-baked for island interiors
4. **GPU Instancing**: Ocean tiles, vegetation, crowds
5. **Async Loading**: Background thread for asset loading
6. **Object Pooling**: Combat effects, projectiles, debris
