using UnityEngine;

public abstract class GameState
{
    protected GameStateManager stateManager;
    protected virtual AudioClip Music => null;

    public virtual void EnterState(GameStateManager gameManager)
    {
        stateManager = gameManager;
        Debug.Log($"[{GetType().Name}] Music clip = {Music}");
        if (Music != null)
        {
            Debug.Log($"AudioManager.Instance = {AudioManager.Instance}");
            AudioManager.Instance.PlayMusic(Music);
        }
    }

    public virtual void UpdateState() { }
    public virtual void ExitState()
    {
        clearWand();
    }

    public void clearWand()
    {
        if (stateManager.wandLeft != null)
            if (stateManager.wandLeft.lineRenderer != null)
                stateManager.wandLeft.lineRenderer.positionCount = 0;

        if (stateManager.wandRight != null)
            if (stateManager.wandRight.lineRenderer != null)
                stateManager.wandRight.lineRenderer.positionCount = 0;

        if (stateManager.wandListenerLeft != null)
            if (stateManager.wandListenerLeft.lineRenderer != null)
                stateManager.wandListenerLeft.lineRenderer.positionCount = 0;

        if (stateManager.wandListenerRight != null)
            if (stateManager.wandListenerRight.lineRenderer != null)
                stateManager.wandListenerRight.lineRenderer.positionCount = 0;
    }
}
