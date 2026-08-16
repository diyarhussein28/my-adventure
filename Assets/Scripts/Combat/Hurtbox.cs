using UnityEngine;

/// <summary>
/// Simple hurtbox component for characters. Receives hits from hitboxes.
/// Place on character colliders (typically same as main collider or separate).
/// </summary>
public class Hurtbox : MonoBehaviour
{
    public bool IsActive = true;
    public GameObject owner; // Reference to the character that owns this hurtbox

    // Event called when this hurtbox is hit
    public System.Action<Hitbox, Hurtbox> OnHit;

    public void TakeHit(Hitbox hitbox)
    {
        if (!IsActive) return;

        // Apply damage, knockback, etc. via CombatSystem
        CombatSystem.Instance.ProcessHit(hitbox, this);
        OnHit?.Invoke(hitbox, this);
    }

    // Called by Hitbox.OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            TakeHit(hitbox);
        }
    }

    // Optional: method to temporarily disable hurtbox (e.g., during invincibility)
    public void SetActive(bool active)
    {
        IsActive = active;
    }
}