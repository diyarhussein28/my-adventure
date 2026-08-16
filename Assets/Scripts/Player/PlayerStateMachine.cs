using System.Collections.Generic;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Coordinates reusable state instances. A change always exits the old state before entering
    /// the new one, preventing animation flags and physics overrides from leaking between states.
    /// </summary>
    public sealed class PlayerStateMachine
    {
        private readonly Dictionary<PlayerStateId, PlayerState> states = new Dictionary<PlayerStateId, PlayerState>();
        public PlayerState Current { get; private set; }
        public PlayerStateId CurrentId { get; private set; }

        public PlayerStateMachine(PlayerController player)
        {
            states.Add(PlayerStateId.Locomotion, new LocomotionState(player, this));
            states.Add(PlayerStateId.Airborne, new AirborneState(player, this));
            states.Add(PlayerStateId.Dashing, new DashState(player, this));
            states.Add(PlayerStateId.WallRunning, new WallRunState(player, this));
            states.Add(PlayerStateId.Attacking, new AttackingState(player, this));
            states.Add(PlayerStateId.Blocking, new BlockingState(player, this));
            states.Add(PlayerStateId.Stunned, new StunnedState(player, this));
            states.Add(PlayerStateId.Executing, new ExecutingState(player, this));
        }

        public void Change(PlayerStateId next)
        {
            if (Current != null && CurrentId == next) return;
            Current?.Exit();
            CurrentId = next;
            Current = states[next];
            Current.Enter();
        }

        public void Tick() => Current?.Tick();
        public void FixedTick() => Current?.FixedTick();
    }
}
