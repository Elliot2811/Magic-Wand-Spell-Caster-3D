using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    [SerializeField] private VictoryState victoryState;

    [SerializeField] private GameObject player1CharacterDisplay;
    [SerializeField] private GameObject player2CharacterDisplay;

    //Scale of Players (TO BE ADJUSTED)
    [SerializeField] private Vector3 winnerScale = new Vector3(10f, 10f, 10f);
    [SerializeField] private Vector3 loserScale = new Vector3(3f, 3f, 3f);

    void Start()
    {
        DisplayWinner(victoryState.WinnerID);
    }
    private void DisplayWinner(int winningPlayer)
    {
        //SCALE OF CHARACTERS TO BE ADJUSTED
        if (winningPlayer == 1)
        {
            player1CharacterDisplay.transform.localScale = winnerScale;
            player2CharacterDisplay.transform.localScale = loserScale;
        }
        else if (winningPlayer == 2)
        {
            player1CharacterDisplay.transform.localScale = loserScale;
            player2CharacterDisplay.transform.localScale = winnerScale;
        }
        else
        {
            Debug.Log("No winner is assigned"); //Incase there is a tie/edge case
            //Keep characters default size
            player1CharacterDisplay.transform.localScale = Vector3.one;
            player2CharacterDisplay.transform.localScale = Vector3.one;
        }
    }


    void Update()
    {
        
    }
}
