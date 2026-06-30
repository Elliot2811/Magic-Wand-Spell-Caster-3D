using UnityEngine;

public class InitializationState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        stateManager.AddState(GameStateManager.StateEnum.PlayersSelect, () => new CoinInsertState());
        stateManager.AddState(GameStateManager.StateEnum.Fight, () => new GamePlayState());
        stateManager.AddState(GameStateManager.StateEnum.Winner, () => new VictoryState());

        if (stateManager.joyConTracker == null)
        {
            Debug.LogWarning("[InitializationState]: No joyConTracker");
            //stateManager.Disable();
            stateManager.TransitionToState(GameStateManager.StateEnum.PlayersSelect);
        }
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