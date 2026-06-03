using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    [SerializeField] private VictoryState victoryState;

    [Header("Character")]
    [SerializeField] private GameObject player1WinPanel;
    [SerializeField] private GameObject player2WinPanel;

    void Start()
    {
        if (player1WinPanel != null) player1WinPanel.SetActive(false);
        if (player2WinPanel != null) player2WinPanel.SetActive(false);

        DisplayWinner(victoryState.WinnerID);
    }
    private void DisplayWinner(int winningPlayer)
    {
        //SCALE OF CHARACTERS TO BE ADJUSTED
        if (winningPlayer == 1)
        {
            player1WinPanel.SetActive(true);
        }
        else if (winningPlayer == 2)
        {
            player2WinPanel.SetActive(true);
        }
        else
        {
            Debug.Log("No winner is assigned");
        }
    }


    void Update()
    {
        
    }
}
