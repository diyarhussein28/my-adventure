using UnityEngine;
using System.Collections;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Attacking state: handles combo input buffering, hitbox activation,
    /// and transitions based on frame data (startup/active/recovery).
    /// </summary>
    public class PlayerAttackingState : PlayerState
    {
        private float stateTimer = 0f;
        private bool hitboxActive = false;
        private SeasOfLegends.Data.AttackDataSO currentAttack;
        private int comboIndex = 0;
        private string comboInputBuffer = "";
        private float bufferTimer = 0f;

        public PlayerAttackingState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            player.CanAct = false;
            player.IsBlocking = false;
            hitboxActive = false;
            stateTimer = 0f;
            comboInputBuffer = "";

            // Determine which attack to perform
            currentAttack = DetermineAttack();
            if (currentAttack == null)
            {
                stateMachine.ChangeState(player.IsGrounded ? stateMachine.LocomotionState : stateMachine.AirborneState);
                return;
            }

            // Trigger animation
            string animName = GetAnimationName(currentAttack);
            player.Animator?.SetTrigger(animName);
            player.Animator?.SetFloat("AttackSpeed", player.CurrentWeapon?.speedMultiplier ?? 1f);

            // Begin combo tracking
            SeasOfLegends.Core.EventManager.Instance?.TriggerComboExtended(comboIndex + 1);
        }

        public override void Exit()
        {
            player.CanAct = true;
            hitboxActive = false;
            player.Animator?.ResetTrigger("LightAttack");
            player.Animator?.ResetTrigger("HeavyAttack");
            player.Animator?.ResetTrigger("SpecialAttack");
        }

        public override void Update()
        {
            stateTimer += Time.deltaTime;
            var input = SeasOfLegends.Input.InputManager.Instance;

            // Frame-data phases
            if (!hitboxActive && stateTimer >= currentAttack.StartupTime)
            {
                // Enter ACTIVE frames - enable hitbox
                hitboxActive = true;
                ActivateHitbox(true);
            }
            else if (hitboxActive && stateTimer >= currentAttack.StartupTime + currentAttack.ActiveTime)
            {
                // Exit ACTIVE frames - disable hitbox
                hitboxActive = false;
                ActivateHitbox(false);
            }

            // Buffer new inputs during active/recovery for combo continuation
            if (stateTimer >= currentAttack.StartupTime)
            {
                BufferInput(input);
            }

            // Attack complete
            if (stateTimer >= currentAttack.TotalDuration)
            {
                if (!string.IsNullOrEmpty(comboInputBuffer) && bufferTimer < currentAttack.RecoveryTime + 0.2f)
                {
                    // Continue combo
                    comboIndex++;
                    stateMachine.ChangeState(stateMachine.AttackingState);
                }
                else
                {
                    // Combo ended
                    comboIndex = 0;
                    SeasOfLegends.Core.EventManager.Instance?.TriggerComboBroken();
                    stateMachine.ChangeState(player.IsGrounded ? stateMachine.LocomotionState : stateMachine.AirborneState);
                }
            }

            bufferTimer += Time.deltaTime;
        }

        public override void FixedUpdate()
        {
            // Minimal movement during attacks - allow slight adjustment
            Vector3 moveDir = GetMovementDirection();
            if (moveDir.sqrMagnitude > 0.01f)
            {
                player.RotateTowards(moveDir, 180f);
            }
        }

        // --- Combo Logic ---

        private SeasOfLegends.Data.AttackDataSO DetermineAttack()
        {
            var input = SeasOfLegends.Input.InputManager.Instance;
            var weapon = player.CurrentWeapon;
            if (weapon == null || weapon.availableCombos == null || weapon.availableCombos.Length == 0)
                return null;

            // Get current input character
            string inputChar = GetInputChar(input);
            if (string.IsNullOrEmpty(inputChar)) return null;

            // Build combo string
            comboInputBuffer += inputChar;

            // Find matching combo
            foreach (var combo in weapon.availableCombos)
            {
                if (comboIndex < combo.attackSequence.Length)
                {
                    var attack = combo.attackSequence[comboIndex];
                    // Simple matching - in production, use proper sequence matching
                    if (attack.attackType == GetAttackTypeFromInput(input))
                        return attack;
                }
            }

            // Fallback: return basic attack for input type
            return CreateFallbackAttack(input);
        }

        private string GetInputChar(SeasOfLegends.Input.InputManager input)
        {
            if (input.LightAttackPressed) return "L";
            if (input.HeavyAttackPressed) return "H";
            if (input.SpecialAttackPressed) return "S";
            if (input.GrabPressed) return "G";
            if (input.UltimatePressed) return "U";
            return "";
        }

        private SeasOfLegends.Data.AttackType GetAttackTypeFromInput(SeasOfLegends.Input.InputManager input)
        {
            if (input.LightAttackPressed) return SeasOfLegends.Data.AttackType.Light;
            if (input.HeavyAttackPressed) return SeasOfLegends.Data.AttackType.Heavy;
            if (input.SpecialAttackPressed) return SeasOfLegends.Data.AttackType.Special;
            if (input.GrabPressed) return SeasOfLegends.Data.AttackType.Grab;
            if (input.UltimatePressed) return SeasOfLegends.Data.AttackType.Ultimate;
            return SeasOfLegends.Data.AttackType.Light;
        }

        private SeasOfLegends.Data.AttackDataSO CreateFallbackAttack(SeasOfLegends.Input.InputManager input)
        {
            // In production, load from ScriptableObject. Here we return a placeholder.
            // The actual system uses pre-authored AttackDataSO assets.
            return null;
        }

        private void BufferInput(SeasOfLegends.Input.InputManager input)
        {
            if (AnyAttackInput())
            {
                comboInputBuffer += GetInputChar(input);
                bufferTimer = 0f;
            }
        }

        private string GetAnimationName(SeasOfLegends.Data.AttackDataSO attack)
        {
            return attack.attackType switch
            {
                SeasOfLegends.Data.AttackType.Light => "LightAttack",
                SeasOfLegends.Data.AttackType.Heavy => "HeavyAttack",
                SeasOfLegends.Data.AttackType.Special => "SpecialAttack",
                SeasOfLegends.Data.AttackType.Grab => "Grab",
                SeasOfLegends.Data.AttackType.Ultimate => "Ultimate",
                _ => "LightAttack"
            };
        }

        private void ActivateHitbox(bool active)
        {
            // Activate/deactivate weapon hitbox collider
            // Actual implementation triggers CombatSystem to enable the hitbox
            if (active)
            {
                SeasOfLegends.Combat.HitboxManager.Instance?.ActivateHitbox(
                    player.WeaponAnchor, 
                    player.CurrentWeapon, 
                    currentAttack
                );
            }
            else
            {
                SeasOfLegends.Combat.HitboxManager.Instance?.DeactivateHitbox();
            }
        }
    }
}
