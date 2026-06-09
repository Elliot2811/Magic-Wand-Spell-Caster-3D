using UnityEngine;

public class InitializationState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (stateManager.joyConTracker == null)
        {
            Debug.LogError("[InitializationState]: No joyConTracker");
            stateManager.Disable();
        }

        stateManager.AddState(GameStateManager.StateEnum.PlayersSelect, new CoinInsertState());
    }

    public override void UpdateState()
    {
        if (stateManager.joyConTracker.Connected)
        {
            stateManager.TransitionToState(GameStateManager.StateEnum.PlayersSelect);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}