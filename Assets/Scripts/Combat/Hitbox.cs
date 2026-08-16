using UnityEngine;

/// <summary>
/// Simple hitbox component for weapons. Detects overlaps with hurtboxes.
/// Place on weapon colliders (trigger) to register hits.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 5f;
    public Vector3 knockbackDirection = Vector3.up; // Default upward knockback
    public bool canCauseHitPause = true;
    public float hitPauseDuration = 0.05f; // Seconds to pause time
    public float hitPauseScale = 0.01f; // Time scale during pause (0.01 = 1% normal speed)

    // Event called when this hitbox hits a hurtbox
    public System.Action<Hitbox, Hurtbox> OnHit;

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && hurtbox.IsActive)
        {
            OnHit?.Invoke(this, hurtbox);

            // Spawn VFX at point of impact
            // We approximate impact point as the closest point on the hitbox collider to the hurtbox
            // For simplicity, we use the hitbox's position (could be improved)
            Vector3 impactPoint = transform.position;
            Vector3 impactNormal = -transform.forward; // Assuming hitbox faces forward

            // Call VFX manager if available
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.ProcessHitVFX(this, hurtbox, impactPoint, impactNormal);
            }
        }
    }
}