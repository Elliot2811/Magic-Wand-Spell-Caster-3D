using System;
using System.Collections;
using UnityEngine;

public class WandListener : MonoBehaviour
{
    [SerializeField]
    private bool instantiated = true;
    private bool initialized = false;

    public ShapesCollectionSO shapes;

    public LineRenderer lineRenderer;

    public Wand wand;

    [Header("Shape Draw Animation")]
    [Tooltip("Controls the pacing of the draw — e.g. ease-out for a fast start that settles in.")]
    [SerializeField] private AnimationCurve shapeDrawCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine drawShapeCoroutine;

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
            DisplayBestShape(shapeInfo, points); //pass the raw points, not processedPoints
        }
    }

    private void DisplayBestShape(ShapeInfoSO shapeInfo, Vector2[] originalDrawnPoints)
    {
        if (drawShapeCoroutine != null)
            StopCoroutine(drawShapeCoroutine);

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
        Vector2[] finalPoints = PointsManipulation.ResampleAndNormalize(catmulledData, GameConstants.PointCount);
        float zDepth = Camera.main.transform.position.z + GameConstants.DistanceToCamera;
        Vector3[] rescaledData = PointsManipulation.FitToOriginalDrawing(finalPoints, originalDrawnPoints, zDepth);

        if (rescaledData == null || rescaledData.Length == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        drawShapeCoroutine = StartCoroutine(AnimateShapeDraw(rescaledData));
    }
    private IEnumerator AnimateShapeDraw(Vector3[] points)
    {
        float elapsed = 0f;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, points[0]);

        while (elapsed < GameConstants.Instance.ShapeDrawDuration)
        {
            elapsed += Time.deltaTime;
            float t = shapeDrawCurve.Evaluate(Mathf.Clamp01(elapsed / GameConstants.Instance.ShapeDrawDuration));

            float exactIndex = t * (points.Length - 1);
            int fullyDrawnIndex = Mathf.FloorToInt(exactIndex);
            float frac = exactIndex - fullyDrawnIndex;
            bool hasTip = fullyDrawnIndex < points.Length - 1;

            // Grow positionCount as more of the shape reveals, rather than
            // pre-filling hidden points — avoids a stray line snapping back
            // to the start point.
            lineRenderer.positionCount = fullyDrawnIndex + 1 + (hasTip ? 1 : 0);

            for (int i = 0; i <= fullyDrawnIndex; i++)
                lineRenderer.SetPosition(i, points[i]);

            if (hasTip)
                lineRenderer.SetPosition(fullyDrawnIndex + 1, Vector3.Lerp(points[fullyDrawnIndex], points[fullyDrawnIndex + 1], frac));

            yield return null;
        }

        // Snap to the exact final shape in case of frame-timing drift.
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
            lineRenderer.SetPosition(i, points[i]);

        drawShapeCoroutine = null;
    }

    public void ChangeShapesCollection(ShapesCollectionSO shapes)
    {
        this.shapes = shapes;
    }

    public event Action<ShapeInfoSO> MatchedShape;
}