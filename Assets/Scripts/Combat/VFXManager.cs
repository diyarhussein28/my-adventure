using UnityEngine;

/// <summary>
/// Simple manager for spawning VFX effects (e.g., hit impacts, weapon trails).
/// In a full implementation, this would use Unity VFX Graph or Particle Systems.
/// </summary>
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Hit Effects")]
    public GameObject hitImpactPrefab; // Assign a VFX prefab in inspector
    public GameObject hitSparkPrefab;  // Optional sparks
    public GameObject bloodPrefab;     // For organic enemies

    [Header("Weapon Effects")]
    public GameObject swordTrailPrefab; // Trail renderer for weapons

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Spawns a hit impact effect at the given position and normal.
    /// </summary>
    public void SpawnHitImpact(Vector3 position, Vector3 normal, bool isOrganic = false)
    {
        GameObject effectPrefab = isOrganic ? bloodPrefab : hitImpactPrefab;
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.LookRotation(normal));
            // Auto-destroy after duration (assumes VFX has a Destroy component or we can use Destroy after delay)
            Destroy(effect, 2f);
        }
        else
        {
            Debug.LogWarning("VFXManager: Hit impact prefab not assigned.");
        }
    }

    /// <summary>
    /// Spawns hit sparks (for metallic hits).
    /// </summary>
    public void SpawnHitSparks(Vector3 position, Vector3 normal)
    {
        if (hitSparkPrefab != null)
        {
            GameObject sparks = Instantiate(hitSparkPrefab, position, Quaternion.LookRotation(normal));
            Destroy(sparks, 1f);
        }
    }

    /// <summary>
    /// Spawns a weapon trail (to be parented to weapon).
    /// </summary>
    public GameObject SpawnWeaponTrail(Transform parent)
    {
        if (swordTrailPrefab != null)
        {
            return Instantiate(swordTrailPrefab, parent);
        }
        return null;
    }

    /// <summary>
    /// Called by Hitbox.OnHit to spawn appropriate VFX.
    /// </summary>
    public void ProcessHitVFX(Hitbox hitbox, Hurtbox hurtbox, Vector3 impactPoint, Vector3 impactNormal)
    {
        // Determine if target is organic (could check tag or component)
        bool isOrganic = hurtbox.CompareTag("Enemy") || hurtbox.CompareTag("Player"); // Simple example

        SpawnHitImpact(impactPoint, impactNormal, isOrganic);
        // Optionally sparks for metallic hits
        if (!isOrganic)
        {
            SpawnHitSparks(impactPoint, impactNormal);
        }
    }
}