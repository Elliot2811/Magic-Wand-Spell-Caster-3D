using UnityEngine;

public static class LineRendererInterface
{
    public static void UpdateLineRenderer(LineRenderer lineRenderer, Vector2[] points)
    {
        lineRenderer.positionCount = points.Length;

        for (int i = 0; i <  points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}