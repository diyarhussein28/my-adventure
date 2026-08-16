using UnityEngine;
using UnityEngine.InputSystem;
using SeasOfLegends.AI;
using SeasOfLegends.CameraSystem;
using SeasOfLegends.Combat;
using SeasOfLegends.Data;
using SeasOfLegends.Input;
using SeasOfLegends.Player;
using SeasOfLegends.UI;

namespace SeasOfLegends.Core
{
    /// <summary>
    /// A deliberately self-contained first playable scene. Attach this script to the root object
    /// in `StarterIsland.unity`; it creates primitive visual placeholders and wires the gameplay
    /// systems at runtime, so the project can be played before final art and prefabs are authored.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class PrototypeSceneBootstrap : MonoBehaviour
    {
        private readonly Color oceanColor = new Color(0.02f, 0.21f, 0.36f);
        private readonly Color islandColor = new Color(0.19f, 0.42f, 0.16f);
        private readonly Color sandColor = new Color(0.82f, 0.67f, 0.33f);
        private readonly Color playerColor = new Color(0.18f, 0.66f, 0.94f);
        private readonly Color enemyColor = new Color(0.85f, 0.18f, 0.21f);

        private void Awake()
        {
            if (FindObjectOfType<PlayerController>() != null) return;
            BuildEnvironment();
            CombatSystem combatSystem = CreateSystems();
            Transform player = CreatePlayer(combatSystem);
            Transform enemy = CreateEnemy(player, combatSystem);
            CreateCamera(player);
            CreateHud(player.GetComponent<Combatant>(), enemy.GetComponent<Combatant>());
        }

        private void BuildEnvironment()
        {
            RenderSettings.ambientLight = new Color(0.47f, 0.62f, 0.74f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.36f, 0.63f, 0.77f);
            RenderSettings.fogDensity = 0.012f;

            GameObject lightObject = new GameObject("Sun");
            Light sun = lightObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(48f, -30f, 0f);

            GameObject ocean = CreatePrimitive(PrimitiveType.Plane, "Ocean", Vector3.zero, new Vector3(15f, 1f, 15f), oceanColor);
            ocean.transform.SetParent(transform);
            GameObject island = CreatePrimitive(PrimitiveType.Cylinder, "Starter Island", new Vector3(0f, 0.35f, 0f), new Vector3(7.5f, 0.35f, 7.5f), islandColor);
            island.transform.SetParent(transform);
            GameObject beach = CreatePrimitive(PrimitiveType.Cylinder, "Beach", new Vector3(0f, 0.39f, 0f), new Vector3(8.2f, 0.06f, 8.2f), sandColor);
            beach.transform.SetParent(transform);

            CreateRock(new Vector3(-3.5f, 1.1f, 2.4f), new Vector3(1.4f, 2.2f, 1.1f));
            CreateRock(new Vector3(3.7f, 1f, -2.8f), new Vector3(1.7f, 2f, 1.2f));
            CreatePalm(new Vector3(-4.5f, 0.7f, -1.8f), 2.5f);
            CreatePalm(new Vector3(4.1f, 0.7f, 2.1f), 2.2f);
            CreatePalm(new Vector3(1.6f, 0.7f, 4.7f), 1.9f);
        }

        private CombatSystem CreateSystems()
        {
            GameObject systems = new GameObject("Combat Systems");
            systems.transform.SetParent(transform);
            return systems.AddComponent<CombatSystem>();
        }

        private Transform CreatePlayer(CombatSystem combatSystem)
        {
            CharacterDefinition playerDefinition = ScriptableObject.CreateInstance<CharacterDefinition>();
            AttackDefinition lightOne = CreateAttack("tide_cut_1", AttackInput.Light, 13f, 7, 4, 12);
            AttackDefinition lightTwo = CreateAttack("tide_cut_2", AttackInput.Light, 16f, 8, 4, 14);
            ComboDefinition combo = ScriptableObject.CreateInstance<ComboDefinition>();
            combo.ConfigureForPrototype("tide_cut", new[] { lightOne, lightTwo });

            GameObject player = new GameObject("Player - Tide Warden");
            player.transform.position = new Vector3(0f, 0.72f, -4.3f);
            Rigidbody body = player.AddComponent<Rigidbody>();
            body.mass = 70f;
            body.constraints = RigidbodyConstraints.FreezeRotation;
            CapsuleCollider capsule = player.AddComponent<CapsuleCollider>();
            capsule.height = 2f;
            capsule.radius = 0.38f;
            capsule.center = new Vector3(0f, 1f, 0f);
            Animator animator = player.AddComponent<Animator>();
            player.AddComponent<PlayerInput>();
            player.AddComponent<PlayerInputReader>();
            Combatant combatant = player.AddComponent<Combatant>();
            combatant.ConfigureForPrototype(120f, 0.05f);
            ComboManager comboManager = player.AddComponent<ComboManager>();
            comboManager.ConfigureForPrototype(new[] { combo });
            PlayerController controller = player.AddComponent<PlayerController>();

            CreateCharacterVisual(player.transform, "Tide Warden Model", playerColor, 1.1f);
            CreateWeaponHitbox(player.transform, "Tide Warden Blade", new Vector3(0.25f, 1.1f, 0.7f), playerColor);

            // Camera is created after the player, then assigned in CreateCamera before Start runs.
            controller.ConfigureForPrototype(playerDefinition, combatSystem, null);
            return player.transform;
        }

        private Transform CreateEnemy(Transform player, CombatSystem combatSystem)
        {
            AttackDefinition enemyAttack = CreateAttack("raider_slash", AttackInput.Light, 9f, 12, 4, 18);
            GameObject enemy = new GameObject("Enemy - Crimson Raider");
            enemy.transform.position = new Vector3(0f, 0.72f, 3.5f);
            CapsuleCollider capsule = enemy.AddComponent<CapsuleCollider>();
            capsule.height = 2f;
            capsule.radius = 0.42f;
            capsule.center = new Vector3(0f, 1f, 0f);
            enemy.AddComponent<Animator>();
            enemy.AddComponent<UnityEngine.AI.NavMeshAgent>();
            Combatant combatant = enemy.AddComponent<Combatant>();
            combatant.ConfigureForPrototype(85f);
            EnemyController controller = enemy.AddComponent<EnemyController>();

            CreateCharacterVisual(enemy.transform, "Crimson Raider Model", enemyColor, 1.12f);
            CreateWeaponHitbox(enemy.transform, "Raider Cutlass", new Vector3(-0.25f, 1.05f, 0.75f), enemyColor);
            controller.ConfigureForPrototype(player, combatSystem, enemyAttack);
            return enemy.transform;
        }

        private void CreateCamera(Transform player)
        {
            GameObject cameraObject = new GameObject("Adventure Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 62f;
            cameraObject.AddComponent<AudioListener>();
            ThirdPersonCameraRig rig = cameraObject.AddComponent<ThirdPersonCameraRig>();
            rig.ConfigureForPrototype(player);

            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null) playerController.SetCameraTransform(cameraObject.transform);
        }

        private void CreateHud(Combatant player, Combatant enemy)
        {
            GameObject hud = new GameObject("Vertical Slice HUD");
            PrototypeHud display = hud.AddComponent<PrototypeHud>();
            display.ConfigureForPrototype(player, enemy);
        }

        private AttackDefinition CreateAttack(string id, AttackInput input, float damage, int startup, int active, int recovery)
        {
            AttackDefinition definition = ScriptableObject.CreateInstance<AttackDefinition>();
            definition.ConfigureForPrototype(id, input, damage, startup, active, recovery);
            return definition;
        }

        private void CreateCharacterVisual(Transform root, string visualName, Color color, float height)
        {
            GameObject body = CreatePrimitive(PrimitiveType.Capsule, visualName, root.position, Vector3.one * height, color);
            body.transform.SetParent(root);
            body.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            Collider visualCollider = body.GetComponent<Collider>();
            if (visualCollider != null) Destroy(visualCollider);
        }

        private void CreateWeaponHitbox(Transform root, string weaponName, Vector3 localPosition, Color color)
        {
            GameObject weapon = CreatePrimitive(PrimitiveType.Cube, weaponName, root.position, new Vector3(0.15f, 0.15f, 0.9f), color);
            weapon.transform.SetParent(root);
            weapon.transform.localPosition = localPosition;
            weapon.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);
            BoxCollider collider = weapon.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            weapon.AddComponent<Hitbox>();
        }

        private void CreateRock(Vector3 position, Vector3 scale)
        {
            GameObject rock = CreatePrimitive(PrimitiveType.Sphere, "Island Rock", position, scale, new Color(0.26f, 0.28f, 0.31f));
            rock.transform.SetParent(transform);
        }

        private void CreatePalm(Vector3 position, float height)
        {
            GameObject trunk = CreatePrimitive(PrimitiveType.Cylinder, "Palm Tree", position + Vector3.up * height * 0.5f, new Vector3(0.22f, height * 0.5f, 0.22f), new Color(0.3f, 0.18f, 0.08f));
            trunk.transform.SetParent(transform);
            for (int i = 0; i < 5; i++)
            {
                GameObject leaf = CreatePrimitive(PrimitiveType.Cube, "Palm Leaf", position + Vector3.up * height, new Vector3(1.4f, 0.08f, 0.26f), new Color(0.11f, 0.48f, 0.16f));
                leaf.transform.SetParent(trunk.transform);
                leaf.transform.rotation = Quaternion.Euler(20f, i * 72f, 0f);
            }
        }

        private GameObject CreatePrimitive(PrimitiveType type, string objectName, Vector3 position, Vector3 scale, Color color)
        {
            GameObject primitive = GameObject.CreatePrimitive(type);
            primitive.name = objectName;
            primitive.transform.position = position;
            primitive.transform.localScale = scale;
            Renderer renderer = primitive.GetComponent<Renderer>();
            renderer.material = CreateMaterial(color);
            return primitive;
        }

        private Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}
