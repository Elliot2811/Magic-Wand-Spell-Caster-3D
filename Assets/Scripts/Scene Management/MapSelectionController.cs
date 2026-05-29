using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectionController : MonoBehaviour
{
    [SerializeField] private GameplayState gameplayState;

    void Update()
    {
        //PLACEHOLDERS for actual controller input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectMap("LakeWorld");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectMap("MysticalForestWorld");
        }
    }

    public void SelectMap(string mapName)
    {
        gameplayState.SetMap(mapName);
        GameStateManager.Instance.TransitionToState(gameplayState);
    }
}
