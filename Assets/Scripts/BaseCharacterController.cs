using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls a character's movement, combat, and state transitions.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("References")]
    public CharacterData data;
    public Animator animator;
    public Rigidbody rb;

    [Header("Combo & Ability Data (set via CharacterManager)")]
    public ComboData[] combos;
    public CharacterAbility[] abilities;
    public CharacterAbility ultimateAbility;

    [Header("State")]
    public enum State { Idle, Moving, Jumping, Falling, Attacking, Ability, Ultimate, Stunned, Dodging }
    public State currentState = State.Idle;
    private State previousState;

    [Header("Combat Variables")]
    private float currentStamina;
    private float currentHealth;
    private bool isGrounded;
    private float lastInputTime;
    private int comboStep = 0;
    private float comboTimer;

    [Header("Input Buffer")]
    private Queue<string> inputBuffer = new Queue<string>();
    private const float INPUT_BUFFER_DURATION = 0.2f;

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError("CharacterData not assigned!", this);
            enabled = false;
            return;
        }

        currentHealth = data.maxHealth;
        currentStamina = data.maxStamina;

        // Initialize animator and rigidbody if not assigned
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Set up Rigidbody for character controller feel
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Assign abilities and combos from data
        abilities = data.abilities;
        ultimateAbility = data.ultimateAbility;
        combos = data.combos;
    }

    private void Update()
    {
        HandleInputBuffer();
        UpdateStateMachine();
    }

    private void FixedUpdate()
    {
        HandlePhysics();
    }

    private void HandleInputBuffer()
    {
        // Simplified input: we'll assume there's an InputManager that provides strings.
        // In a full implementation, you'd read from Unity's Input System or old Input.
        // For now, we'll simulate with placeholder.
        // This method would be called by an external input handler.
        // We'll leave it as a stub for now.
    }

    /// <summary>
    /// Called by input system when an input occurs.
    /// </summary>
    public void OnInput(string input)
    {
        inputBuffer.Enqueue(input);
        lastInputTime = Time.time;
    }

    private void UpdateStateMachine()
    {
        // State transitions based on current state and conditions
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Moving:
                HandleMoving();
                break;
            case State.Jumping:
                HandleJumping();
                break;
            case State.Falling:
                HandleFalling();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.Ability:
                HandleAbility();
                break;
            case State.Ultimate:
                HandleUltimate();
                break;
            case State.Stunned:
                HandleStunned();
                break;
            case State.Dodging:
                HandleDodging();
                break;
        }

        // Update animator parameters
        animator.SetInteger("State", (int)currentState);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    private void HandlePhysics()
    {
        // Ground check (simplified)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        // Apply gravity if not grounded
        if (!isGrounded && rb.velocity.y > -10f)
        {
            rb.AddForce(Vector3.down * data.weight * 20f, ForceMode.Acceleration);
        }
    }

    private void HandleIdle()
    {
        // Transition to moving if there's movement input
        // (This would be set by input handling)
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
        {
            currentState = State.Moving;
        }

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            currentState = State.Jumping;
            rb.AddForce(Vector3.up * Mathf.Sqrt(2f * data.jumpHeight * 9.81f), ForceMode.VelocityChange);
        }

        // Attack input (light attack)
        if (Input.GetButtonDown("Fire1"))
        {
            AttemptLightAttack();
        }

        // Ability input
        if (Input.GetButtonDown("Fire2"))
        {
            AttemptAbility(0); // First ability for simplicity
        }
    }

    private void HandleMoving()
    {
        // Handle movement input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;
        if (moveDir.sqrMagnitude > 0)
        {
            rb.MovePosition(rb.position + moveDir * data.movementSpeed * Time.fixedDeltaTime);
        }

        // Transition to idle if no movement
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
        {
            currentState = State.Idle;
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            currentState = State.Jumping;
            rb.AddForce(Vector3.up * Mathf.Sqrt(2f * data.jumpHeight * 9.81f), ForceMode.VelocityChange);
        }

        // Attack
        if (Input.GetButtonDown("Fire1"))
        {
            AttemptLightAttack();
        }
    }

    private void HandleJumping()
    {
        // Apply jump force already done in state change
        if (!isGrounded)
        {
            // Still in jump
            return;
        }
        else
        {
            // Landed
            currentState = State.Idle;
        }
    }

    private void HandleFalling()
    {
        // Similar to jumping but falling
        if (isGrounded)
        {
            currentState = State.Idle;
        }
    }

    private void HandleAttacking()
    {
        // This state is timed by animation events or animation length
        // We'll rely on animation to tell us when to exit
        // For simplicity, we'll use a timer based on the current combo's hit-stop
        // In a full implementation, animation events would reset the state.
    }

    private void HandleAbility()
    {
        // Similar to attacking, wait for animation to finish
    }

    private void HandleUltimate()
    {
        // Ultimate move, likely longer animation
    }

    private void HandleStunned()
    {
        // Wait for stun duration to finish
    }

    private void HandleDodging()
    {
        // Dodge roll or sidestep
    }

    private void AttemptLightAttack()
    {
        if (currentState != State.Idle && currentState != State.Moving)
            return;

        // Start combo
        comboStep = 0;
        ExecuteComboStep(comboStep);
        currentState = State.Attacking;
    }

    private void ExecuteComboStep(int step)
    {
        if (combos == null || step >= combos.Length || combos[step] == null)
        {
            Debug.LogWarning("Invalid combo step");
            return;
        }

        ComboData combo = combos[step];
        // Trigger animation via animator
        animator.SetTrigger("Attack_" + step);
        // Apply hit-stop, damage, etc. would be handled in animation events
        // For now, we'll just note that we're in an attack
        comboStep++;
        comboTimer = Time.time + combo.inputBufferTime;
    }

    private void AttemptAbility(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= abilities.Length)
            return;

        CharacterAbility ability = abilities[abilityIndex];
        if (ability == null)
            return;

        if (currentStamina < ability.staminaCost)
        {
            // Not enough stamina
            return;
        }

        // Check cooldown (simplified)
        // In reality, we'd track cooldown timers per ability
        currentStamina -= ability.staminaCost;
        currentState = State.Ability;
        animator.SetTrigger("Ability_" + abilityIndex);
        // Spawn VFX, apply effects, etc.
    }

    // Animation events would call these methods
    public void OnHit(int hitIndex)
    {
        // Apply damage, knockback, etc.
        Debug.Log($"Hit {hitIndex}");
    }

    public void OnAbilityComplete()
    {
        currentState = State.Idle;
    }

    public void OnUltimateComplete()
    {
        currentState = State.Idle;
    }

    public void OnComboComplete()
    {
        // Reset combo if not chaining
        if (Time.time > comboTimer)
        {
            comboStep = 0;
        }
        currentState = State.Idle;
    }

    // Getters for other systems
    public float GetHealthPercentage() => currentHealth / data.maxHealth;
    public float GetStaminaPercentage() => currentStamina / data.maxStamina;
}