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

        this.lineRenderer = wand.lineRenderer; //use wand.lineRenderer so shape replaces the spell drawn by player
        if (this.lineRenderer == null)
        {
            this.lineRenderer = GetComponent<LineRenderer>();
            if (this.lineRenderer == null)
            {
                Debug.LogError("No linerender found.");
            }
        }

        //this.lineRenderer.startColor = Color.white;
        //this.lineRenderer.endColor = Color.white;
        //this.lineRenderer.startWidth = GameConstants.LineWidth;
        //this.lineRenderer.endWidth = GameConstants.LineWidth;

        this.shapes = shapes;

        initialized = true;
    }

    private void FindBestShape(Vector2[] points)
    {
        if (!initialized)
        {
            Debug.LogWarning("[WandListener]: No ShapesCollectionSO to compare to.");
            MatchedShape?.Invoke(null);
            return;
        }

        Vector2[] processedPoints = PointsManipulation.ResampleAndNormalize(points, GameConstants.PointCount);

        ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(processedPoints, shapes);

        MatchedShape?.Invoke(shapeInfo);

        if (lineRenderer != null)
        {
            DisplayBestShape(shapeInfo, points); // pass the raw points, not processedPoints
        }
    }

    private void DisplayBestShape(ShapeInfoSO shapeInfo, Vector2[] originalDrawnPoints)
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

        // Match the depth the drawing was rendered at (see Wand.LogMousePos's zClipPlane calc).
        float zDepth = Camera.main.transform.position.z + GameConstants.DistanceToCamera;

        Vector3[] rescaledData = PointsManipulation.FitToOriginalDrawing(catmulledData, originalDrawnPoints, zDepth);

        LineRendererInterface.Points(lineRenderer, rescaledData);
    }

    public void ChangeShapesCollection(ShapesCollectionSO shapes)
    {
        this.shapes = shapes;
    }

    public event Action<ShapeInfoSO> MatchedShape;
}