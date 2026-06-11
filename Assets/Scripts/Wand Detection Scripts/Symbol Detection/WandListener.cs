using System;
using UnityEngine;

public class WandListener : MonoBehaviour
{
    [SerializeField]
    private bool instantiated = true;
    private bool initialized = false;

    public ShapesCollectionSO shapes;

    public LineRenderer lineRenderer;

    public Wand wand;

    private void Start()
    {
        if (!instantiated)
        {
            Init();
        }
    }

    private void OnDisable()
    {
        if (wand != null)
            wand.OnDrawingComplete -= FindBestShape;
    }

    private void Init()
    {
        Init(wand, lineRenderer, shapes);
    }

    public void Init(Wand wand, LineRenderer lineRenderer = null, ShapesCollectionSO shapes = null)
    {
        this.wand = wand;
        if (this.wand == null)
            Debug.LogError("No wand gameobject found");


        wand.OnDrawingComplete += FindBestShape;

        this.lineRenderer = lineRenderer;
        if (this.lineRenderer == null)
        {
            this.lineRenderer = GetComponent<LineRenderer>();
            if (this.lineRenderer == null)
                Debug.LogWarning("No linerender found.");
        }

        this.lineRenderer.startColor = Color.white;
        this.lineRenderer.endColor = Color.white;
        this.lineRenderer.startWidth = GameConstants.LineWidth;
        this.lineRenderer.endWidth = GameConstants.LineWidth;

        this.shapes = shapes;

        initialized = true;
    }

    private void ChangeComparedShapes(ShapesCollectionSO shapes)
    {
        this.shapes = shapes;

        if (this.shapes == null)
            Debug.LogWarning("No ShapesCollectionSO passed.");
    }

    private void FindBestShape(Vector2[] points)
    {
        if (!initialized)
        {
            Debug.LogWarning("[WandListener]: No ShapesCollectionSO to compare to.");
            MatchedShape?.Invoke(null);
        }

        Vector2[] processedPoints = PointsManipulation.ResampleAndNormalize(points, GameConstants.PointCount);

        ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(processedPoints, shapes);

        MatchedShape?.Invoke(shapeInfo);

        if (lineRenderer != null)
        {
            DisplayBestShape(shapeInfo);
        }
    }

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

        Vector2[] catmulledData = PointsManipulation.CatmullRomLine(shapeData);
        Vector3[] rescaledData = PointsManipulation.ScaleToViewPort(
            catmulledData,
            Camera.main,
            10,
            wand.deviceIndex == 0 ? GameConstants.DisplayRectLeft : GameConstants.DisplayRectRight,
            GameConstants.DisplayShapePercentage
            );

        LineRendererInterface.Points(lineRenderer, rescaledData);
    }

    public event Action<ShapeInfoSO> MatchedShape;
}