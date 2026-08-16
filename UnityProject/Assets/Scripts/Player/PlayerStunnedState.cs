using UnityEngine;
using System.Collections;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Stunned state: player cannot act after taking a hit.
    /// Duration scales with stun resistance stat.
    /// </summary>
    public class PlayerStunnedState : PlayerState
    {
        private float stunDuration = 0.5f;
        private float timer = 0f;

        public PlayerStunnedState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            player.CanAct = false;
            player.IsBlocking = false;
            timer = 0f;

            // Apply stun resistance reduction
            stunDuration = 0.5f * (1f - player.CharacterStats.stunResistance);
            stunDuration = Mathf.Max(0.1f, stunDuration);

            player.Animator?.SetTrigger("Stunned");
            player.Animator?.SetBool("IsStunned", true);
        }

        public override void Exit()
        {
            player.CanAct = true;
            player.Animator?.SetBool("IsStunned", false);
            player.Animator?.ResetTrigger("Stunned");
        }

        public override void Update()
        {
            timer += Time.deltaTime;
            if (timer >= stunDuration)
            {
                stateMachine.ChangeState(player.IsGrounded ? stateMachine.LocomotionState : stateMachine.AirborneState);
            }
        }

        public override void FixedUpdate()
        {
            // Apply drag to slow down knockback
            Vector3 velocity = player.Velocity;
            velocity.x *= 0.9f;
            velocity.z *= 0.9f;
            player.SetVelocity(velocity);
        }
    }
}
