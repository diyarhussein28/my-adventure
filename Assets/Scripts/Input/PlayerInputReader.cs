using UnityEngine;
using UnityEngine.InputSystem;
using SeasOfLegends.Data;

namespace SeasOfLegends.Input
{
    /// <summary>
    /// Required component: PlayerInput configured with Behaviour = Invoke Unity Events.
    /// Public callbacks support production action maps. The optional fallback gives a new
    /// vertical-slice scene immediate keyboard and mouse playability without inspector wiring.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private bool usePrototypeKeyboardFallback = true;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool BlockHeld { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool DashPressed { get; private set; }
        public bool LockOnPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool HasAttackPressed => attackPressed;
        public AttackInput BufferedAttack { get; private set; }

        private bool attackPressed;

        private void Update()
        {
            if (!usePrototypeKeyboardFallback || Keyboard.current == null) return;
            Keyboard keyboard = Keyboard.current;
            Move = new Vector2(
                (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f)).normalized;
            SprintHeld = keyboard.leftShiftKey.isPressed;
            JumpPressed |= keyboard.spaceKey.wasPressedThisFrame;
            DashPressed |= keyboard.leftShiftKey.wasPressedThisFrame;
            LockOnPressed |= keyboard.tabKey.wasPressedThisFrame;
            InteractPressed |= keyboard.fKey.wasPressedThisFrame;
            BlockHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;
            Look = Mouse.current == null ? Vector2.zero : Mouse.current.delta.ReadValue();

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) BufferAttack(AttackInput.Light);
            if (keyboard.qKey.wasPressedThisFrame) BufferAttack(AttackInput.Heavy);
            if (keyboard.eKey.wasPressedThisFrame) BufferAttack(AttackInput.Special);
            if (keyboard.rKey.wasPressedThisFrame) BufferAttack(AttackInput.Ultimate);
        }

        public void OnMove(InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();
        public void OnSprint(InputAction.CallbackContext context) => SprintHeld = context.ReadValueAsButton();
        public void OnBlock(InputAction.CallbackContext context) => BlockHeld = context.ReadValueAsButton();
        public void OnJump(InputAction.CallbackContext context) { if (context.performed) JumpPressed = true; }
        public void OnDash(InputAction.CallbackContext context) { if (context.performed) DashPressed = true; }
        public void OnLockOn(InputAction.CallbackContext context) { if (context.performed) LockOnPressed = true; }
        public void OnInteract(InputAction.CallbackContext context) { if (context.performed) InteractPressed = true; }
        public void OnLightAttack(InputAction.CallbackContext context) { if (context.performed) BufferAttack(AttackInput.Light); }
        public void OnHeavyAttack(InputAction.CallbackContext context) { if (context.performed) BufferAttack(AttackInput.Heavy); }
        public void OnSpecialAttack(InputAction.CallbackContext context) { if (context.performed) BufferAttack(AttackInput.Special); }
        public void OnGrab(InputAction.CallbackContext context) { if (context.performed) BufferAttack(AttackInput.Grab); }
        public void OnUltimate(InputAction.CallbackContext context) { if (context.performed) BufferAttack(AttackInput.Ultimate); }

        private void BufferAttack(AttackInput input)
        {
            BufferedAttack = input;
            attackPressed = true;
        }

        private void LateUpdate()
        {
            JumpPressed = false;
            DashPressed = false;
            LockOnPressed = false;
            InteractPressed = false;
            attackPressed = false;
        }
    }
}
