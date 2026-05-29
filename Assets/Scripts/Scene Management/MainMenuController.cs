using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameState MapSelectionState;

    void Update()
    {
        //PLACEHOLDERS for actual controller input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerStartGame();
        }
    }
    public void TriggerStartGame()
    {
        GameStateManager.Instance.TransitionToState(MapSelectionState);
    }
}
