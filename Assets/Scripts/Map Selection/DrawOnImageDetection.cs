using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawOnImageDetection : MonoBehaviour
{
    // Maps
    private int numMaps;
    public List<Image> mapImageList;
    private List<(Image mapImage, MapData mapData)> imageAndMapList;

    public void Awake()
    {
        //GameStateManager.Instance.imageList = new List<Image> (mapImageList);

        numMaps = GameConstants.Instance.mapPresets.Length;
        for (int i = 0; i < numMaps; i++)
        {
            imageAndMapList.Add((mapImageList[i], GameConstants.Instance.mapPresets[i]));
            Debug.Log(imageAndMapList[i]);
        }

        GameStateManager.Instance.imageAndMapList = new List<(Image mapImage, MapData mapData)> (this.imageAndMapList);
    }
}
