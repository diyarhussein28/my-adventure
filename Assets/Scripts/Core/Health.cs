using UnityEngine;

/// <summary>
/// Simple health component. Can be placed on any GameObject that can take damage.
/// </summary>
public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public System.Action<int, int> OnHealthChanged; // current, max
    public System.Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Apply damage to this health component.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal this health component.
    /// </summary>
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return; // Can't heal dead
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        // Trigger death animation, disable components, etc.
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Disable collision and scripts
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Notify listeners
        OnDeath?.Invoke();

        // Optionally destroy after delay
        // Destroy(gameObject, 5f);
    }
}