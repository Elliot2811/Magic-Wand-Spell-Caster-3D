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

        stateManager.AddState(GameStateManager.StateEnum.PlayersSelect, () => new CoinInsertState());
        stateManager.AddState(GameStateManager.StateEnum.Fight, () => new GamePlayState());
    }

    public override void UpdateState()
    {
        if (stateManager.joyConTracker.readyToConnect)
        {
            stateManager.TransitionToState(GameStateManager.StateEnum.PlayersSelect);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}