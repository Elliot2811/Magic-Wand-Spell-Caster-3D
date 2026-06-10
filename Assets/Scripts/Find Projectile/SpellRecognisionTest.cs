using UnityEngine;

public class SpellRecognisionTest : MonoBehaviour
{
    public static SpellRecognisionTest Instance { get; private set; }

    [SerializeField] private ShapesCollectionSO shapesCollection;

    [SerializeField] private LineRenderer lineRendererLeft;
    [SerializeField] private Wand wandLeft;
    private Vector2[] pointsLeft;
    private bool newDrawingDataLeft = false;

    [SerializeField] private LineRenderer lineRendererRight;
    [SerializeField] private Wand wandRight;
    private Vector2[] pointsRight;
    private bool newDrawingDataRight = false;

    private Camera mainCamera;

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
            Debug.LogError("[GameManager]: shapesCollection not found.");

        if (wandLeft == null)
            Debug.LogError("[GameManager]: Wand script for left wand not found.");

        if (wandRight == null)
            Debug.LogError("[GameManager]: Wand script for right wand not found.");

        if (GameConstants.DisplayBestShape)
        {
            if (lineRendererLeft == null) 
                Debug.LogError("[GameManager]: lineRenderer for left wand not found.");

            if (lineRendererRight == null)
                Debug.LogError("[GameManager]: lineRenderer for right wand not found.");

            lineRendererLeft.startWidth = GameConstants.LineWidth;
            lineRendererLeft.endWidth = GameConstants.LineWidth;

            lineRendererRight.startWidth = GameConstants.LineWidth;
            lineRendererRight.endWidth = GameConstants.LineWidth;

            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (newDrawingDataLeft)
        {
            newDrawingDataLeft = false;
            ProcessData(pointsLeft, lineRendererLeft, true);
        }

        if (newDrawingDataRight)
        {
            newDrawingDataRight = false;
            ProcessData(pointsRight, lineRendererRight, false);
        }
    }

    #region Subscribe to wand and drawing data sent
    private void OnEnable()
    {
        wandLeft.OnDrawingComplete += UpdateDrawingLeft;
        wandRight.OnDrawingComplete += UpdateDrawingRight;
    }

    private void OnDisable()
    {
        wandLeft.OnDrawingComplete -= UpdateDrawingLeft;
        wandRight.OnDrawingComplete -= UpdateDrawingRight;
    }

    private void ProcessData(Vector2[] points, LineRenderer lineRenderer, bool left)
    {
        Vector2[] processedPoints = PointsManipulation.ResampleAndNormalize(points, GameConstants.PointCount);

        ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(processedPoints, shapesCollection);

        PrintBestShape(shapeInfo);

        if (GameConstants.DisplayBestShape)
            DisplayBestShape(shapeInfo, lineRenderer, left);
    }

    private void UpdateDrawingLeft(Vector2[] receivedPoints) 
    {
        //Debug.Log("GameManager: Recieved data");
        pointsLeft = receivedPoints;
        newDrawingDataLeft = true;
    }

    private void UpdateDrawingRight(Vector2[] recivedPoints)
    {
        //Debug.Log("GameManager: Recieved data");
        pointsRight = recivedPoints;
        newDrawingDataRight = true;
    }
    #endregion

    #region Display / Print Shape
    private void DisplayBestShape(ShapeInfoSO shapeInfo, LineRenderer lineRenderer, bool left)
    {
        if (shapeInfo == null)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        Vector2[] shapeData = shapeInfo.RandomVariantData;

        if (shapeData == null || shapeData.Length <= 1)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        Vector2[] catmulledData = PointsManipulation.CatmullRomLine(shapeData);
        Vector3[] rescaledData = PointsManipulation.ScaleToViewPort(
            catmulledData,
            mainCamera,
            10,
            left ? GameConstants.DisplayRectLeft : GameConstants.DisplayRectRight,
            GameConstants.DisplayShapePercentage
            );

        LineRendererInterface.Points(lineRenderer, rescaledData);
    }

    private void PrintBestShape(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo == null)
            Debug.Log($"No Shape to match to");
        else
            Debug.Log($"Shape is: {shapeInfo.ShapeName}");
    }
    #endregion
}   