using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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
            SeasOfLegends.World.CinematicIslandBuilder.Build(transform);
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
            Vector3 playerSpawn = new Vector3(-5.5f, 0f, -17f);
            playerSpawn.y = SampleTerrainHeight(playerSpawn) + 0.04f;
            player.transform.position = playerSpawn;
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

            Animator playerPresentation = CreateCharacterVisual(player.transform, "Tide Warden Model", playerColor, 1.1f, "Art/Characters/tide_warden", "Models/Characters/VanguardHero");
            if (playerPresentation != null) controller.SetPresentationAnimator(playerPresentation);
            CreateWeaponHitbox(player.transform, "Tide Warden Blade", new Vector3(0.25f, 1.1f, 0.7f), playerColor);

            // Camera is created after the player, then assigned in CreateCamera before Start runs.
            controller.ConfigureForPrototype(playerDefinition, combatSystem, null);
            return player.transform;
        }

        private Transform CreateEnemy(Transform player, CombatSystem combatSystem)
        {
            AttackDefinition enemyAttack = CreateAttack("raider_slash", AttackInput.Light, 9f, 12, 4, 18);
            GameObject enemy = new GameObject("Enemy - Crimson Raider");
            Vector3 enemySpawn = new Vector3(1.5f, 0f, 4f);
            enemySpawn.y = SampleTerrainHeight(enemySpawn) + 0.04f;
            enemy.transform.position = enemySpawn;
            CapsuleCollider capsule = enemy.AddComponent<CapsuleCollider>();
            capsule.height = 2f;
            capsule.radius = 0.42f;
            capsule.center = new Vector3(0f, 1f, 0f);
            enemy.AddComponent<Animator>();
            enemy.AddComponent<UnityEngine.AI.NavMeshAgent>();
            Combatant combatant = enemy.AddComponent<Combatant>();
            combatant.ConfigureForPrototype(85f);
            EnemyController controller = enemy.AddComponent<EnemyController>();

            Animator enemyPresentation = CreateCharacterVisual(enemy.transform, "Crimson Raider Model", enemyColor, 1.12f, "Art/Characters/crimson_raider", "Models/Characters/VanguardHero");
            if (enemyPresentation != null) controller.SetPresentationAnimator(enemyPresentation);
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

        private float SampleTerrainHeight(Vector3 position)
        {
            Terrain terrain = Terrain.activeTerrain;
            return terrain != null ? terrain.SampleHeight(position) + terrain.transform.position.y : 0f;
        }

        private AttackDefinition CreateAttack(string id, AttackInput input, float damage, int startup, int active, int recovery)
        {
            AttackDefinition definition = ScriptableObject.CreateInstance<AttackDefinition>();
            definition.ConfigureForPrototype(id, input, damage, startup, active, recovery);
            return definition;
        }

        private Animator CreateCharacterVisual(Transform root, string visualName, Color color, float height, string portraitResourcePath, string riggedModelResourcePath)
        {
            GameObject riggedModel = Resources.Load<GameObject>(riggedModelResourcePath);
            if (riggedModel != null)
            {
                GameObject modelInstance = Instantiate(riggedModel, root);
                modelInstance.name = visualName + " Rigged Humanoid";
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                modelInstance.transform.localScale = Vector3.one;
                ApplyCharacterAccent(modelInstance, color);
                Animator modelAnimator = modelInstance.GetComponentInChildren<Animator>();
                RuntimeAnimatorController locomotionController = Resources.Load<RuntimeAnimatorController>("Models/Animations/VanguardLocomotion");
                if (modelAnimator != null && locomotionController != null)
                {
                    modelAnimator.runtimeAnimatorController = locomotionController;
                    modelAnimator.applyRootMotion = false;
                }
                return modelAnimator;
            }

            // Art-panel fallback remains available only when the real FBX model has not imported.
            GameObject body = CreatePrimitive(PrimitiveType.Capsule, visualName, root.position, Vector3.one * height, color);
            body.transform.SetParent(root);
            body.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            Collider visualCollider = body.GetComponent<Collider>();
            if (visualCollider != null) Destroy(visualCollider);

            Texture2D portrait = Resources.Load<Texture2D>(portraitResourcePath);
            if (portrait == null) return null;
            GameObject portraitPanel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            portraitPanel.name = visualName + " Concept Art";
            portraitPanel.transform.SetParent(root);
            portraitPanel.transform.localPosition = new Vector3(0f, 1.5f, 0.12f);
            portraitPanel.transform.localScale = new Vector3(1.7f, 2.55f, 1f);
            Collider portraitCollider = portraitPanel.GetComponent<Collider>();
            if (portraitCollider != null) Destroy(portraitCollider);
            Renderer portraitRenderer = portraitPanel.GetComponent<Renderer>();
            portraitRenderer.material = CreateTransparentMaterial(portrait);
            portraitPanel.AddComponent<CharacterArtBillboard>();
            return null;
        }

        private void ApplyCharacterAccent(GameObject modelInstance, Color accent)
        {
            Renderer[] renderers = modelInstance.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    if (materials[j].HasProperty("_Color"))
                        materials[j].color = Color.Lerp(materials[j].color, accent, 0.18f);
                }
            }
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

        private void ApplyTexture(Renderer renderer, string resourcePath, Vector2 tiling)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (renderer == null || texture == null) return;
            renderer.material.mainTexture = texture;
            renderer.material.mainTextureScale = tiling;
        }

        private Material CreateTransparentMaterial(Texture2D texture)
        {
            Shader shader = Shader.Find("Unlit/Transparent");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            Material material = new Material(shader);
            material.mainTexture = texture;
            return material;
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
            // A project can import URP without assigning a pipeline asset. In that case a URP
            // shader renders magenta in Unity's built-in pipeline, so select by active pipeline.
            bool usesScriptableRenderPipeline = GraphicsSettings.currentRenderPipeline != null;
            Shader shader = usesScriptableRenderPipeline
                ? Shader.Find("Universal Render Pipeline/Lit")
                : Shader.Find("Standard");

            // Keep the prototype visible even if an unexpected SRP is configured.
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Unlit/Color");

            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}
