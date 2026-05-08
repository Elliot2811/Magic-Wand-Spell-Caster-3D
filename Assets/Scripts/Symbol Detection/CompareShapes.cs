using UnityEngine;

public static class CompareShapes
{
    private const float sqrt2 = 1.41421356f;

    /// <summary>
    /// Compares each and every shape variant,
    /// applies checks on player drawing to shape,
    /// and returns best shape.
    /// </summary>
    /// <returns>
    /// ShapeInfoSO object that has shape name information
    /// </returns>
    public static ShapeInfoSO FindBestMatch(Vector2[] playerPoints, ShapesStorageSO availableShapes)
    {
        //Debug.Log("CompareShapes.FindBestMatch is called");

        if (GameManager.Instance == null)
        {
            Debug.LogError("CompareShapes: GameManager instance not found");
        }

        if (
            playerPoints == null ||
            availableShapes == null ||
            playerPoints.Length <= 1
            )
            return null;

        ShapeInfoSO bestMatch = null;
        float bestShapeAccuracy = 0;
        foreach (ShapeInfoSO shape in availableShapes)
        {
            //Debug.Log($"Parsing through {shape.ShapeName}");

            foreach (ShapeVariantSO variant in shape)
            {
                float averageAcc = CaculateAverageAcc(playerPoints, variant);

                if (averageAcc > bestShapeAccuracy)
                {
                    bestMatch = shape;
                    bestShapeAccuracy = averageAcc;
                }
            }
        }

        return PassedMinAccuracy(bestShapeAccuracy) ? bestMatch : null;
    }
        
    /// <summary>
    /// Parses throught the array,
    /// gets total accuracy of playerPoints compared to shapeDrawing.
    /// Returns average (acc of all points) / (num of points)
    /// </summary>
    private static float CaculateAverageAcc(Vector2[] playerPoints, ShapeVariantSO shapeVariant)
    {
        Vector2[] shapePoints = shapeVariant.Points;
        if (shapePoints == null)
            return 0;

        if (playerPoints.Length != shapePoints.Length)
        {
            Debug.LogError($"[CompareShapes.caculateTotalAcc]: point count mismatch on {shapeVariant.name}" +
                $"(Player: {playerPoints.Length}, shape: {shapePoints.Length}).");
        }

        //Debug.Log("Starting caculation of totalAcc");
        float totalAcc = 0;
        for (int i = 0; i < shapePoints.Length; i++)
        {
            totalAcc += CaculatePointAcc(playerPoints[i], shapePoints[i], GameManager.DeviationPower);
        }

        //Debug.Log($"totalAcc: {totalAcc}");
        Debug.Log($"AverageAcc: {totalAcc / shapePoints.Length}");

        return (totalAcc / shapePoints.Length);
    }

    /// <summary>
    /// Returns accuracy / deviation of player point compared to its comparision.
    /// 0 is as far as possible, 1 is same position.
    /// </summary>
    /// <param name="pow">
    /// Changes return by math operation ^pow
    /// </param>
    private static float CaculatePointAcc(Vector2 player, Vector2 comparision, int pow)
    {
        float dist = Vector2.Distance(player, comparision);
        float acc = Mathf.Max(0f, 1f - (dist / sqrt2));
        return Mathf.Pow(acc, pow);
    }

    private static bool PassedMinAccuracy(float acc)
    {
        return (acc >= GameManager.MinAccuracy);
    }
}