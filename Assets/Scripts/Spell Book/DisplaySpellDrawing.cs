using System.Collections;
using UnityEngine;

public class DisplaySpellDrawing : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField]
    private UILineRenderer uiLineRenderer;

    private ShapeInfoSO shapeInfoSO;

    private Vector2[] shapeData;
    private Vector2[] shapeDataLocalPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            Debug.LogError("[DisplaySpellDrawing]: RectTransform component not found.");

        if (uiLineRenderer == null)
        {
            uiLineRenderer = GetComponent<UILineRenderer>();
            if (uiLineRenderer == null)
                Debug.LogError("[DisplaySpellDrawing]: UILineRenderer component not found.");
        }
    }

    private void Start()
    {
        StartCoroutine(DrawAfterLayout());
    }

    private void DrawShape()
    {
        if (rectTransform == null || uiLineRenderer == null || shapeInfoSO == null)
            return;

        Rect rect = rectTransform.rect;

        shapeData = shapeInfoSO.RandomVariantData;
        shapeData = PointsManipulation.CatmullRomLine(shapeData);
        shapeDataLocalPos = new Vector2[shapeData.Length];

        for (int i = 0; i < shapeData.Length; i++)
        {
            shapeDataLocalPos[i] = new Vector2(
                Mathf.Lerp(rect.xMin, rect.xMax, shapeData[i].x),
                Mathf.Lerp(rect.yMin, rect.yMax, shapeData[i].y)
                );
        }

        uiLineRenderer.SetPoints(shapeDataLocalPos);
    }

    //public void Redraw()
    //{
    //    if (shapeInfoSO = null)
    //    {
    //        Debug.LogError("[DisplaySpellDrawing]: No ShapeInfoSO scriptable object assigned.");
    //        return;
    //    }

    //    DrawShape();
    //}

    public void Redraw(ShapeInfoSO newShape)
    {
        if (newShape == null)
        {
            Debug.LogError("[DisplaySpellDrawing]: Redraw called with a null ShapeInfoSO.");
            return;
        }

        shapeInfoSO = newShape;
        DrawShape();
    }

    public void RedrawIfNew(ShapeInfoSO newShape)
    {
        if (newShape == shapeInfoSO)
            return;

        Redraw(newShape);
    }

    private IEnumerator DrawAfterLayout()
    {
        yield return new WaitForEndOfFrame();

        DrawShape();
    }
}