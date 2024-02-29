using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected string animParameterName;

    #region State Timer
    // ���� ���� �ð� ex) 0.25�� ���� �뽬
    protected float stateTimer;
    #endregion

    #region Animation Event
    protected bool triggerCalled;
    #endregion

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animParameterName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animParameterName = animParameterName;
    }

    // Enter State
    public virtual void Enter()
    {
        player.animator.SetBool(animParameterName, true);
        triggerCalled = false;
    }

    // Update State
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    // FixedUpdate State
    public virtual void FixedUpdate()
    {

    }

    // Exit State
    public virtual void Exit()
    {
        player.animator.SetBool(animParameterName, false);
    }

    // Animation Finish
    public virtual void AnimationFinish()
    {
        triggerCalled = true;
    }
}
