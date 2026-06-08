using System.Collections;
using UnityEngine;

public class DisplaySpellDrawing : MonoBehaviour
{
    RectTransform rectTransform;
    Rect screenRect;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    private ShapeInfoSO shapeInfoSO;

    private Vector2[] shapeData;
    private Vector3[] shapeDataPos;

    [SerializeField]
    private Canvas canvas;
    private Camera canvasCamera;

    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            Debug.LogError("[DisplaySpellDrawing]: RectTransform compoment not found.");

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                Debug.LogError("[DisplaySpellDrawing]: LineRenderer compoment not found.");
        }

        if (shapeInfoSO == null)
            Debug.LogError("[DisplaySpellDrawing]: No ShapeInfoSO scriptable object assigned.");

        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[DisplaySpellDrawing]: No parent Canvas found.");
            }
        }

        canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera ?? Camera.main;
    }

    private void Start()
    {
        StartCoroutine(DrawAfterLayout()    );
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return new Rect(
                corners[0].x,
                corners[0].y,
                corners[2].x - corners[0].x,
                corners[2].y - corners[0].y
                );
        }

        // Convert world corners to screen space
        Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[0]);
        Vector3 topRight = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[2]);

        return new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );
    }

    private IEnumerator DrawAfterLayout()
    {
        yield return new WaitForEndOfFrame();

        screenRect = GetScreenRect(rectTransform);

        shapeData = shapeInfoSO.RandomVariantData;
        shapeData = PointsManipulation.CatmullRomLine(shapeData);
        shapeDataPos = new Vector3[shapeData.Length];

        for (int i = 0; i < shapeData.Length; i++)
        {
            Vector2 screenPoint = new Vector2(
                Mathf.Lerp(screenRect.xMin, screenRect.xMax, shapeData[i].x),
                Mathf.Lerp(screenRect.yMin, screenRect.yMax, shapeData[i].y)
            );

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                screenPoint,
                canvasCamera,
                out Vector3 pointPos
                );
                
            shapeDataPos[i] = pointPos;
        }

        LineRendererInterface.Points(lineRenderer, shapeDataPos);
    }
}