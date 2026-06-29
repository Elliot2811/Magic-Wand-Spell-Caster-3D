using UnityEngine;

public class GameConstants : MonoBehaviour
{
    #region Shape Matching settings
    public const float sampleSpeedSec = 0.01f;
    public const int PointCount = 128;

    // Weightage should add up to 1.
    public const float DollarWeightage = 0.75f; // initial: 0.75
    public const float AspectWeightage = 0.15f; // initial: 0.15
    public const float LineWeightage = 0.10f; // initial: 0.10

    public const float MinAccuracy = 0.75f;
    public const float MinAccDiff = 0.10f;
    #endregion

    #region Display settings
    [Range(0f, 0.5f)]
    public const float DrawingAreaPercentage = 0.90f;

    public const bool DisplayBestShape = true;
    public const float LineWidth = 0.05f;
    public const float DisplayShapePercentage = 0.75f;
    public static readonly Vector2 DisplayShapeOffset = Vector2.zero;

    public const int CatmullResolution = 10;
    public const float sharpAngleThreshold = 45f;
    #endregion

    #region Wand 3D model settings
    public const float DistanceToCamera = 10f;
    public static Vector3 QuatRotation
    {
        get { return new Vector3(0, 0, 15); }
    }

    public const bool CursorVisible = false;

    public const float HorizontalFovDeg = 45f;
    public const float VerticalFovDeg = 30f;
    #endregion

    #region Game Play settings

    public static float coinInsertionCountdownTime = 10f;

    public static class ProjectileSpawn
    {    
        public static readonly Vector3 relativePos = new Vector3(0, 0.25f, 0.85f);
        public static readonly Quaternion relativeRotation = Quaternion.identity;
        public static readonly Vector3 scale = Vector3.one;
    }
    public static readonly Rect DrawingRectLeft = new Rect(0.05f, 0.05f, 0.425f, 0.9f);
    public static readonly Rect DrawingRectRight = new Rect(0.55f, 0.05f, 0.425f, 0.9f);
    public static readonly Rect DisplayRectLeft = new Rect(0.025f, 0.05f, 0.1f, 0.1f);
    public static readonly Rect DisplayRectRight = new Rect(0.875f, 0.05f, 0.1f, 0.1f);

    #region Lake World
    public static readonly Vector3 LakeWorldLeftPos = new Vector3(-16F, 5, 8);
    public static readonly Vector3 LakeWorldLeftRot = new Vector3(0, 90, 0);
    public static readonly Vector3 LakeWorldLeftScale = new Vector3(5f, 5f, 5f);
    public static readonly Vector3 LakeWorldRightPos = new Vector3(16F, 5, 8);
    public static readonly Vector3 LakeWorldRightRot = new Vector3(0, -90, 0);
    public static readonly Vector3 LakeWorldRightScale = new Vector3(5f, 5f, 5f);
    #endregion

    #endregion

    public const float controllerCallibrationTime = 2f;

    //-------------------------------------------------------------------------------------------------------------
    public static GameConstants Instance;

    public MapData[] mapPresets;
    public CharacterEntity characterPrefab;

    public SpellProjectileLookUpTable lookUpTable;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
