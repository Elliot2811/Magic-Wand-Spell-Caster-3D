using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ShapesStorageSO shapesCollection;

    [SerializeField] private LineRenderer lineRenderer;

    private Camera mainCamera;

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
            Debug.LogError("[GameManager]: shapesCollection not found.");

        if (GameConstants.DisplayBestShape)
        {
            if (lineRenderer == null)
                Debug.LogError("[GameManager]: lineRenderer not found.");
            else
            {
                lineRenderer.startWidth = GameConstants.LineWidth;
                lineRenderer.endWidth = GameConstants.LineWidth;

                mainCamera = Camera.main;
            }
        }
    }

    private void Update()
    {
        if (!newDrawingData)
            return;

        newDrawingData = false;

        points = PointsManipulation.ResampleAndNormalize(points, GameConstants.PointCount);

        ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(points, shapesCollection);

        PrintBestShape(shapeInfo);

        if (GameConstants.DisplayBestShape)
            DisplayBestShape(shapeInfo);
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
    private void DisplayBestShape(ShapeInfoSO shapeInfo)
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

        Vector2[] rescaledData = PointsManipulation.ScaleToScreen(shapeData, mainCamera, GameConstants.DisplayShapePercentage, GameConstants.DisplayShapeOffset);

        LineRendererInterface.UpdateLineRenderer(lineRenderer, rescaledData);
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