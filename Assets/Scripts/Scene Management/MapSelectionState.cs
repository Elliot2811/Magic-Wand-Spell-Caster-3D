using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "States/MapSelectionState")]
public class MapSelectionState : GameState
{
    public override void EnterState(GameStateManager gameManager)
    {
        if (SceneManager.GetActiveScene().name != "MapSelection")
        {
            SceneManager.LoadScene("MapSelection");
        }
    }
    public override void UpdateState(GameStateManager gameManager)
    {
        //WAIT FOR PLAYER TO SELECT MAP (LakeWorld/MysticalForestWorld)
    }
    public override void ExitState(GameStateManager gameManager)
    {

    }
}
