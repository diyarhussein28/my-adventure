using UnityEngine;
using SeasOfLegends.Combat;
using SeasOfLegends.Data;
using SeasOfLegends.Input;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Required components: Rigidbody (Interpolate), CapsuleCollider, Animator, PlayerInput,
    /// PlayerInputReader, Combatant. Freeze Rigidbody rotation in the Inspector.
    /// The controller only owns motor/sensing concerns; attack timing remains in CombatSystem.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
    [RequireComponent(typeof(PlayerInputReader), typeof(Combatant))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Required Data")]
        [SerializeField] private CharacterDefinition definition;
        [SerializeField] private CombatSystem combatSystem;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody body;

        [Header("Ground and Wall Sensing")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private LayerMask wallMask = ~0;
        [SerializeField, Min(0.05f)] private float groundProbeRadius = 0.24f;
        [SerializeField, Min(0.05f)] private float groundProbeDistance = 0.25f;
        [SerializeField, Min(0.1f)] private float wallProbeDistance = 0.8f;

        private PlayerStateMachine machine;
        private PlayerInputReader input;
        private Vector3 wallNormal;
        private float nextDashTime;
        private int airDashes;
        private float stunEndsAt;
        private float executionEndsAt;

        public PlayerInputReader Input => input;
        public bool IsGrounded { get; private set; }
        public bool IsBlocking { get; set; }
        public bool CanDash => Time.time >= nextDashTime && (IsGrounded || airDashes < definition.MaxAirDashes);
        public bool CanWallRun => !IsGrounded && ProbeWall(out wallNormal);
        public float WallRunDuration => definition.WallRunDuration;
        public bool StunFinished => Time.time >= stunEndsAt;
        public bool ExecutionFinished => Time.time >= executionEndsAt;

        /// <summary>Runtime bootstrap helper for the self-contained vertical-slice scene.</summary>
        public void ConfigureForPrototype(CharacterDefinition characterDefinition, CombatSystem systems, Transform camera)
        {
            definition = characterDefinition;
            combatSystem = systems;
            cameraTransform = camera;
        }

        public void SetCameraTransform(Transform camera) => cameraTransform = camera;

        /// <summary>Routes gameplay state parameters to the Animator on an imported humanoid visual.</summary>
        public void SetPresentationAnimator(Animator presentationAnimator)
        {
            if (presentationAnimator != null) animator = presentationAnimator;
        }

        private void Awake()
        {
            input = GetComponent<PlayerInputReader>();
            if (body == null) body = GetComponent<Rigidbody>();
            if (animator == null) animator = GetComponent<Animator>();
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.freezeRotation = true;
            machine = new PlayerStateMachine(this);
        }

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
            machine.Change(PlayerStateId.Locomotion);
        }

        private void Update()
        {
            UpdateSensors();
            machine.Tick();
            UpdateAnimatorParameters();
        }

        private void FixedUpdate()
        {
            machine.FixedTick();
            ApplyGravity();
        }

        private void UpdateSensors()
        {
            Vector3 origin = transform.position + Vector3.up * 0.2f;
            RaycastHit[] hits = Physics.SphereCastAll(origin, groundProbeRadius, Vector3.down, groundProbeDistance, groundMask, QueryTriggerInteraction.Ignore);
            IsGrounded = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.transform == transform || hits[i].collider.transform.IsChildOf(transform)) continue;
                IsGrounded = true;
                break;
            }
            if (IsGrounded) airDashes = 0;
        }

        private void ApplyGravity()
        {
            if (machine.CurrentId == PlayerStateId.Dashing || machine.CurrentId == PlayerStateId.WallRunning || IsGrounded) return;
            Vector3 velocity = body.velocity;
            velocity.y += Physics.gravity.y * (definition.GravityMultiplier - 1f) * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -definition.MaxFallSpeed);
            body.velocity = velocity;
        }

        public void MoveOnGround() => ApplyPlanarMovement(1f);
        public void MoveInAir() => ApplyPlanarMovement(0.45f);
        public void ApplyAttackMovement() => ApplyPlanarMovement(0.18f);
        public void ApplyBlockMovement() => ApplyPlanarMovement(0.3f);
        public void ApplyStunPhysics() { }

        private void ApplyPlanarMovement(float controlMultiplier)
        {
            Vector3 desired = CameraRelativeDirection(input.Move);
            float speed = definition.MoveSpeed * controlMultiplier;
            Vector3 current = body.velocity;
            Vector3 currentPlanar = new Vector3(current.x, 0f, current.z);
            Vector3 targetPlanar = desired * speed;
            // MoveTowards gives acceleration in m/s²: delta-v = acceleration * fixed delta time.
            Vector3 nextPlanar = Vector3.MoveTowards(currentPlanar, targetPlanar, definition.Acceleration * controlMultiplier * Time.fixedDeltaTime);
            body.velocity = new Vector3(nextPlanar.x, current.y, nextPlanar.z);
            if (desired.sqrMagnitude > 0.001f)
                body.MoveRotation(Quaternion.RotateTowards(body.rotation, Quaternion.LookRotation(desired), definition.RotationDegreesPerSecond * Time.fixedDeltaTime));
        }

        public void Jump()
        {
            Vector3 velocity = body.velocity;
            velocity.y = definition.JumpSpeed;
            body.velocity = velocity;
            IsGrounded = false;
            animator.SetTrigger("Jump");
        }

        public void JumpFromWall()
        {
            Vector3 away = wallNormal.sqrMagnitude > 0.01f ? wallNormal : -transform.forward;
            body.velocity = away * (definition.MoveSpeed * 0.75f) + Vector3.up * definition.JumpSpeed;
        }

        public float BeginDash()
        {
            Vector3 direction = CameraRelativeDirection(input.Move);
            if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
            if (!IsGrounded) airDashes++;
            nextDashTime = Time.time + definition.DashCooldown;
            body.velocity = new Vector3(direction.x * definition.DashSpeed, 0f, direction.z * definition.DashSpeed);
            animator.SetTrigger("Dash");
            return definition.DashDuration;
        }

        public void MoveAlongWall()
        {
            Vector3 tangent = Vector3.ProjectOnPlane(transform.forward, wallNormal).normalized;
            body.velocity = tangent * definition.WallRunSpeed + Vector3.down * 1.5f;
            body.MoveRotation(Quaternion.LookRotation(tangent));
        }

        public bool BeginAttack()
        {
            return combatSystem != null && combatSystem.TryStartAttack(this, input.BufferedAttack);
        }

        public bool UpdateAttack() => combatSystem == null || combatSystem.TickAttack(this);
        public void EndAttack() => combatSystem?.EndAttack(this);

        public void ReceiveHit(float stunSeconds)
        {
            if (IsBlocking) return;
            stunEndsAt = Mathf.Max(stunEndsAt, Time.time + stunSeconds);
            machine.Change(PlayerStateId.Stunned);
        }

        public void BeginExecution(float duration)
        {
            executionEndsAt = Time.time + duration;
            machine.Change(PlayerStateId.Executing);
        }

        public void SetAnimatorBool(string parameter, bool value) => animator.SetBool(parameter, value);

        private Vector3 CameraRelativeDirection(Vector2 move)
        {
            Vector3 forward = cameraTransform == null ? transform.forward : cameraTransform.forward;
            Vector3 right = cameraTransform == null ? transform.right : cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            return (forward.normalized * move.y + right.normalized * move.x).normalized;
        }

        private bool ProbeWall(out Vector3 normal)
        {
            Vector3 direction = CameraRelativeDirection(input.Move);
            if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
            Vector3 origin = transform.position + Vector3.up;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, wallProbeDistance, wallMask, QueryTriggerInteraction.Ignore))
            {
                normal = hit.normal;
                return Vector3.Angle(hit.normal, Vector3.up) > 70f;
            }
            normal = Vector3.zero;
            return false;
        }

        private void UpdateAnimatorParameters()
        {
            Vector3 local = transform.InverseTransformDirection(body.velocity);
            animator.SetFloat("SpeedX", local.x);
            animator.SetFloat("SpeedZ", local.z);
            animator.SetFloat("WalkRate", new Vector2(local.x, local.z).magnitude / Mathf.Max(0.01f, definition.MoveSpeed));
            animator.SetFloat("VerticalSpeed", body.velocity.y);
            animator.SetBool("Grounded", IsGrounded);
        }
    }
}
