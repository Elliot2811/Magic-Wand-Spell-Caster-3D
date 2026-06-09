using UnityEngine;

public abstract class GameState
{
    protected GameStateManager stateManager;

    public virtual void EnterState(GameStateManager gameManager)
    {
        stateManager = gameManager;
    }

    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}
