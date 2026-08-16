using UnityEngine;

/// <summary>
/// Abstract base class for all player states in the Hierarchical FSM.
/// Each state handles its own logic for entering, updating, and exiting.
/// </summary>
public abstract class PlayerState
{
    protected PlayerController controller;
    protected Rigidbody rb;
    protected Animator animator;

    public PlayerState(PlayerController controller)
    {
        this.controller = controller;
        this.rb = controller.rb;
        this.animator = controller.animator;
    }

    public virtual void Enter() { }
    public virtual void HandleInput() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }
}