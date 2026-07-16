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
    public static Vector2[] ResamplePoints(List<Vector2> points, int newNumberOfPoints)
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
    /// If axis not spanning 0-1, centers the drawing.
    /// </summary>
    public static Vector2[] NormalizePoints(List<Vector2> points)
    {
        if (points != null)
            return NormalizePoints(points.ToArray());

        Debug.LogWarning("[PointsManipulation.NormalizePoints]: points not found.");
        return null;
    }
    /// <summary>
    /// Normalize the Vector2 x and y values to be between 0 to 1.
    /// If axis not spanning 0-1, centers the drawing.
    /// </summary>
    public static Vector2[] NormalizePoints(Vector2[] points)
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

        float scale = Mathf.Max(bounds.Width, bounds.Height);

        if (scale == 0)
        {
            Debug.LogWarning("[PointsManipulation.NormalizeKeepScale] Scale should not be 0.");
            return null;
        }

        bool flatX = bounds.Width == 0;
        bool flatY = bounds.Height == 0;

        float offsetX = flatX ? 0f : (scale - bounds.Width) / (2f * scale);
        float offsetY = flatY ? 0f : (scale - bounds.Height) / (2f * scale);

        Vector2[] results = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = points[i];
            results[i] = new Vector2(
                flatX ? 0.5f : offsetX + (point.x - bounds.MinX) / scale,
                flatY ? 0.5f : offsetY + (point.y - bounds.MinY) / scale
                );
        }

        //PrintPoints(results);

        return results;
    }
    #endregion


    #region CatmullRom Points
    /// <summary>
    /// Creates the catmull segments for the entire drawing
    /// </summary>
    public static Vector2[] CatmullRomLine(Vector2[] points)
    {
        if (points.Length < 4) return points;

        List<Vector2> catmullPoints = new List<Vector2>();
        List<Vector2[]> renderedPoints = new List<Vector2[]>();

        catmullPoints.Add(points[0]);
        catmullPoints.Add(points[0]);

        for (int i = 2; i < points.Length;i++)
        {
            catmullPoints.Add(points[i - 1]);
            if (Angle(points[i - 2], points[i - 1], points[i]) > GameConstants.sharpAngleThreshold)
                catmullPoints.Add(points[i - 1]);
        }

        catmullPoints.Add(points[^1]);
        catmullPoints.Add(points[^1]);


        for (int i = 3; i < catmullPoints.Count; i++)
        {
            renderedPoints.Add(
                EvaluateCatmullSegment(
                    catmullPoints[i - 3],
                    catmullPoints[i - 2],
                    catmullPoints[i - 1],
                    catmullPoints[i],
                    GameConstants.CatmullResolution
                    ));
        }

        List<Vector2> result = new List<Vector2>();

        foreach (Vector2[] segment in renderedPoints)
        {
            foreach (Vector2 point in segment)
            {
                result.Add(point);
            }
        }

        return result.ToArray();
    }

    public static float Angle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 enterDir = p2 - p1;
        Vector2 endDir = p3 - p2;

        return Vector2.Angle(enterDir, endDir);
    }

    /// <summary>
    /// Evaluates A Catmull-Rom segment
    /// </summary>
    public static Vector2[] EvaluateCatmullSegment(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int resolution)
    {
        if (p1 == p2)
            return new Vector2[]{p1};

        Vector2[] segment = new Vector2[resolution];

        for (int step = 0; step < resolution; step++)
        {
            float t = step / (float)(resolution - 1);
            segment[step] = CatmullRomPoint(p0, p1, p2, p3, t);
        }

        return segment;
    }

    /// <summary>
    /// Finds a single Catmull-Rom point at t between p1 and p2.
    /// </summary>
    public static Vector2 CatmullRomPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        if (p1 == p2)
            return p1;

        return 0.5f * (
            2f * p1                             +
            (-p0 + p2)                          * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3)  * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3)      * t * t * t
            );
    }
    #endregion


    #region Miscalleneous Functions

    /// <summary>
    /// Resamples original points to
    /// new points of equal distances along segments of the original points.
    /// And normalize the Vector2 x and y values to be between 0 to 1.
    /// </summary>
    /// <param name="preserveScale">
    /// Drawing keeps spellScale (true). Drawing is stretched (false).
    /// </param>
    public static Vector2[] ResampleAndNormalize(List<Vector2> points, int newNumberOfPoints)
    {
        if (points == null)
        {
            Debug.LogWarning("[PointsManipulation.ResampleAndNormalize]: points not found.");
            return null;
        }

        return ResampleAndNormalize(points.ToArray(), newNumberOfPoints);
    }
    /// <summary>
    /// Resamples original points to
    /// new points of equal distances along segments of the original points.
    /// And normalize the Vector2 x and y values to be between 0 to 1.
    /// </summary>
    /// <param name="preserveScale">
    /// Drawing keeps spellScale (true). Drawing is stretched (false).
    /// </param>
    public static Vector2[] ResampleAndNormalize(Vector2[] points, int newNumberOfPoints)
    {
        if (points == null)
        {
            Debug.LogWarning("[PointsManipulation.ResampleAndNormalize]: points not found.");
            return null;
        }

        Vector2[] newPoints = ResamplePoints(points, newNumberOfPoints);
        return NormalizePoints(newPoints);
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
    public static PointBounds GetBounds(Vector3[] points)
    {
        Vector3 first = points[0];

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
    public static float TotalDistance(Vector3[] points)
    {
        if (points.Length <= 1)
        {
            Debug.LogWarning("[PointsManipulation.TotalDistance]: points.Length should be more than 1.");
            return 0;
        }

        float totalDistance = 0f;

        Vector2 previousPoint = points[0];

        for (int i = 1; i < points.Length; i++)
        {
            totalDistance += Vector2.Distance(previousPoint, points[i]);
            previousPoint = points[i];
        }

        return totalDistance;
    }

    /// <summary>
    /// Move the center of the points to (0.5, 0.5).
    /// </summary>
    public static Vector2[] CenterPoints(Vector2[] points)
    {
        if (points == null || points.Length == 0)
            return null;

        PointBounds bounds = GetBounds(points);
        Vector2 center = new Vector2(
            (bounds.MinX + bounds.MaxX) / 2,
            (bounds.MinY + bounds.MaxY) / 2
            );

        Vector2 offset = new Vector2(0.5f, 0.5f) - center;

        Vector2[] results = new Vector2[points.Length];

        for (int i = 0; i <  points.Length; i++)
        {
            results[i] = points[i] + offset;
        }

        return results;
    }

    #region Scale To Screen Functions
    /// <summary>
    /// Rescales points to certain area of the viewPort.
    /// </summary>
    public static Vector3[] ScaleToViewPort(Vector2[] points, Camera mainCamera, float renderDistance, Rect viewportRect, float displayPercent)
    {
        if (points == null || points.Length <= 1)
            return null;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(
            viewportRect.xMin,
            viewportRect.yMin,
            renderDistance
            ));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(
            viewportRect.xMax,
            viewportRect.yMax,
            renderDistance
            ));

        float rectWorldWidth = topRight.x - bottomLeft.x;
        float rectWorldHeight = topRight.y - bottomLeft.y;

        float displayWidth = rectWorldWidth * displayPercent;
        float displayHeight = rectWorldHeight * displayPercent;

        Vector2 rectWorldCentre = new Vector2(
            (bottomLeft.x + topRight.x) * 0.5f,
            (bottomLeft.y + topRight.y) * 0.5f
            );

        float scale = Mathf.Min(displayWidth, displayHeight);

        Vector2[] centred = CenterPoints(points);

        Vector3[] results = new Vector3[centred.Length];
        for (int i = 0; i < points.Length; i++)
        {
            results[i] = new Vector3(
                (centred[i].x - 0.5f) * scale + rectWorldCentre.x,
                (centred[i].y - 0.5f) * scale + rectWorldCentre.y,
                bottomLeft.z
                );
        }

        return results;
    }
    #endregion

    /// <summary>
    /// Prints points to the console in rows of 5 
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

            result += $"({point.x}, {point.y})".PadRight(10);
        }

        Debug.Log(result);
    }
    #endregion
}