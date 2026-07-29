using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : MaskableGraphic
{
    [SerializeField]
    private float thickness = 1f;

    private List<Vector2> points = new List<Vector2>();

    public float Thickness
    {
        get => thickness;
        set
        {
            thickness = value;
            SetVerticesDirty();
        }
    }

    public void SetPoints(Vector2[] newPoints)
    {
        points.Clear();
        if (newPoints != null)
            points.AddRange(newPoints);

        SetVerticesDirty();
    }

    public void Clear()
    {
        points.Clear();
        SetVerticesDirty();
    }

    private void Start()
    {
        transform.SetAsLastSibling();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points.Count < 2)
            return;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 p0 = points[i];
            Vector2 p1 = points[i + 1];

            Vector2 dir = (p1 - p0).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

            int idx = vh.currentVertCount;

            vh.AddVert(p0 - normal, color, Vector2.zero);
            vh.AddVert(p0 + normal, color, Vector2.zero);
            vh.AddVert(p1 + normal, color, Vector2.zero);
            vh.AddVert(p1 - normal, color, Vector2.zero);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx + 2, idx + 3, idx);

            if (i > 0)
            {
                Vector2 prevDir = (p0 - points[i - 1]).normalized;
                Vector2 prevNormal = new Vector2(-prevDir.y, prevDir.x) * (thickness * 0.5f);

                int jointIdx = vh.currentVertCount;
                vh.AddVert(p0 - prevNormal, color, Vector2.zero);
                vh.AddVert(p0 + prevNormal, color, Vector2.zero);
                vh.AddVert(p0 + normal, color, Vector2.zero);
                vh.AddVert(p0 - normal, color, Vector2.zero);

                vh.AddTriangle(jointIdx, jointIdx + 1, jointIdx + 2);
                vh.AddTriangle(jointIdx + 2, jointIdx + 3, jointIdx);
            }
        }
    }
}