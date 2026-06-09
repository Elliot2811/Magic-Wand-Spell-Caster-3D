using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName ="States/CoinInsertState")]
public class CoinInsertState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        //Load Main Menu Screen
        if (SceneManager.GetActiveScene().name != "CoinInsert")
        {
            SceneManager.LoadScene("CoinInsert");
        }
    }
    public override void UpdateState(GameStateManager gameManager)
    {
        //WAIT FOR PLAYER TO START GAME
    }
    public override void ExitState(GameStateManager gameManager)
    {

    }
}
