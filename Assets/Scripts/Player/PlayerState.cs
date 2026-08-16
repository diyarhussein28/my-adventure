namespace SeasOfLegends.Player
{
    public abstract class PlayerState
    {
        protected readonly PlayerController Player;
        protected readonly PlayerStateMachine Machine;

        protected PlayerState(PlayerController player, PlayerStateMachine machine)
        {
            Player = player;
            Machine = machine;
        }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void FixedTick() { }
        public virtual void Exit() { }
    }

    public enum PlayerStateId
    {
        Locomotion,
        Airborne,
        Dashing,
        WallRunning,
        Attacking,
        Blocking,
        Stunned,
        Executing
    }
}
