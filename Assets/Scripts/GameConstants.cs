using UnityEngine;

public static class GameConstants
{
    #region Shape Matching settings
    public const float sampleSpeedSec = 0.01f;
    public const int PointCount = 128;
    public const float MinAccuracy = 0.7f;
    public const float DollarPercent = 0.85f; // Orignial dollar %: 80%, original line %: 20%
    #endregion

    #region Display settings
    public const bool DisplayBestShape = true;
    public const float LineWidth = 0.1f;
    public const float DisplayShapePercentage = 0.5f;
    public static Vector2 DisplayShapeOffset
    {
        get { return Vector2.zero; }
    }
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
