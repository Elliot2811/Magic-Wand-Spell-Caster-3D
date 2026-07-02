//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MapSelectionController : MonoBehaviour
//{
//    [SerializeField] private GameplayState gameplayState;

//    [Header("Map Assets")]
//    [SerializeField] private MapData LakeWorldMapData;
//    [SerializeField] private MapData MysticalForestWorldData;

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            SelectMap(LakeWorldMapData);
//        }
//        else if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            SelectMap(MysticalForestWorldData);
//        }
//    }

//    public void SelectMap(MapData mapData)
//    {
//        if (mapData == null)
//        {
//            Debug.LogError("The selected Map Data asset is missing from the Inspector!");
//            return;
//        }

//        gameplayState.SetMap(mapData);
//        GameStateManager.Instance.TransitionToState(gameplayState);
//    }
//}