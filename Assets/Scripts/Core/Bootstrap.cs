using UnityEngine;

/// <summary>
/// Bootstrap script to set up a basic test scene at runtime.
/// Attach to an empty GameObject in the initial scene (e.g., a scene called Bootstrap).
/// This will spawn the player, set up cameras, island manager, and quest manager.
/// </summary>
[DefaultExecutionOrder(-100)] // Run early
public class Bootstrap : MonoBehaviour
{
    [Header("References (assign in inspector or leave null to auto-create)")]
    public GameObject playerPrefab; // Prefab containing PlayerController, etc.
    public GameObject islandManagerPrefab;
    public GameObject questManagerPrefab;
    public GameObject vfxManagerPrefab;
    public GameObject combatSystemPrefab;
    public GameObject cameraControllerPrefab;

    [Header("Fallbacks")]
    public Material defaultMaterial; // For creating simple shapes

    private void Awake()
    {
        // Ensure only one bootstrap exists
        if (FindObjectsOfType<Bootstrap>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject); // Persist if we load other scenes

        CreateManagers();
        SpawnPlayer();
        SpawnEnemy(); // Add a simple enemy for testing
        SetupCamera();
        Debug.Log("Bootstrap complete. Basic systems ready.");
    }

    private void CreateManagers()
    {
        // Helper to instantiate if prefab assigned, otherwise create empty GameObject with component
        void CreateIfMissing(GameObject prefab, System.Type componentType, string name)
        {
            GameObject obj;
            if (prefab != null)
            {
                obj = Instantiate(prefab);
                obj.name = prefab.name;
            }
            else
            {
                obj = new GameObject(name);
                obj.AddComponent(componentType);
            }
            DontDestroyOnLoad(obj); // Keep across scene loads if desired
        }

        if (islandManagerPrefab != null)
            CreateIfMissing(islandManagerPrefab, typeof(IslandManager), "IslandManager");
        else
        {
            var go = new GameObject("IslandManager");
            go.AddComponent<IslandManager>();
            DontDestroyOnLoad(go);
        }

        if (questManagerPrefab != null)
            CreateIfMissing(questManagerPrefab, typeof(QuestManager), "QuestManager");
        else
        {
            var go = new GameObject("QuestManager");
            go.AddComponent<QuestManager>();
            DontDestroyOnLoad(go);
        }

        if (vfxManagerPrefab != null)
            CreateIfMissing(vfxManagerPrefab, typeof(VFXManager), "VFXManager");
        else
        {
            var go = new GameObject("VFXManager");
            go.AddComponent<VFXManager>();
            DontDestroyOnLoad(go);
        }

        if (combatSystemPrefab != null)
            CreateIfMissing(combatSystemPrefab, typeof(CombatSystem), "CombatSystem");
        else
        {
            var go = new GameObject("CombatSystem");
            go.AddComponent<CombatSystem>();
            DontDestroyOnLoad(go);
        }

        if (cameraControllerPrefab != null)
            CreateIfMissing(cameraControllerPrefab, typeof(CameraController), "CameraController");
        else
        {
            var go = new GameObject("CameraController");
            go.AddComponent<CameraController>();
            DontDestroyOnLoad(go);
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, new Vector3(0, 1f, 0), Quaternion.identity);
            return;
        }

        // Create a simple player placeholder
        GameObject player = new GameObject("Player");
        player.tag = "Player";

        // Add components
        var controller = player.AddComponent<PlayerController>();
        var rb = player.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var capsule = player.AddComponent<CapsuleCollider>();
        capsule.height = 2f;
        capsule.radius = 0.5f;
        var animator = player.AddComponent<Animator>();
        // For placeholder, we assume an Animator Controller is assigned elsewhere; otherwise add a runtime controller?
        var health = player.AddComponent<Health>();
        var hurtbox = player.AddComponent<Hurtbox>();
        hurtbox.owner = player;

        // Add a simple weapon hitbox (as child)
        GameObject weapon = new GameObject("Weapon");
        weapon.transform.SetParent(player.transform);
        weapon.transform.localPosition = new Vector3(1f, 0f, 0f);
        weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        var weaponCol = weapon.AddComponent<BoxCollider>();
        weaponCol.isTrigger = true;
        weaponCol.size = new Vector3(1f, 0.5f, 0.5f);
        var hitbox = weapon.AddComponent<Hitbox>();
        hitbox.damage = 20;

        // Add a simple mesh for visibility
        var meshFilter = player.AddComponent<MeshFilter>();
        var meshRenderer = player.AddComponent<MeshRenderer>();
        meshFilter.mesh = CreateSimpleCapsuleMesh();
        meshRenderer.material = defaultMaterial != null ? defaultMaterial : new Material(Shader.Find("Standard"));

        DontDestroyOnLoad(player);
    }

    private void SpawnEnemy()
    {
        // Simple enemy placeholder
        GameObject enemy = new GameObject("Enemy");
        enemy.tag = "Enemy";

        // Add components
        var enemyAI = enemy.AddComponent<EnemyAI>();
        var rb = enemy.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var capsule = enemy.AddComponent<CapsuleCollider>();
        capsule.height = 2f;
        capsule.radius = 0.5f;
        var animator = enemy.AddComponent<Animator>();
        var health = enemy.AddComponent<Health>();
        var hurtbox = enemy.AddComponent<Hurtbox>();
        hurtbox.owner = enemy;

        // Add a simple weapon hitbox (as child)
        GameObject weapon = new GameObject("Weapon");
        weapon.transform.SetParent(enemy.transform);
        weapon.transform.localPosition = new Vector3(1f, 0f, 0f);
        weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        var weaponCol = weapon.AddComponent<BoxCollider>();
        weaponCol.isTrigger = true;
        weaponCol.size = new Vector3(1f, 0.5f, 0.5f);
        var hitbox = weapon.AddComponent<Hitbox>();
        hitbox.damage = 10;

        // Add a simple mesh for visibility
        var meshFilter = enemy.AddComponent<MeshFilter>();
        var meshRenderer = enemy.AddComponent<MeshRenderer>();
        meshFilter.mesh = CreateSimpleCapsuleMesh();
        meshRenderer.material = defaultMaterial != null ? defaultMaterial : new Material(Shader.Find("Standard"));
        // Tint enemy red for distinction
        if (meshRenderer.material != null)
        {
            meshRenderer.material.color = Color.red;
        }

        // Position enemy in front of player
        enemy.transform.position = new Vector3(3f, 1f, 0f);

        DontDestroyOnLoad(enemy);
    }

    private void SetupCamera()
    {
        // Ensure there is a Main Camera; if not, create one and attach free-look cinemachine setup
        if (Camera.main == null)
        {
            GameObject camObj = new GameObject("MainCamera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<Camera>();
            cam.backgroundColor = new Color(0.1f, 0.2f, 0.4f); // ocean-like
            // Add CinemachineBrain if package present
            // We'll assume it's added via the CameraController prefab or manually.
            DontDestroyOnLoad(camObj);
        }

        // Optionally set up a simple follow script if no Cinemachine
        // For brevity, we rely on the CameraController to manage Cinemachine cams.
    }
}