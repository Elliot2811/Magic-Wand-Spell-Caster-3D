using UnityEngine;

public class VictoryCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject player1WinPanel;
    [SerializeField]
    private GameObject player2WinPanel;

    private void Start()
    {
        if (player1WinPanel != null) player1WinPanel.SetActive(false);
        if (player2WinPanel != null) player2WinPanel.SetActive(false);

        DisplayWinner(GameStateManager.Instance);
    }

    private void DisplayWinner(GameStateManager stateManager)
    {
        if (stateManager.leftWon)
        {
            player1WinPanel.SetActive(true);
        }
        else if (stateManager.rightWon)
        {
            player2WinPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No one won");
        }
    }
}