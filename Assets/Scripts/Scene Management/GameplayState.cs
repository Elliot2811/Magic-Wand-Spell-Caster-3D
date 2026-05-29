using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "States/GameplayState")]
public class GameplayState : GameState
{
    [SerializeField] private VictoryState victoryState;
    private string mapToLoad;
    public void SetMap(string mapName)
    {
        mapToLoad = mapName;
    }

    public override void EnterState(GameStateManager gameManager)
    {
        SceneManager.LoadScene(mapToLoad);
    }
    public override void UpdateState(GameStateManager gameManager)
    {
        //PLACEHOLDER to decide which player wins
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EndMatch(1,gameManager);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EndMatch(2, gameManager);
        }
    }
    public override void ExitState(GameStateManager gameManager)
    {

    }
    public void EndMatch(int winningPlayerNumber, GameStateManager gameManager)
    {
        victoryState.SetWinner(winningPlayerNumber);
        gameManager.TransitionToState(victoryState);
    }
}
