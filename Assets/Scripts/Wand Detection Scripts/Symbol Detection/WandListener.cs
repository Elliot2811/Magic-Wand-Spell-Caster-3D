using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WandListener : MonoBehaviour
{
    [SerializeField]
    private bool instantiated = true;
    private bool initialized = false;

    public ShapesCollectionSO shapes;

    //Legacy single-renderer fallback — kept so any existing Init(...) callers or
    //prefabs that still wire a single LineRenderer in don't break. Prefer
    //shapeLineRenderers below, which gives every shape its own dedicated
    //renderer so one shape's leftover line can never collide with another's.
    public LineRenderer lineRenderer;

    [System.Serializable]
    public struct ShapeLineRenderer
    {
        public ShapeInfoSO shape;
        public LineRenderer lineRenderer;
    }

    [Tooltip("One dedicated LineRenderer per drawable shape (e.g. the four tutorial shapes), so displaying/erasing one shape's system-drawn line never collides with another's.")]
    private ShapeLineRenderer[] shapeLineRenderers;

    public Wand wand;

    [Header("Shape Draw Animation")]
    [Tooltip("Controls the pacing of the draw — e.g. ease-out for a fast start that settles in.")]
    [SerializeField] private AnimationCurve shapeDrawCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Seconds the matched shape stays visible before it's erased.")]
    [SerializeField] private float shapeVisibleDuration = 1f; // was 3f — now clears after 1 sec per design note

    //Per-renderer coroutine tracking, since each shape can now animate/erase
    //independently instead of all sharing one lineRenderer's coroutine fields.
    private Dictionary<LineRenderer, Coroutine> drawCoroutines = new Dictionary<LineRenderer, Coroutine>();
    private Dictionary<LineRenderer, Coroutine> eraseCoroutines = new Dictionary<LineRenderer, Coroutine>();

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
        {
            wand.OnDrawingComplete -= FindBestShape;
            wand.OnDrawStarted -= ClearAllShapeRenderers;
        }
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
        wand.OnDrawStarted += ClearAllShapeRenderers;

        this.lineRenderer = lineRenderer;
        if (this.lineRenderer == null)
            this.lineRenderer = GetComponent<LineRenderer>();

        //Build one dedicated LineRenderer per base shape, parented to this
        //WandListener so they persist across every scene it does (Gameplay,
        //mid-game events, etc.) — no per-scene Inspector wiring needed.
        BuildShapeLineRenderers();

        foreach (var renderer in AllRenderers())
        {
            renderer.startColor = wand.deviceIndex == 0 ? GameConstants.LeftDrawingColor : GameConstants.RightDrawingColor;
            renderer.endColor = renderer.startColor;
            renderer.startWidth = GameConstants.LineWidth;
            renderer.endWidth = GameConstants.LineWidth;
        }

        this.shapes = shapes;
        initialized = true;
    }

    private void BuildShapeLineRenderers()
    {
        if (GameConstants.Instance.shapeLineRendererPrefab == null)
        {
            Debug.LogWarning("[WandListener]: No shapeLineRendererPrefab assigned on GameConstants — falling back to the single shared lineRenderer.");
            shapeLineRenderers = new ShapeLineRenderer[0];
            return;
        }

        ShapeInfoSO[] baseShapes = GameConstants.Instance.allShapes.GetAllShapes();
        shapeLineRenderers = new ShapeLineRenderer[baseShapes.Length];

        for (int i = 0; i < baseShapes.Length; i++)
        {
            LineRenderer instance = Instantiate(GameConstants.Instance.shapeLineRendererPrefab, transform);
            instance.gameObject.name = $"ShapeLine_{baseShapes[i].ShapeName}";
            instance.positionCount = 0;

            shapeLineRenderers[i] = new ShapeLineRenderer { shape = baseShapes[i], lineRenderer = instance };
        }
    }

    //Every LineRenderer this WandListener could possibly draw with — the per-shape
    //array plus the legacy fallback (if assigned) — so setup/clear code loops once.
    private IEnumerable<LineRenderer> AllRenderers()
    {
        if (shapeLineRenderers != null)
        {
            foreach (var pair in shapeLineRenderers)
                if (pair.lineRenderer != null)
                    yield return pair.lineRenderer;
        }

        if (lineRenderer != null)
            yield return lineRenderer;
    }

    private LineRenderer GetRendererFor(ShapeInfoSO shapeInfo)
    {
        if (shapeLineRenderers != null)
        {
            foreach (var pair in shapeLineRenderers)
            {
                if (pair.shape == shapeInfo && pair.lineRenderer != null)
                    return pair.lineRenderer;
            }
        }

        //Fallback for shapes without a dedicated renderer wired up in the Inspector.
        if (lineRenderer != null)
            Debug.LogWarning($"[WandListener]: No dedicated LineRenderer for shape '{shapeInfo?.ShapeName}', falling back to the shared lineRenderer.");

        return lineRenderer;
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

        if (AllRenderers().Any())
        {
            wand.ClearDrawnLine();
            DisplayBestShape(shapeInfo, points); //pass the raw points, not processedPoints
        }
    }

    private void DisplayBestShape(ShapeInfoSO shapeInfo, Vector2[] originalDrawnPoints)
    {
        if (shapeInfo == null)
            return;

        LineRenderer targetRenderer = GetRendererFor(shapeInfo);
        if (targetRenderer == null)
            return;

        StopRendererCoroutines(targetRenderer);

        Vector2[] shapeData = shapeInfo.RandomVariantData;
        if (shapeData == null || shapeData.Length <= 1)
        {
            targetRenderer.positionCount = 0;
            return;
        }

        Vector2[] catmulledData = PointsManipulation.CatmullRomLine(shapeData);
        Vector2[] finalPoints = PointsManipulation.ResampleAndNormalize(catmulledData, GameConstants.PointCount);
        float zDepth = Camera.main.transform.position.z + GameConstants.DistanceToCamera;
        Vector3[] rescaledData = PointsManipulation.FitToOriginalDrawing(finalPoints, originalDrawnPoints, zDepth);

        if (rescaledData == null || rescaledData.Length == 0)
        {
            targetRenderer.positionCount = 0;
            return;
        }

        drawCoroutines[targetRenderer] = StartCoroutine(AnimateShapeDraw(targetRenderer, rescaledData));
    }

    private IEnumerator AnimateShapeDraw(LineRenderer targetRenderer, Vector3[] points)
    {
        float elapsed = 0f;

        targetRenderer.positionCount = 1;
        targetRenderer.SetPosition(0, points[0]);

        while (elapsed < GameConstants.Instance.ShapeDrawDuration)
        {
            elapsed += Time.deltaTime;
            float t = shapeDrawCurve.Evaluate(Mathf.Clamp01(elapsed / GameConstants.Instance.ShapeDrawDuration));

            float exactIndex = t * (points.Length - 1);
            int fullyDrawnIndex = Mathf.FloorToInt(exactIndex);
            float frac = exactIndex - fullyDrawnIndex;
            bool hasTip = fullyDrawnIndex < points.Length - 1;

            //Grow positionCount as more of the shape reveals, rather than
            //pre-filling hidden points — avoids a stray line snapping back
            //to the start point.
            targetRenderer.positionCount = fullyDrawnIndex + 1 + (hasTip ? 1 : 0);

            for (int i = 0; i <= fullyDrawnIndex; i++)
                targetRenderer.SetPosition(i, points[i]);

            if (hasTip)
                targetRenderer.SetPosition(fullyDrawnIndex + 1, Vector3.Lerp(points[fullyDrawnIndex], points[fullyDrawnIndex + 1], frac));

            yield return null;
        }

        //Snap to the exact final shape in case of frame-timing drift.
        targetRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
            targetRenderer.SetPosition(i, points[i]);

        drawCoroutines.Remove(targetRenderer);

        //Shape fully revealed — start its own clear timer, since this
        //renderer is separate from Wand's and nothing else clears it
        if (eraseCoroutines.TryGetValue(targetRenderer, out Coroutine existingErase) && existingErase != null)
            StopCoroutine(existingErase);
        eraseCoroutines[targetRenderer] = StartCoroutine(EraseShapeAfterDelay(targetRenderer));
    }

    private IEnumerator EraseShapeAfterDelay(LineRenderer targetRenderer)
    {
        yield return new WaitForSeconds(shapeVisibleDuration);

        targetRenderer.positionCount = 0;
        eraseCoroutines.Remove(targetRenderer);
    }

    private void StopRendererCoroutines(LineRenderer targetRenderer)
    {
        if (drawCoroutines.TryGetValue(targetRenderer, out Coroutine draw) && draw != null)
        {
            StopCoroutine(draw);
            drawCoroutines.Remove(targetRenderer);
        }
        if (eraseCoroutines.TryGetValue(targetRenderer, out Coroutine erase) && erase != null)
        {
            StopCoroutine(erase);
            eraseCoroutines.Remove(targetRenderer);
        }
    }

    //Clears every shape's system-drawn line immediately — called the instant the
    //wand starts a new draw, so a previous shape's line can never linger on screen
    //(and read as "my drawing isn't registering") while the next one is in progress.
    private void ClearAllShapeRenderers()
    {
        foreach (var renderer in AllRenderers())
        {
            StopRendererCoroutines(renderer);
            renderer.positionCount = 0;
        }
    }

    public void ClearShape()
    {
        ClearAllShapeRenderers();
    }

    public void ChangeShapesCollection(ShapesCollectionSO shapes)
    {
        this.shapes = shapes;
    }

    public event Action<ShapeInfoSO> MatchedShape;
}