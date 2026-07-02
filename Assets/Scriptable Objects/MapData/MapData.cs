using UnityEngine;

[CreateAssetMenu(fileName = "NewMapData", menuName = "Game/Map Data")]
public class MapData : ScriptableObject
{
    [Header("Map Info")]
    public string mapName;
    public GameObject mapPrefab;
    public AudioClip mapMusic;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Header("Left Player Transform")]
    public Vector3 leftPos = new Vector3(-16f, 5f, 8f);
    public Vector3 leftRot = new Vector3(0f, 90f, 0f);
    public Vector3 leftScale = new Vector3(5f, 5f, 5f);

    [Header("Right Player Transform")]
    public Vector3 rightPos = new Vector3(16f, 5f, 8f);
    public Vector3 rightRot = new Vector3(0f, -90f, 0f);
    public Vector3 rightScale = new Vector3(5f, 5f, 5f);
}