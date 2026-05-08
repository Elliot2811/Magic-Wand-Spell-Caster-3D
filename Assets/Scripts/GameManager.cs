using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Shapes to compare")]
    [SerializeField] private ShapesStorageSO shapesCollection;

    [Space]
    [Header("Display matched shape")]
    [SerializeField] private bool displayClosestShape = false;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float displayPercentage = 0.5f;

    [Space]
    [Header("Points matching options")]
    [SerializeField][Min(2)] private int pointCount = 64;

    [Space]
    [Header("Shapes comparision options")]
    [SerializeField][Range(0f, 1f)] private float minAccuracy = 0.8f;
    [SerializeField][Min(1)] private int deviationPower = 4;

    public static int PointCount { get; private set; }
    public static float MinAccuracy { get; private set; }
    public static int DeviationPower { get; private set; }

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
        MinAccuracy = minAccuracy;
        DeviationPower = deviationPower;
    }

    private void Update()
    {
        if (!newDrawingData)
            return;

        newDrawingData = false;

        points = PointsManipulation.ResampleAndNormalize(points, PointCount);

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
        //Debug.Log("GameManager: Recieved data");
        points = receivedPoints;
        newDrawingData = true;
    }
    #endregion

    #region Display / Print Shape
    private void PrintBestShape(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo == null)
            Debug.Log($"No Shape to match to");
        else
            Debug.Log($"Shape is: {shapeInfo.ShapeName}");
    }
    #endregion
}