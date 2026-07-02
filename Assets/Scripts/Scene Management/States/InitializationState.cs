using UnityEngine;

public class InitializationState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (stateManager.joyConTracker == null)
        {
            Debug.LogWarning("[InitializationState]: No joyConTracker");
            //stateManager.Disable();
            stateManager.TransitionToState(GameStateManager.StateEnum.PlayersSelect);
        }

        stateManager.joyConTracker.gameObject.SetActive(true);
    }

    public override void UpdateState()
    {
        if (stateManager.joyConTracker.count == 0 || stateManager.joyConTracker.gameObject.activeInHierarchy)
        {
            stateManager.joyConTracker.gameObject.SetActive(false);
            stateManager.TransitionToState(GameStateManager.StateEnum.PlayersSelect);
        }

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