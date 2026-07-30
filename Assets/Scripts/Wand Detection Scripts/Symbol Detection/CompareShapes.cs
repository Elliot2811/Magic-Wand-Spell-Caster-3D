using System.Collections.Generic;
using UnityEngine;


// TODO: REMOVE REUSED FINDBESTMATCH CODE AFTER DEPENDENCY ON SHAPESSOTRAGESO REMOVAL
public static class CompareShapes
{
    public struct ShapeMatchResult
    {
        public ShapeInfoSO shape;
        public float accuracy;
    }

    private const float sqrt2 = 1.41421356f;

    /// <summary>
    /// Compares each and every shape variant,
    /// applies checks on player drawing to shape,
    /// and returns best shape.
    /// </summary>
    /// <param name="availableShapes">
    /// Accepts any enumarable collection of ShapeInfoSO, such as ShapesStorageSO or ShapeInfoSO array
    /// </param>
    /// <returns>
    /// ShapeInfoSO object that has shape name information
    /// </returns>
    public static ShapeMatchResult FindBestMatch(Vector2[] playerPoints, IEnumerable<ShapeInfoSO> availableShapes)
    {
        ShapeMatchResult result = new ShapeMatchResult
        {
            shape = null,
            accuracy = 0
        };

        if (
            playerPoints == null ||
            availableShapes == null ||
            playerPoints.Length <= 1
            )
            return result;

        ShapeInfoSO bestMatch = null;
        float bestShapeAccuracy = 0;
        float secondBestShapeAccuracy = 0;

        foreach (ShapeInfoSO shape in availableShapes)
        {
            foreach (ShapeVariantSO variant in shape)
            {
                float averageAcc = CaculateScore(playerPoints, variant, shape.RotSymmetries);

                if (averageAcc > bestShapeAccuracy)
                {
                    bestMatch = shape;
                    bestShapeAccuracy = averageAcc;
                }
                else if (shape != bestMatch && averageAcc > secondBestShapeAccuracy)
                {
                    secondBestShapeAccuracy = averageAcc;
                }
            }
        }

        result.accuracy = bestShapeAccuracy;

        if (PassedMinAccuracy(bestShapeAccuracy) && UniqueEnough(bestShapeAccuracy, secondBestShapeAccuracy))
        {
            result.shape = bestMatch;
        }

        return result;
    }

    /// <summary>
    /// Parrses through player drawing, getting total accuracy of points
    /// Acc: Dollar Score + Length Score
    /// </summary>
    private static float CaculateScore(Vector2[] playerPoints, ShapeVariantSO variant, int rotSymmetries)
    {
        if (variant == null)
        {
            Debug.LogError("[CompareShapes]: variant is null");
            return 0f;
        }

        Vector2[] shapePoints = variant.Points;

        if (shapePoints == null)
        {
            Debug.LogWarning($"[CompareShapes]: {variant.name} has null Points array");
            return 0f;
        }

        if (playerPoints.Length != shapePoints.Length)
        {
            Debug.LogError($"[CompareShapes]: point count mismatch on {variant.name} (Player: {playerPoints.Length}, Shape: {shapePoints.Length})");
            return 0f;
        }

        float dollarScore = DollarScore(playerPoints, shapePoints, rotSymmetries);
        float aspectScore = AspectRatioScore(playerPoints, shapePoints);
        float lengthScore = LengthScore(playerPoints, shapePoints);

        return
            (dollarScore * GameConstants.DollarWeightage) +
            (aspectScore * GameConstants.AspectWeightage) +
            (lengthScore * GameConstants.LineWeightage);
    }


    #region Dollar Recognizer
    /// <summary>
    /// Within range of 45deg and -45deg, does binary search for the best accuracy.
    /// </summary>
    private static float DollarScore(Vector2[] player, Vector2[] shape, int rotSymmetries)
    {
        if (rotSymmetries == 0)
            return 0f;

        float symmetricAngle = (2 * Mathf.PI) / rotSymmetries;
        float angleBounds = (rotSymmetries < 8) ? Mathf.PI / 4 : symmetricAngle;

        float bestDist = float.MaxValue;

        for (int i = 0; i < rotSymmetries; i++)
        {
            bestDist = Mathf.Min(
                bestDist,
                GoldenSectionSearch(player, RotateBy(shape, i * symmetricAngle), -angleBounds, angleBounds)
                );
        }


        return 1f - (float)(bestDist / (0.5 * sqrt2));
    }

    // The binary search function to check closer to a or b
    private static float GoldenSectionSearch(Vector2[] player, Vector2[] shape, float a, float b)
    {
        float phi = 0.5f * (-1 + Mathf.Sqrt(5f));
        float x1 = phi * a + (1f - phi) * b;
        float x2 = (1 - phi) * a + (1f - phi) * b;
        float f1 = PathDistance(RotateBy(player, x1), shape);
        float f2 = PathDistance(RotateBy(player, x2), shape);

        while (Mathf.Abs(b - a) > 0.0001f) // Check diff, escape if tiny
        {
            if (f1 < f2)
            {
                b = x2;
                x2 = x1;
                f2 = f1;

                x1 = phi * a + (1f - phi) * b;
                f1 = PathDistance(RotateBy(player, x1), shape);
            }
            else
            {
                a = x1;
                x1 = x2;
                f1 = f2;

                x2 = (1f - phi) * a + phi * b;
                f2 = PathDistance(RotateBy(player, x2), shape);
            }
        }

        return Mathf.Min(f1, f2);
    }

    private static float PathDistance(Vector2[] a, Vector2[] b)
    {
        float d = 0f;
        
        for (int i = 0; i < a.Length; i++)
        {
            d += Vector2.Distance(a[i], b[i]);
        }

        return d / a.Length;
    }

    private static Vector2[] RotateBy(Vector2[] points, float angle)
    {
        if (angle == 0)
            return points;

        Vector2 centroid = Centroid(points);
        Vector2[] result = new Vector2[points.Length];

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 p = points[i] - centroid;
            result[i] = new Vector2(
                p.x * cos - p.y * sin,
                p.x * sin + p.y * cos
                ) + centroid;
        }

        return result;
    }

    private static Vector2 Centroid(Vector2[] points)
    {
        Vector2 c = Vector2.zero;

        foreach (Vector2 point in points)
        {
            c += point;
        }

        return c / points.Length;
    }
    #endregion

    private static float AspectRatioScore(Vector2[] player, Vector2[] shape)
    {
        PointsManipulation.PointBounds playerBounds = PointsManipulation.GetBounds(player);
        float playerRatio =
            (playerBounds.Height != 0) ? 
            playerBounds.Height / playerBounds.Width :
            0;

        PointsManipulation.PointBounds shapeBounds = PointsManipulation.GetBounds(shape);
        float shapeRatio =
            (shapeBounds.Height != 0) ?
            shapeBounds.Height / shapeBounds.Width :
            0;

        if (shapeRatio == 0)
            return 0;

        return 
            Mathf.Min(playerRatio, shapeRatio) /
            Mathf.Max(playerRatio, shapeRatio);
    }

    /// <summary>
    /// For both length of drawings, compare shorter to longer drawing.
    /// If same/close length would return 1.
    /// </summary>
    private static float LengthScore(Vector2[] player, Vector2[] shape)
    {
        float playerLength = PointsManipulation.TotalDistance(player);
        float shapeLength = PointsManipulation.TotalDistance(shape);

        if (shapeLength == 0)
            return 0;

        return
            Mathf.Min(playerLength, shapeLength) /
            Mathf.Max(playerLength, shapeLength);
    }

    private static bool PassedMinAccuracy(float acc) => acc >= GameConstants.MinAccuracy;

    private static bool UniqueEnough(float bestAcc, float secondBestAcc) => (bestAcc - secondBestAcc) > GameConstants.MinAccDiff;

    public static float GetDamageMultiplier(float acc)
    {
        float percent = acc * 100f;

        if (percent < 30f)
            return 0f;

        if (percent < 50f)
            return Mathf.Lerp(0f, 0.2f, (percent - 30f) / 20f);

        if (percent < 60f)
            return Mathf.Lerp(0.2f, 0.5f, (percent - 50f) / 10f);

        if (percent < 75f)
            return Mathf.Lerp(0.5f, 0.7f, (percent - 60f) / 15f);

        if (percent < 90f)
            return Mathf.Lerp(0.7f, 1f, (percent - 75f) / 15f);

        return 1f;
    }

    public static string GetFeedback(float accuracy)
    {
        float percent = accuracy * 100f;

        if (percent < 70f)
            return "Miss";

        if (percent <= 75f)
            return "OK";

        if (percent <= 80f)
            return "Good";

        if (percent <= 90f)
            return "Amazing";

        return "Perfect";
    }
}