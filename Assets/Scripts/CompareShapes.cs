using UnityEngine;

public class CompareShapes
{
    #region Variables

    #region Static

    private static Vector2[][][] shapesData;

    #endregion

    #region Constant
    private const float sqrt2 = 1.41421356f;
    #endregion

    #region Instantiated Variables

    private Vector2[] userPoints;
    private string[] shapeNames;

    private float minAccuracy = 1;
    #endregion

    #region Non-Instantiated Variables
    private string type = "Unknown";
    #endregion

    #endregion

    #region Getter Setters
    public string ShapeType
    {
        get
        {
            if (type == "Unknown" || type == "")
            {
                FindBestMatch();
            }

            return type;
        }
    }
    #endregion

    #region Constructors
    private CompareShapes(Vector2[] userPoints, string[] shapeNames)
    {
        this.userPoints = userPoints;
        this.shapeNames = shapeNames;
    }

    public CompareShapes(Vector2[] userPoints, Vector2[][][] allShapesData, string[] shapeNames) : this(userPoints, shapeNames)
    {
        shapesData = allShapesData;
    }
    public CompareShapes(Vector2[] userPoints, Vector2[][][] allShapesData, string[] shapeNames, float minAccuracy) : this(userPoints, allShapesData, shapeNames)
    {
        this.minAccuracy = minAccuracy;
    }
    #endregion

    #region Functions
    public string FindBestMatch()
    {
        if (
            shapesData == null || shapesData.Length == 0 ||
            userPoints == null || userPoints.Length == 0
            )
        {
            type = "None";
            return type;
        }

        float[] shapeAcc = new float[shapesData.Length];

        for (int shapeId = 0; shapeId <  shapesData.Length; shapeId++)
        {
            if (shapesData[shapeId] == null)
                continue;

            for (int variantId = 0; variantId < shapesData[shapeId].Length; variantId++)
            {
                if (shapesData[shapeId][variantId] == null)
                    continue;

                int pointCount = Mathf.Min(
                userPoints.Length,
                shapesData[shapeId][variantId].Length
                );

                if (pointCount == 0) continue;

                float totalAcc = 0;

                for (int pointId = 0; pointId < pointCount; pointId++) // Fixed
                {
                    totalAcc += CaculatePointAcc(
                        userPoints[pointId],
                        shapesData[shapeId][variantId][pointId]
                    );
                }

                totalAcc /= pointCount;
                if (totalAcc > shapeAcc[shapeId])
                {
                    shapeAcc[shapeId] = totalAcc;
                }
            }
        }

        DetermineBestMatch(shapeAcc);
        return type;
    }

    private float CaculatePointAcc(Vector2 user, Vector2 orginal)
    {
        // This might be too basic
        //return 1 - (Vector2.Distance(user, orginal) / sqrt2);

        // Penalty weighted score
        float dist = Vector2.Distance(user, orginal);
        float acc = Mathf.Max(0f, 1f - (dist / sqrt2));
        return acc * acc;
    }

    private void DetermineBestMatch(float[] shapeAcc)
    {
        if (shapeAcc == null || shapeAcc.Length == 0)
        {
            return;
        }

        int highestIndex = 0;
        for (int i = 1; i < shapeAcc.Length; i++)
        {
            if (shapeAcc[i] > shapeAcc[highestIndex])
            {
                highestIndex = i;
            }
        }

        if (highestIndex >= shapeNames.Length)
        {
            type = "None";
            return;
        }

        Debug.Log($"Accuracy: {shapeAcc[highestIndex]} for the shape {shapeNames[highestIndex]}");

        if (shapeAcc[highestIndex] >= minAccuracy)
        {
            type = shapeNames[highestIndex];
            return;
        }

        type = "None";
    }
    #endregion
}