public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    // State Initialize
    public void Initialize(PlayerState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    // State Change
    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
