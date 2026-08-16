using UnityEngine;
using UnityEngine.InputSystem;

namespace SeasOfLegends.Input
{
    /// <summary>
    /// Required Components:
    /// - PlayerInput component (Unity New Input System)
    /// 
    /// Wraps Unity's New Input System into game-specific actions.
    /// Decouples raw input from gameplay logic.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        // Movement
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprinting { get; private set; }

        // Combat
        public bool LightAttackPressed { get; private set; }
        public bool HeavyAttackPressed { get; private set; }
        public bool SpecialAttackPressed { get; private set; }
        public bool BlockPressed { get; private set; }
        public bool GrabPressed { get; private set; }
        public bool UltimatePressed { get; private set; }

        // Movement Actions
        public bool JumpPressed { get; private set; }
        public bool DashPressed { get; private set; }

        // Camera
        public bool LockOnPressed { get; private set; }
        public bool CameraSwitchSidePressed { get; private set; }

        // UI
        public bool PausePressed { get; private set; }
        public bool InteractPressed { get; private set; }

        // Raw access for combo buffering
        public string LastAttackInput { get; private set; }
        public float LastInputTime { get; private set; }

        private PlayerInput playerInput;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("InputManager requires a PlayerInput component!");
                return;
            }

            // Subscribe to action events
            var actions = playerInput.actions;
            actions["Move"].performed += OnMove;
            actions["Move"].canceled += OnMoveCanceled;
            actions["Look"].performed += OnLook;
            actions["Look"].canceled += OnLookCanceled;
            actions["Sprint"].performed += OnSprint;
            actions["Sprint"].canceled += OnSprintCanceled;
            actions["Jump"].performed += OnJump;
            actions["Dash"].performed += OnDash;
            actions["LightAttack"].performed += OnLightAttack;
            actions["HeavyAttack"].performed += OnHeavyAttack;
            actions["SpecialAttack"].performed += OnSpecialAttack;
            actions["Block"].performed += OnBlock;
            actions["Block"].canceled += OnBlockCanceled;
            actions["Grab"].performed += OnGrab;
            actions["Ultimate"].performed += OnUltimate;
            actions["LockOn"].performed += OnLockOn;
            actions["Pause"].performed += OnPause;
            actions["Interact"].performed += OnInteract;
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            var actions = playerInput.actions;
            actions["Move"].performed -= OnMove;
            actions["Move"].canceled -= OnMoveCanceled;
            actions["Look"].performed -= OnLook;
            actions["Look"].canceled -= OnLookCanceled;
            actions["Sprint"].performed -= OnSprint;
            actions["Sprint"].canceled -= OnSprintCanceled;
            actions["Jump"].performed -= OnJump;
            actions["Dash"].performed -= OnDash;
            actions["LightAttack"].performed -= OnLightAttack;
            actions["HeavyAttack"].performed -= OnHeavyAttack;
            actions["SpecialAttack"].performed -= OnSpecialAttack;
            actions["Block"].performed -= OnBlock;
            actions["Block"].canceled -= OnBlockCanceled;
            actions["Grab"].performed -= OnGrab;
            actions["Ultimate"].performed -= OnUltimate;
            actions["LockOn"].performed -= OnLockOn;
            actions["Pause"].performed -= OnPause;
            actions["Interact"].performed -= OnInteract;
        }

        private void LateUpdate()
        {
            // Reset one-frame inputs
            LightAttackPressed = false;
            HeavyAttackPressed = false;
            SpecialAttackPressed = false;
            GrabPressed = false;
            UltimatePressed = false;
            JumpPressed = false;
            DashPressed = false;
            LockOnPressed = false;
            PausePressed = false;
            InteractPressed = false;
        }

        // --- Movement ---
        private void OnMove(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(InputAction.CallbackContext ctx) => MoveInput = Vector2.zero;
        private void OnLook(InputAction.CallbackContext ctx) => LookInput = ctx.ReadValue<Vector2>();
        private void OnLookCanceled(InputAction.CallbackContext ctx) => LookInput = Vector2.zero;
        private void OnSprint(InputAction.CallbackContext ctx) => IsSprinting = true;
        private void OnSprintCanceled(InputAction.CallbackContext ctx) => IsSprinting = false;
        private void OnJump(InputAction.CallbackContext ctx) { JumpPressed = true; }
        private void OnDash(InputAction.CallbackContext ctx) { DashPressed = true; }

        // --- Combat ---
        private void OnLightAttack(InputAction.CallbackContext ctx) 
        { 
            LightAttackPressed = true; 
            LastAttackInput = "L";
            LastInputTime = Time.time;
        }
        private void OnHeavyAttack(InputAction.CallbackContext ctx) 
        { 
            HeavyAttackPressed = true; 
            LastAttackInput = "H";
            LastInputTime = Time.time;
        }
        private void OnSpecialAttack(InputAction.CallbackContext ctx) 
        { 
            SpecialAttackPressed = true; 
            LastAttackInput = "S";
            LastInputTime = Time.time;
        }
        private void OnBlock(InputAction.CallbackContext ctx) => BlockPressed = true;
        private void OnBlockCanceled(InputAction.CallbackContext ctx) => BlockPressed = false;
        private void OnGrab(InputAction.CallbackContext ctx) { GrabPressed = true; LastAttackInput = "G"; LastInputTime = Time.time; }
        private void OnUltimate(InputAction.CallbackContext ctx) { UltimatePressed = true; LastAttackInput = "U"; LastInputTime = Time.time; }

        // --- Camera & UI ---
        private void OnLockOn(InputAction.CallbackContext ctx) => LockOnPressed = true;
        private void OnPause(InputAction.CallbackContext ctx) => PausePressed = true;
        private void OnInteract(InputAction.CallbackContext ctx) => InteractPressed = true;
    }
}
