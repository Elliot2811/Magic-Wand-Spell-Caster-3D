using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : ScriptableObject
{
    public abstract void EnterState(GameStateManager gameManager);
    public abstract void UpdateState(GameStateManager gameManager);
    public abstract void ExitState(GameStateManager gameManager);
}
