//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem.LowLevel;
//using UnityEngine.SceneManagement;

//[CreateAssetMenu(menuName = "States/VictoryState")]
//public class VictoryState : GameState
//{
//    [SerializeField] private GameState MainMenuState;

//    [Header("Music Settings")]
//    [SerializeField] private AudioClip victoryMusic;
//    public int WinnerID { get; private set; } //Get the player ID of who won the game
//    public void SetWinner(int playerNumber)
//    {
//        WinnerID = playerNumber;
//    }
//    public override void EnterState(GameStateManager gameManager)
//    {
//        SceneManager.LoadScene("Victory");

//        //Play Music
//        if (AudioManager.Instance != null)
//        {
//            AudioManager.Instance.PlayMusic(victoryMusic);
//        }
//    }
//    public override void UpdateState(GameStateManager gameManager)
//    {
//        //PLACEHOLDER
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            ReturnToMainMenu();
//        }
//    }
//    public override void ExitState(GameStateManager gameManager)
//    {
//        //Stop victoryMusic when exit Victory
//        if (AudioManager.Instance != null)
//        {
//            AudioManager.Instance.StopMusic();
//        }
//    }
//    private void ReturnToMainMenu()
//    {
//        GameStateManager.Instance.TransitionToState(MainMenuState);
//    }
//}

using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: reset game. 
public class VictoryState : GameState
{
    protected override AudioClip Music => stateManager?.audioLibrary?.victoryMusic;
    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (SceneManager.GetActiveScene().name != "Victory")
            SceneManager.LoadScene("victory");
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}