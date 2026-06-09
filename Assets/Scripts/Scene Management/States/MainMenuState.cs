using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public override void UpdateState()
    {

    }
    public override void ExitState()
    {

    }
}
