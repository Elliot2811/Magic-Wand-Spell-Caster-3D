using System.Collections.Generic;
using UnityEngine;

public static class PointsManipulation
{
    #region Readonly Structs - Streamline Dataflow
    /// <summary>
    /// Contains Width and Height.
    /// Contains Left, Right, Top, and Bottom.
    /// </summary>
    public readonly struct PointBounds
    {
        public readonly float MinX, MaxX, MinY, MaxY;

        public float Width => MaxX - MinX;
        public float Height => MaxY - MinY;

        public PointBounds(float minX, float maxX, float minY, float maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }
    #endregion


    #region Resampling Points
    /// <summary>
    /// Resamples original points to
    /// new points of equal distances along segments of the original points.
    /// </summary>
    public static Vector2[] ResamplePoints(Queue<Vector2> points, int newNumberOfPoints)
    {
        if (points != null)
            return ResamplePoints(points.ToArray(), newNumberOfPoints);

        Debug.LogWarning("[PointsManipulation.ResamplePoints]: points not found.");
        return null;
    }
    /// <summary>
    /// Resamples original points to
    /// new points of equal distances along segments of the original points.
    /// </summary>
    public static Vector2[] ResamplePoints(Vector2[] points, int newNumberOfPoints)
    {
        if (points == null || points.Length <= 1)
        {
            Debug.LogWarning("[PointsManipulation.ResamplePoints]: points not found.");
            return null;
        }
        if (points.Length <= 1)
        {
            Debug.LogWarning("[PointsManipulation.ResamplePoints]: points.Length should be more than 1.");
            return null;
        }

        float totalDistance = TotalDistance(points);
        if (totalDistance <= 0)
        {
            Debug.LogWarning("[PointsManipulation.ResamplePoints]: totalDistance should be more than 0.");
            return null;
        }

        float distanceBetweenPoints = totalDistance / (newNumberOfPoints - 1);
        if (distanceBetweenPoints <= 0)
        {
            Debug.LogWarning("[PointsManipulation.ResamplePoints]: distanceBetweenPoints sould be more than 0.");
            return null;
        }

        return BuildResampledArray(points, newNumberOfPoints, distanceBetweenPoints);
    }

    /// <summary>
    /// Helper function to create and call functions to build Resampled Points Array
    /// </summary>
    private static Vector2[] BuildResampledArray(Vector2[] points, int count, float spacing)
    {
        Vector2[] resampledPoints = new Vector2[count];
        resampledPoints[0] = points[0];

        int pointsPassed = 1;
        float overflow = 0f;

        for (int i = 0; i < points.Length - 1; i++)
        {
            bool reachedMax = StepAlongSegment(
                points[i],
                points[i + 1],
                spacing,
                resampledPoints,
                ref pointsPassed,
                ref overflow);

            if (reachedMax) break;
        }

        EndPointCap(resampledPoints, points[points.Length - 1]);
        return resampledPoints;
    }

    /// <summary>
    /// Steps from one point to the other,
    /// and store new points based on spacing.
    /// </summary>
    private static bool StepAlongSegment(
        Vector2 start,
        Vector2 end,
        float spacing,
        Vector2[] resampledPoints,
        ref int pointsPassed,
        ref float overflow
        )
    {
        float remainingDistance = Vector2.Distance(start, end);

        while (remainingDistance + overflow >= spacing )
        {
            if (pointsPassed >= resampledPoints.Length)
            {
                // Float precision might result in extra points
                return true;
            }

            float step = spacing - overflow;

            start = PosInDirection(start, end, step);
            resampledPoints[pointsPassed++] = start;

            overflow = 0f;
            remainingDistance = Vector2.Distance(start, end);
        }

        overflow += remainingDistance;
        return false;
    }

    /// <summary>
    /// Find next pos in direction of start toward end of distance away
    /// </summary>
    private static Vector2 PosInDirection(Vector2 start, Vector2 end, float distance)
    {
        Vector2 direction = (end - start).normalized;
        return (start + direction * distance);
    }

    /// <summary>
    /// Place a point at the end of new points if
    /// float accuracy droppoff in previous seccion.
    /// </summary>
    private static void EndPointCap(Vector2[] resampledPoints, Vector2 lastOriginalPoint)
    {
        if (resampledPoints[resampledPoints.Length - 1] == Vector2.zero)
            resampledPoints[resampledPoints.Length - 1] = lastOriginalPoint;
    }
    #endregion


    #region Normalizing Points
    /// <summary>
    /// Normalize the Vector2 x and y values to be between 0 to 1.
    /// </summary>
    /// <param name="preserveScale">
    /// Drawing keeps scale (true). Drawing is stretched (false).
    /// </param>
    public static Vector2[] NormalizePoints(Queue<Vector2> points, bool preserveScale)
    {
        if (points != null)
            return NormalizePoints(points.ToArray(), preserveScale);

        Debug.LogWarning("[PointsManipulation.NormalizePoints]: points not found.");
        return null;
    }
    /// <summary>
    /// Normalize the Vector2 x and y values to be between 0 to 1.
    /// </summary>
    /// <param name="preserveScale">
    /// Drawing keeps scale (true). Drawing is stretched (false).
    /// </param>
    public static Vector2[] NormalizePoints(Vector2[] points, bool preserveScale)
    {
        if (points == null)
        {
            Debug.LogWarning("[PointsManipulation.NormalizePoints]: points not found.");
            return null;
        }

        if (points.Length == 0)
        {
            Debug.LogWarning("[PointsManipulation.NormalizePoints]: points.Length should be more than 0.");
            return null;
        }

        PointBounds bounds = GetBounds(points);

        return (preserveScale) ?
            NormalizeKeepScale(points, bounds) :
            NormalizeStretch(points, bounds);
    }

    /// <summary>
    /// Normalize the Vector2 x and y values to be between 0 to 1.
    /// Keep proportions of drawing.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="bounds"></param>
    /// <returns></returns>
    private static Vector2[] NormalizeKeepScale(Vector2[] points, PointBounds bounds)
    {
        float scale = Mathf.Max(bounds.Width, bounds.Height);

        if (scale == 0)
        {
            Debug.LogWarning("[PointsManipulation.NormalizeKeepScale] Scale should not be 0.");
            return null;
        }

        Vector2[] results = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = points[i];
            results[i] = new Vector2(
                (point.x - bounds.MinX) / scale,
                (point.y - bounds.MinY) / scale
                );
        }

        return results;
    }

    /// <summary>
    /// Normalize the Vector2 x and y values to be between 0 to 1.
    /// Stretches drawing.  
    /// </summary>
    private static Vector2[] NormalizeStretch(Vector2[] points, PointBounds bounds)
    {
        if (bounds.Width == 0 && bounds.Height == 0)
        {
            Debug.LogWarning("[PointsManipulation.NormalizeStretch]: width or height should not be 0.");
            return null;
        }

        float scaleX = (bounds.Width == 0 ? 1f : bounds.Width);
        float scaleY = (bounds.Height == 0 ? 1f : bounds.Height);

        Vector2[] results = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = points[i];
            results[i] = new Vector2(
                (point.x - bounds.MinX) / scaleX,
                (point.y - bounds.MinY) / scaleY
                );
        }

        return results;
    }
    #endregion


    #region Miscalleneous Functions

    /// <summary>
    /// Resamples original points to
    /// new points of equal distances along segments of the original points.
    /// And normalize the Vector2 x and y values to be between 0 to 1.
    /// </summary>
    /// <param name="preserveScale">
    /// Drawing keeps scale (true). Drawing is stretched (false).
    /// </param>
    public static Vector2[] ResampleAndNormalize(Queue<Vector2> points, int newNumberOfPoints, bool preserveScale)
    {
        if (points == null)
        {
            Debug.LogWarning("[PointsManipulation.ResampleAndNormalize]: points not found.");
            return null;
        }

        return ResampleAndNormalize(points.ToArray(), newNumberOfPoints, preserveScale);
    }
    /// <summary>
    /// Resamples original points to
    /// new points of equal distances along segments of the original points.
    /// And normalize the Vector2 x and y values to be between 0 to 1.
    /// </summary>
    /// <param name="preserveScale">
    /// Drawing keeps scale (true). Drawing is stretched (false).
    /// </param>
    public static Vector2[] ResampleAndNormalize(Vector2[] points, int newNumberOfPoints, bool preserveScale)
    {
        if (points == null)
        {
            Debug.LogWarning("[PointsManipulation.ResampleAndNormalize]: points not found.");
            return null;
        }

        Vector2[] newPoints = ResamplePoints(points, newNumberOfPoints);
        return NormalizePoints(newPoints, preserveScale);
    }

    /// <summary>
    /// Returns information about the drawing boundaries.
    /// </summary>
    public static PointBounds GetBounds(Vector2[] points)
    {
        Vector2 first = points[0];

        float maxX = first.x, minX = first.x;
        float maxY = first.y, minY = first.y;

        foreach (Vector2 point in points)
        {
            if (point.x > maxX) maxX = point.x;
            if (point.x < minX) minX = point.x;
            if (point.y > maxY) maxY = point.y;
            if (point.y < minY) minY = point.y;
        }

        return new PointBounds(minX, maxX, minY, maxY);
    }

    /// <summary>
    /// Total distance between Vector2 points
    /// </summary>
    public static float TotalDistance(Vector2[] points)
    {
        if (points.Length <= 1)
        {
            Debug.LogWarning("[PointsManipulation.TotalDistance]: points.Length should be more than 1.");
            return 0;
        }

        float totalDistance = 0f;

        Vector2 previousPoint = points[0];

        for (int i = 1; i <  points.Length; i++)
        {
            totalDistance += Vector2.Distance(previousPoint, points[i]);
            previousPoint = points[i];
        }

        return totalDistance;
    }

    /// <summary>
    /// Prints points in rows of 5 
    /// </summary>
    public static void PrintPoints(Vector2[] points)
    {
        string result = "Points:";

        if (points == null)
        {
            Debug.Log(result);
            return;
        }

        for (int i = 0;  i < points.Length; i++)
        {
            if (i % 5 == 0)
            {
                result += "\n";
            }

            Vector2 point = points[i];

            result += $"({point.x}, {point.y}),\t ";
        }

        Debug.Log(result);
    }
    #endregion
}