using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "States/GameplayState")]
public class GameplayState : GameState
{
    [SerializeField] private VictoryState victoryState;
    private string mapToLoad;

    [Header("Music Settings")]
    [SerializeField] private AudioClip lakeWorldMusic;
    [SerializeField] private AudioClip mysticalForestWorldMusic;

    public void SetMap(string mapName)
    {
        mapToLoad = mapName;
    }

    public override void EnterState(GameStateManager gameManager)
    {
        SceneManager.LoadScene(mapToLoad);

        AudioClip trackToPlay = null;

        if (mapToLoad == "LakeWorld")
        {
            trackToPlay = lakeWorldMusic;
        }
        if (mapToLoad == "MysticalForestWorld")
        {
            trackToPlay = mysticalForestWorldMusic;
        }

        if (trackToPlay != null && gameManager.MusicSource != null)
        {
            gameManager.MusicSource.clip = trackToPlay;
            gameManager.MusicSource.Play();
        }
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
        //Stop battleMusic if exit Gameplay
        if (gameManager.MusicSource != null)
        {
            gameManager.MusicSource.Stop();
        }
    }
    public void EndMatch(int winningPlayerNumber, GameStateManager gameManager)
    {
        victoryState.SetWinner(winningPlayerNumber);
        gameManager.TransitionToState(victoryState);
    }
}
