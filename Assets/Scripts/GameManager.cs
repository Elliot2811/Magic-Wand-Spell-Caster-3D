using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ShapesStorageSO shapesCollection; 
    [SerializeField] [Min(2)]private int pointCount = 64;
    [SerializeField] private bool conserveScale = true;

    [Range(0f, 1f)] public static float MinAccuracy = 0.8f;
    [Min(1)] public static int DeviationPower = 4;

    //[SerializeField] [Range(0f, 1f)] private float minAccuracy = 0.8f;
    //[SerializeField] [Min(1)] private int deviationPower = 1;

    public static int PointCount { get; private set; }
    public static bool ConserveScale { get; private set; }
    //public static float MinAccuracy { get; private set; }
    //public static int DeviationPower { get; private set; }

    private Vector2[] points;
    private bool newDrawingData = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("GameManager initialised");
        DontDestroyOnLoad(gameObject);

        if (shapesCollection == null)
        {
            Debug.LogError("[GameManager]: shapes collection not found");
        }

        PointCount = pointCount;
        //ConserveScale = conserveScale;
        //MinAccuracy = minAccuracy;
        //DeviationPower = deviationPower;
    }

    private void Update()
    {
        if (!newDrawingData)
            return;

        newDrawingData = false;

        points = PointsManipulation.ResampleAndNormalize(points, pointCount, conserveScale);

        ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(points, shapesCollection);

        PrintBestShape(shapeInfo);
    }

    #region Subscribe to wand and drawing data sent
    private void OnEnable()
    {
        WandBase.OnDrawingComplete += UpdateDrawing;
    }

    private void OnDisable()
    {
        WandBase.OnDrawingComplete -= UpdateDrawing;
    }

    private void UpdateDrawing(Vector2[] receivedPoints) 
    {
        Debug.Log("GameManager: Recieved data");
        points = receivedPoints;
        newDrawingData = true;
    }
    #endregion

    private void PrintBestShape(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo == null)
            Debug.Log($"No Shape to match to");
        else
            Debug.Log($"Shape is: {shapeInfo.ShapeName}");
    }
}