using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{
    protected override AudioPair Music => stateManager?.audioLibrary?.mapSelectionMusic;
    public override void EnterState(GameStateManager gameManager)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SceneManager.LoadScene("MainMenu");
    }
    public override void UpdateState()
    {

    }
    public override void ExitState()
    {

    }
}
