using UnityEngine;

public static class GameConstants
{
    #region Shape Matching settings
    public const float sampleSpeedSec = 0.01f;
    public const int PointCount = 128;

    // Weightage should add up to 1.
    public const float DollarWeightage = 0.75f; // initial: 0.75
    public const float AspectWeightage = 0.15f; // initial: 0.15
    public const float LineWeightage = 0.10f; // initial: 0.10

    public const float MinAccuracy = 0.7f;
    public const float MinAccDiff = 0.10f;
    #endregion

    #region Display settings
    public const bool DisplayBestShape = true;
    public const float LineWidth = 0.1f;
    public const float DisplayShapePercentage = 0.75f;
    public static Vector2 DisplayShapeOffset
    {
        get { return Vector2.zero; }
    }

    public const int CatmullResolution = 10;
    public const float sharpAngleThreshold = 45f;
    #endregion

    #region Wand 3D model settings
    public const float DistanceToCamera = 5f;
    public static Vector3 QuatRotation
    {
        get { return new Vector3(0, 0, 15); }
    }

    public const bool CursorVisible = true;
    #endregion
}
