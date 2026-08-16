using UnityEngine;
using UnityEngine.InputSystem;
using SeasOfLegends.Data;

namespace SeasOfLegends.Input
{
    /// <summary>
    /// Required component: PlayerInput configured with Behaviour = Invoke Unity Events.
    /// Map the named actions in the README to these callbacks. One-frame buttons are reset in
    /// LateUpdate so gameplay state updates see every press exactly once.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerInputReader : MonoBehaviour
    {
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

        public void OnMove(InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();
        public void OnSprint(InputAction.CallbackContext context) => SprintHeld = context.ReadValueAsButton();
        public void OnBlock(InputAction.CallbackContext context) => BlockHeld = context.ReadValueAsButton();
        public void OnJump(InputAction.CallbackContext context) { if (context.performed) JumpPressed = true; }
        public void OnDash(InputAction.CallbackContext context) { if (context.performed) DashPressed = true; }
        public void OnLockOn(InputAction.CallbackContext context) { if (context.performed) LockOnPressed = true; }
        public void OnInteract(InputAction.CallbackContext context) { if (context.performed) InteractPressed = true; }
        public void OnLightAttack(InputAction.CallbackContext context) => BufferAttack(context, AttackInput.Light);
        public void OnHeavyAttack(InputAction.CallbackContext context) => BufferAttack(context, AttackInput.Heavy);
        public void OnSpecialAttack(InputAction.CallbackContext context) => BufferAttack(context, AttackInput.Special);
        public void OnGrab(InputAction.CallbackContext context) => BufferAttack(context, AttackInput.Grab);
        public void OnUltimate(InputAction.CallbackContext context) => BufferAttack(context, AttackInput.Ultimate);

        private void BufferAttack(InputAction.CallbackContext context, AttackInput input)
        {
            if (!context.performed) return;
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
