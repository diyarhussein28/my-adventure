using UnityEngine;
using System.Collections;

/// <summary>
/// Central combat manager. Handles hit detection, hit pause, combo logic, and combat states.
/// Requires: References to main camera for shake effects (optional).
/// </summary>
public class CombatSystem : MonoBehaviour
{
    // Singleton
    public static CombatSystem Instance { get; private set; }

    // References
    private Camera mainCamera;

    // Hit pause handling
    private bool isInHitPause = false;
    private float hitPauseTimer;
    private Coroutine hitPauseCoroutine;

    // Combat state
    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isInExecution = false;

    // Configuration
    [Header("Hit Pause")]
    [Tooltip("Default hit pause duration if not overridden by hitbox")]
    public float defaultHitPauseDuration = 0.05f;
    [Tooltip("How much to slow time during hit pause (0 = freeze, 1 = normal)")]
    [Range(0f, 1f)] public float hitPauseTimeScale = 0.02f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.3f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes if needed

        mainCamera = Camera.main;
    }

    /// <summary>
    /// Called by AttackingState to start an attack sequence.
    /// </summary>
    public void StartAttack()
    {
        isAttacking = true;
        ComboManager.Instance.ClearBuffer(); // Reset combo buffer on new attack chain
    }

    /// <summary>
    /// Called to continue a combo after a successful hit.
    /// </summary>
    public void ContinueCombo()
    {
        // Logic to determine next attack in combo (could query animator or state)
        // For now, we just signal that combo is active
    }

    /// <summary>
    /// Called when attack sequence ends.
    /// </summary>
    public void EndAttack()
    {
        isAttacking = false;
    }

    /// <summary>
    /// Set blocking state (called by BlockingState).
    /// </summary>
    public void SetBlocking(bool blocking)
    {
        isBlocking = blocking;
    }

    /// <summary>
    /// Called when execution (finisher) starts.
    /// </summary>
    public void StartExecution()
    {
        isInExecution = true;
        // Lock camera, disable player input, etc.
    }

    /// <summary>
    /// Called when execution ends.
    /// </summary>
    public void EndExecution()
    {
        isInExecution = false;
    }

    /// <summary>
    /// Main method to process a hit between a hitbox and hurtbox.
    /// Called by Hurtbox.TakeHit.
    /// </summary>
    public void ProcessHit(Hitbox hitbox, Hurtbox hurtbox)
    {
        // Ignore if attacker and victim are same (or same team)
        if (hitbox.transform.root == hurtbox.transform.root) return;

        // Apply damage and effects
        ApplyDamage(hitbox.damage, hurtbox.owner);
        ApplyKnockback(hitbox, hurtbox.owner);
        TriggerHitEffects(hitbox, hurtbox);

        // Notify ComboManager that a hit occurred (for combo validation)
        ComboManager.Instance.ClearBuffer(); // Or buffer for combo continuation? Design choice.
    }

    private void ApplyDamage(int damage, GameObject victim)
    {
        // Find health component on victim and apply damage
        Health health = victim.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning($"No Health component on {victim.name}");
        }
    }

    private void ApplyKnockback(Hitbox hitbox, GameObject victim)
    {
        Rigidbody victimRb = victim.GetComponent<Rigidbody>();
        if (victimRb != null)
        {
            Vector3 knockbackDir = hitbox.knockbackDirection;
            if (knockbackDir == Vector3.zero) knockbackDir = Vector3.up;
            victimRb.AddForce(knockbackDir.normalized * hitbox.knockbackForce, ForceMode.Impulse);
        }
    }

    private void TriggerHitEffects(Hitbox hitbox, Hurtbox hurtbox)
    {
        // Trigger hit pause if enabled
        if (hitbox.canCauseHitPause && !isInHitPause)
        {
            float pauseDuration = hitbox.hitPauseDuration > 0 ? hitbox.hitPauseDuration : defaultHitPauseDuration;
            StartHitPause(pauseDuration, hitbox.hitPauseTimeScale > 0 ? hitbox.hitPauseTimeScale : hitPauseTimeScale);
        }

        // Trigger camera shake
        if (mainCamera != null)
        {
            StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));
        }

        // TODO: Spawn hit VFX at point of impact
        // TODO: Play hit sound
    }

    private IEnumerator HitPauseRoutine(float duration, float timeScale)
    {
        isInHitPause = true;
        float originalTimeScale = Time.timeScale;
        float originalFixedDeltaTime = Time.fixedDeltaTime;

        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f; // Keep fixed timestep consistent

        yield return new WaitForSecondsRealtime(duration); // Use unscaled time for wait

        // Restore
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        isInHitPause = false;
    }

    public void StartHitPause(float duration, float timeScale)
    {
        if (hitPauseCoroutine != null)
            StopCoroutine(hitPauseCoroutine);
        hitPauseCoroutine = StartCoroutine(HitPauseRoutine(duration, timeScale));
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            mainCamera.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.unscaledDeltaTime; // Use unscaled time so shake works during hit pause
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }
}