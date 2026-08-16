using UnityEngine;

namespace SeasOfLegends.Player
{
    internal sealed class LocomotionState : PlayerState
    {
        public LocomotionState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Tick()
        {
            if (!Player.IsGrounded) { Machine.Change(PlayerStateId.Airborne); return; }
            if (Player.Input.BlockHeld) { Machine.Change(PlayerStateId.Blocking); return; }
            if (Player.Input.JumpPressed) { Player.Jump(); Machine.Change(PlayerStateId.Airborne); return; }
            if (Player.Input.DashPressed && Player.CanDash) { Machine.Change(PlayerStateId.Dashing); return; }
            if (Player.Input.HasAttackPressed) Machine.Change(PlayerStateId.Attacking);
        }
        public override void FixedTick() => Player.MoveOnGround();
    }

    internal sealed class AirborneState : PlayerState
    {
        public AirborneState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Tick()
        {
            if (Player.IsGrounded) { Machine.Change(PlayerStateId.Locomotion); return; }
            if (Player.Input.DashPressed && Player.CanDash) { Machine.Change(PlayerStateId.Dashing); return; }
            if (Player.CanWallRun && Player.Input.Move.y > 0.1f) { Machine.Change(PlayerStateId.WallRunning); return; }
            if (Player.Input.HasAttackPressed) Machine.Change(PlayerStateId.Attacking);
        }
        public override void FixedTick() => Player.MoveInAir();
    }

    internal sealed class DashState : PlayerState
    {
        private float expiresAt;
        public DashState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Enter() => expiresAt = Time.time + Player.BeginDash();
        public override void Tick()
        {
            if (Time.time >= expiresAt)
                Machine.Change(Player.IsGrounded ? PlayerStateId.Locomotion : PlayerStateId.Airborne);
        }
    }

    internal sealed class WallRunState : PlayerState
    {
        private float expiresAt;
        public WallRunState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Enter() { expiresAt = Time.time + Player.WallRunDuration; Player.SetAnimatorBool("WallRunning", true); }
        public override void Tick()
        {
            if (Player.Input.JumpPressed) { Player.JumpFromWall(); Machine.Change(PlayerStateId.Airborne); return; }
            if (!Player.CanWallRun || Time.time >= expiresAt) { Machine.Change(PlayerStateId.Airborne); return; }
        }
        public override void FixedTick() => Player.MoveAlongWall();
        public override void Exit() => Player.SetAnimatorBool("WallRunning", false);
    }

    internal sealed class AttackingState : PlayerState
    {
        public AttackingState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Enter()
        {
            if (!Player.BeginAttack()) Machine.Change(Player.IsGrounded ? PlayerStateId.Locomotion : PlayerStateId.Airborne);
        }
        public override void Tick()
        {
            if (Player.UpdateAttack()) Machine.Change(Player.IsGrounded ? PlayerStateId.Locomotion : PlayerStateId.Airborne);
        }
        public override void FixedTick() => Player.ApplyAttackMovement();
        public override void Exit() => Player.EndAttack();
    }

    internal sealed class BlockingState : PlayerState
    {
        public BlockingState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Enter() { Player.IsBlocking = true; Player.SetAnimatorBool("Blocking", true); }
        public override void Tick()
        {
            if (!Player.IsGrounded) { Machine.Change(PlayerStateId.Airborne); return; }
            if (!Player.Input.BlockHeld) Machine.Change(PlayerStateId.Locomotion);
        }
        public override void FixedTick() => Player.ApplyBlockMovement();
        public override void Exit() { Player.IsBlocking = false; Player.SetAnimatorBool("Blocking", false); }
    }

    internal sealed class StunnedState : PlayerState
    {
        public StunnedState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Enter() => Player.SetAnimatorBool("Stunned", true);
        public override void Tick()
        {
            if (Player.StunFinished) Machine.Change(Player.IsGrounded ? PlayerStateId.Locomotion : PlayerStateId.Airborne);
        }
        public override void FixedTick() => Player.ApplyStunPhysics();
        public override void Exit() => Player.SetAnimatorBool("Stunned", false);
    }

    internal sealed class ExecutingState : PlayerState
    {
        public ExecutingState(PlayerController player, PlayerStateMachine machine) : base(player, machine) { }
        public override void Enter() => Player.SetAnimatorBool("Executing", true);
        public override void Tick() { if (Player.ExecutionFinished) Machine.Change(PlayerStateId.Locomotion); }
        public override void Exit() => Player.SetAnimatorBool("Executing", false);
    }
}
