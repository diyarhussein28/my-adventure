using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Required Components:
    /// - Rigidbody (with interpolate set to Interpolate)
    /// - CapsuleCollider
    /// - Animator
    /// - PlayerInput component with InputManager
    /// 
    /// Central player controller that manages the state machine,
    /// handles physics, and delegates behavior to active states.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private Transform cameraTransform;

        [Header("Character Data")]
        [SerializeField] private SeasOfLegends.Data.CharacterStatsSO characterStats;
        public SeasOfLegends.Data.CharacterStatsSO CharacterStats => characterStats;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private float groundCheckRadius = 0.45f;

        [Header("Wall Detection")]
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private float wallCheckDistance = 0.6f;
        [SerializeField] private float wallRunHeight = 1.5f;

        [Header("Combat")]
        [SerializeField] private Transform weaponAnchor;
        [SerializeField] private SeasOfLegends.Data.WeaponDataSO currentWeapon;
        public SeasOfLegends.Data.WeaponDataSO CurrentWeapon => currentWeapon;

        // State Machine
        private PlayerStateMachine stateMachine;
        public PlayerStateMachine StateMachine => stateMachine;

        // Physics & Movement Properties (exposed for states)
        public Vector3 Velocity => rb.linearVelocity;
        public bool IsGrounded { get; private set; }
        public bool IsTouchingWall { get; private set; }
        public Vector3 WallNormal { get; private set; }
        public Transform CameraTransform => cameraTransform;
        public Animator Animator => animator;
        public Rigidbody Rigidbody => rb;
        public Transform WeaponAnchor => weaponAnchor;

        // Combat Properties
        public bool CanAct { get; set; } = true;
        public bool IsInvincible { get; set; } = false;
        public bool IsBlocking { get; set; } = false;
        public int CurrentAirDashes { get; set; } = 0;

        // Cached values
        private float originalCapsuleHeight;
        private Vector3 originalCapsuleCenter;

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider>();

            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            originalCapsuleHeight = capsuleCollider.height;
            originalCapsuleCenter = capsuleCollider.center;

            stateMachine = new PlayerStateMachine(this);
        }

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            stateMachine.ChangeState(stateMachine.LocomotionState);
        }

        private void Update()
        {
            UpdateGroundCheck();
            UpdateWallCheck();
            stateMachine.Update();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
            ApplyGravity();
        }

        // --- Physics Helpers ---

        private void UpdateGroundCheck()
        {
            Vector3 spherePosition = transform.position + Vector3.up * groundCheckRadius;
            bool wasGrounded = IsGrounded;
            IsGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer);

            if (IsGrounded && !wasGrounded)
            {
                CurrentAirDashes = 0;
                SeasOfLegends.Core.EventManager.Instance?.TriggerPlayerLanded();
            }
        }

        private void UpdateWallCheck()
        {
            Vector3 origin = transform.position + Vector3.up * wallRunHeight * 0.5f;
            IsTouchingWall = Physics.Raycast(origin, transform.forward, out RaycastHit hit, wallCheckDistance, wallLayer);
            WallNormal = IsTouchingWall ? hit.normal : Vector3.zero;
        }

        private void ApplyGravity()
        {
            if (!IsGrounded && rb.linearVelocity.y < 0)
            {
                rb.AddForce(Physics.gravity * (characterStats.gravityMultiplier - 1f), ForceMode.Acceleration);
            }

            // Terminal velocity cap
            if (rb.linearVelocity.y < -characterStats.maxFallSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -characterStats.maxFallSpeed, rb.linearVelocity.z);
            }
        }

        private void UpdateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            animator.SetFloat("SpeedX", localVelocity.x);
            animator.SetFloat("SpeedZ", localVelocity.z);
            animator.SetFloat("VelocityY", rb.linearVelocity.y);
            animator.SetBool("IsGrounded", IsGrounded);
            animator.SetBool("IsTouchingWall", IsTouchingWall);
        }

        // --- Public Movement API (called by states) ---

        public void Move(Vector3 direction, float speedMultiplier = 1f)
        {
            float speed = characterStats.moveSpeed * speedMultiplier;
            if (SeasOfLegends.Input.InputManager.Instance?.IsSprinting == true)
                speed *= characterStats.sprintMultiplier;

            Vector3 targetVelocity = direction * speed;
            targetVelocity.y = rb.linearVelocity.y; // Preserve gravity

            // Smooth velocity change for responsive but not twitchy movement
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 15f * Time.fixedDeltaTime);
        }

        public void RotateTowards(Vector3 direction, float rotationSpeed = 720f)
        {
            if (direction.sqrMagnitude < 0.001f) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void RotateTowardsCamera()
        {
            if (cameraTransform == null) return;
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            RotateTowards(camForward.normalized);
        }

        public void Jump()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * characterStats.jumpForce, ForceMode.Impulse);
            IsGrounded = false;
        }

        public void Dash(Vector3 direction)
        {
            if (CurrentAirDashes >= characterStats.maxAirDashes && !IsGrounded) return;

            rb.linearVelocity = Vector3.zero;
            float dashSpeed = characterStats.dashDistance / characterStats.dashDuration;
            rb.AddForce(direction * dashSpeed, ForceMode.Impulse);

            if (!IsGrounded)
                CurrentAirDashes++;

            SeasOfLegends.Core.EventManager.Instance?.TriggerPlayerDashed();
        }

        public void ApplyImpulse(Vector3 force, ForceMode mode = ForceMode.Impulse)
        {
            rb.AddForce(force, mode);
        }

        public void SetVelocity(Vector3 velocity)
        {
            rb.linearVelocity = velocity;
        }

        // --- Combat API ---

        public void TakeDamage(float damage, Vector3 hitDirection, bool wasBlocked = false)
        {
            if (IsInvincible) return;

            float finalDamage = damage * (1f - characterStats.defense);
            if (wasBlocked && IsBlocking)
            {
                finalDamage *= 0.2f;
                // Pushback while blocking
                ApplyImpulse(hitDirection * 3f + Vector3.up * 2f);
            }

            // TODO: Apply damage to health system
            SeasOfLegends.Core.EventManager.Instance?.TriggerPlayerTookDamage(
                new SeasOfLegends.Core.CombatEventData
                {
                    Defender = gameObject,
                    Damage = finalDamage,
                    HitPoint = transform.position + Vector3.up * 1f,
                    HitNormal = -hitDirection,
                    WasBlocked = wasBlocked
                }
            );

            // Enter hitstun if not blocking
            if (!wasBlocked || !IsBlocking)
            {
                stateMachine.ChangeState(stateMachine.StunnedState);
            }
        }

        // --- Debug ---

        private void OnDrawGizmosSelected()
        {
            // Ground check
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * groundCheckRadius, groundCheckRadius);

            // Wall check
            Gizmos.color = IsTouchingWall ? Color.cyan : Color.yellow;
            Vector3 wallOrigin = transform.position + Vector3.up * wallRunHeight * 0.5f;
            Gizmos.DrawLine(wallOrigin, wallOrigin + transform.forward * wallCheckDistance);
        }
    }
}
