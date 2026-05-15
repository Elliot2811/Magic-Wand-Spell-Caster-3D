using System.Collections.Generic;
using UnityEngine;

public static class LineRendererInterface
{
    public static void ResetLineRenderer(LineRenderer lineRenderer) => lineRenderer.positionCount = 0;

    public static void Points(LineRenderer lineRenderer, List<Vector2> points)
    {
        lineRenderer.positionCount = points.Count;

        int i = 0;

        foreach (Vector2 point in points)
        {
            lineRenderer.SetPosition(i++, point);
        }
    }

    public static void Points(LineRenderer lineRenderer, Vector2[] points)
    {
        lineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    public static void AddPoints(LineRenderer lineRenderer, List<Vector2[]> points)
    {
        int total = 0;
        foreach (Vector2[] seg in points)
            total += seg.Length;

        int i = lineRenderer.positionCount;

        lineRenderer.positionCount = i + total;

        foreach (Vector2[] seg in points)
        {
            foreach (Vector2 point in seg)
            {
                lineRenderer.SetPosition(i++, point);
            }
        }
    }

    //public static void AddPoint(LineRenderer lineRenderer, Vector2 point)
    //{
    //    int total = lineRenderer.positionCount;
    //    lineRenderer.positionCount += 1;

    //    lineRenderer.SetPosition(total, point);
    //}

    //public static void RemovePoint(LineRenderer lineRenderer)
    //{
    //    if (lineRenderer.positionCount > 0) lineRenderer.positionCount -= 1;
    //}
}