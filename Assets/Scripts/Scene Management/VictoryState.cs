using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "States/VictoryState")]
public class VictoryState : GameState
{
    [SerializeField] private GameState MainMenuState;

    [Header("Music Settings")]
    [SerializeField] private AudioClip victoryMusic;
    public int WinnerID { get; private set; } //Get the player ID of who won the game
    public void SetWinner(int playerNumber)
    {
        WinnerID = playerNumber;
    }
    public override void EnterState(GameStateManager gameManager)
    {
        SceneManager.LoadScene("Victory");

        if (victoryMusic != null && gameManager.MusicSource != null)
        {
            gameManager.MusicSource.clip = victoryMusic;
            gameManager.MusicSource.Play();
        }
    }
    public override void UpdateState(GameStateManager gameManager)
    {
        //PLACEHOLDER
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToMainMenu();
        }
    }
    public override void ExitState(GameStateManager gameManager)
    {
        //Stop victoryMusic when exit Victory
        if (gameManager.MusicSource != null)
        {
            gameManager.MusicSource.Stop();
        }
    }
    private void ReturnToMainMenu()
    {
        GameStateManager.Instance.TransitionToState(MainMenuState);
    }
}
