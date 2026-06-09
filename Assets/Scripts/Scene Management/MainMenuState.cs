using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "States/MainMenuState")]
public class MainMenuState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public override void UpdateState(GameStateManager gameManager)
    {
        //WAIT FOR PLAYER TO SELECT MAP (LakeWorld/MysticalForestWorld)
    }
    public override void ExitState(GameStateManager gameManager)
    {

    }
}
