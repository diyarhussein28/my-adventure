using UnityEngine;

/// <summary>
/// Simple enemy AI state machine. Demonstrates how enemies can interact
/// with the player's combat system. Requires: Rigidbody, Animator, Collider (for hurtbox).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyAI : MonoBehaviour
{
    // Components
    public Rigidbody rb { get; private set; }
    public Animator animator { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }
    public Hurtbox hurtbox { get; private set; } // Reference to enemy's hurtbox

    // State Machine
    private EnemyState currentState;
    public EnemyState CurrentState => currentState;

    // Configuration
    [Header("Detection")]
    public float detectionRadius = 15f;
    public float attackRadius = 3f;
    public LayerMask playerLayer;

    [Header("Combat")]
    public int enemyDamage = 10;
    public float enemyKnockbackForce = 5f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float rotationSpeed = 10f;

    // Internal
    private Transform playerTransform;
    private float timeSinceLastSawPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        hurtbox = GetComponentInChildren<Hurtbox>();
        if (hurtbox == null)
        {
            // Create a hurtbox if missing (should be on prefab)
            hurtbox = gameObject.AddComponent<Hurtbox>();
            hurtbox.owner = gameObject;
        }

        // Find player (assumes player tagged "Player")
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    private void Start()
    {
        // Start in Patrol state
        ChangeState(new PatrolState(this));
    }

    private void Update()
    {
        // Handle state logic
        currentState.Update();

        // Simple timer for losing sight of player
        if (playerTransform != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distToPlayer < detectionRadius && CanSeePlayer())
            {
                timeSinceLastSawPlayer = 0f;
            }
            else
            {
                timeSinceLastSawPlayer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Changes the current state.
    /// </summary>
    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// Simple line-of-sight check (can be enhanced with physics raycast).
    /// </summary>
    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;
        Vector3 toPlayer = (playerTransform.position - transform.position).normalized;
        // Check if player is in front of enemy (within 120 degree cone)
        if (Vector3.Dot(transform.forward, toPlayer) < 0.5f) return false;
        // Raycast for obstacles
        if (Physics.Raycast(transform.position + Vector3.up, toPlayer, out RaycastHit hit, detectionRadius))
        {
            return hit.transform == playerTransform;
        }
        return false;
    }

    /// <summary>
    /// Called by Hurtbox when hit. Applies hit reaction.
    /// </summary>
    public void OnHit(Hitbox hitbox, Hurtbox playerHurtbox)
    {
        // Apply damage via enemy's health (if any)
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(hitbox.damage);
        }

        // React to hit (flinch, stagger, etc.)
        // For simplicity, we'll just enter a hit stun state
        if (currentState.GetType() != typeof(StunnedState))
        {
            ChangeState(new StunnedState(this, 0.5f)); // 0.5 second stun
        }

        // Optional: knockback from hit
        {
            Vector3 knockbackDir = (transform.position - hitbox.transform.position).normalized;
            rb.AddForce(knockbackDir * hitbox.knockbackForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Called when enemy successfully hits the player.
    /// </summary>
    public void AttackPlayer()
    {
        if (playerTransform != null)
        {
            // Apply damage to player via their hurtbox
            var playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyDamage);
            }

            // Apply knockback to player
            var playerRb = playerTransform.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDir = (playerTransform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * enemyKnockbackForce, ForceMode.Impulse);
            }
        }
    }

    // --- Getters for states ---
    public Transform GetPlayerTransform() => playerTransform;
    public float GetTimeSinceLastSawPlayer() => timeSinceLastSawPlayer;
    public Rigidbody GetRigidbody() => rb;
}

// --- Enemy States ---

public abstract class EnemyState
{
    protected EnemyAI enemy;
    protected Rigidbody rb;
    protected Animator animator;

    public EnemyState(EnemyAI enemy)
    {
        this.enemy = enemy;
        this.rb = enemy.rb;
        this.animator = enemy.animator;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class PatrolState : EnemyState
{
    public PatrolState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
        animator.SetFloat("Speed", 0f); // Idle
        // Set a random patrol point (could be waypoints)
        // For simplicity, just wander
        WanderTarget = GetRandomWanderTarget();
        WanderTimer = 0f;
    }

    private Vector3 WanderTarget;
    private float WanderTimer;
    private const float WanderChangeTime = 5f; // Change wander target every 5 seconds

    public override void Update()
    {
        // Check for player
        if (enemy.GetTimeSinceLastSawPlayer() < 1f && enemy.CanSeePlayer())
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;
        }

        // Wander logic
        WanderTimer += Time.deltaTime;
        if (WanderTimer >= WanderChangeTime)
        {
            WanderTarget = GetRandomWanderTarget();
            WanderTimer = 0f;
        }

        // Move towards wander target
        Vector3 direction = (WanderTarget - enemy.transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            // Rotate towards direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            // Move forward
            rb.velocity = enemy.transform.forward * enemy.patrolSpeed;
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private Vector3 GetRandomWanderTarget()
    {
        // Random point within radius
        Vector3 randomDir = Random.insideUnitSphere * 10f;
        randomDir += enemy.transform.position;
        randomDir.y = enemy.transform.position.y; // Keep on ground
        return randomDir;
    }

    public override void Exit()
    {
        // Nothing to clean up
    }
}

public class ChaseState : EnemyState
{
    public ChaseState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
        animator.SetFloat("Speed", 1f); // Walk/Run
    }

    public override void Update()
    {
        // Lose player if not seen for too long
        if (enemy.GetTimeSinceLastSawPlayer() > 5f)
        {
            enemy.ChangeState(new PatrolState(enemy));
            return;
        }

        // If close enough, attack
        if (enemy.CanSeePlayer() &&
            Vector3.Distance(enemy.transform.position, enemy.GetPlayerTransform().position) < enemy.attackRadius)
        {
            enemy.ChangeState(new AttackState(enemy));
            return;
        }

        // Continue chasing
        Vector3 direction = (enemy.GetPlayerTransform().position - enemy.transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            // Rotate towards player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            // Move forward
            rb.velocity = enemy.transform.forward * enemy.chaseSpeed;
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    public override void Exit()
    {
        // Nothing
    }
}

public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public AttackState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        // Stop movement during attack
        rb.velocity = Vector3.zero;
    }

    public override void Update()
    {
        // Execute attack animation event will call AttackPlayer()
        // For now, just timer-based
        if (Time.time - lastAttackTime > attackCooldown)
        {
            // Check if still in range
            if (Vector3.Distance(enemy.transform.position, enemy.GetPlayerTransform().position) < enemy.attackRadius)
            {
                enemy.AttackPlayer();
                lastAttackTime = Time.time;
            }
            else
            {
                // Player ran away, go back to chase
                enemy.ChangeState(new ChaseState(enemy));
            }
        }
    }

    public override void Exit()
    {
        animator.ResetTrigger("Attack");
    }
}

public class StunnedState : EnemyState
{
    private float stunTimer;

    public StunnedState(EnemyAI enemy, float stunDuration) : base(enemy)
    {
        stunTimer = stunDuration;
    }

    public override void Enter()
    {
        rb.velocity = Vector3.zero;
        animator.SetTrigger("Stunned");
    }

    public override void Update()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            // Recover to patrol or chase depending on player visibility
            if (enemy.CanSeePlayer())
                enemy.ChangeState(new ChaseState(enemy));
            else
                enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public override void Exit()
    {
        animator.ResetTrigger("Stunned");
    }
}