using UnityEngine;

/// <summary>
/// Main player controller. Handles input, state machine, and core movement.
/// Requires: Rigidbody, CapsuleCollider, Animator
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // Components
    public Rigidbody rb { get; private set; }
    public Animator animator { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }

    // State Machine
    private PlayerState currentState;
    public PlayerState CurrentState => currentState;

    // Input Configuration
    [Header("Input")]
    public string moveHorizontalAxis = "Horizontal"; // Default Unity Input Manager axis
    public string moveVerticalAxis = "Vertical";     // Default Unity Input Manager axis
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode attackKey = KeyCode.Mouse0;      // Left mouse button
    public KeyCode blockKey = KeyCode.Mouse1;       // Right mouse button (hold)

    // Movement input (updated each frame)
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool dashPressed;
    private bool attackPressed;
    private bool blockPressed;

    // Configuration (tweak via Inspector or ScriptableObject)
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;

    [Header("Physics")]
    public float gravityScale = 2.5f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    // Dash tracking
    private float dashTimeRemaining;
    private Vector3 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        // Read input (Old Input System)
        moveInput = new Vector2(Input.GetAxis(moveHorizontalAxis), Input.GetAxis(moveVerticalAxis));
        jumpPressed = Input.GetKeyDown(jumpKey);
        dashPressed = Input.GetKeyDown(dashKey);
        attackPressed = Input.GetKeyDown(attackKey);
        blockPressed = Input.GetKey(blockKey); // Hold to block

        // Handle input (if any state needs it)
        currentState.HandleInput();

        // Update Animator parameters
        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    private void FixedUpdate()
    {
        // Physics updates
        currentState.PhysicsUpdate();

        // Apply gravity
        if (!IsGrounded())
        {
            rb.AddForce(Vector3.down * gravityScale * rb.mass, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Changes the current state, calling Exit on old and Enter on new.
    /// </summary>
    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// Checks if player is grounded using a sphere cast.
    /// </summary>
    public bool IsGrounded()
    {
        return Physics.CheckSphere(
            transform.position - Vector3.up * (capsuleCollider.height / 2 - groundCheckRadius),
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    /// <summary>
    /// Returns the movement direction relative to camera forward (for camera-relative controls).
    /// Assumes a main camera tagged "MainCamera".
    /// </summary>
    public Vector3 GetMoveDirection()
    {
        if (moveInput == Vector2.zero) return Vector3.zero;

        Camera mainCam = Camera.main;
        if (!mainCam) return new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        return (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }

    /// <summary>
    /// Called by animation events to trigger footsteps or impact effects.
    /// </summary>
    public void OnFootstep() => Debug.Log("Footstep SFX");
    public void OnLand() => Debug.Log("Land SFX & VFX");
}