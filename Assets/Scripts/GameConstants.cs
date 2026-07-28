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

    public const float MinAccuracy = 0.70f;
    public const float MinAccDiff = 0.10f;
    #endregion

    #region Display settings
    [Range(0f, 0.5f)]
    public const float DrawingAreaPercentage = 0.90f;

    public const bool DisplayBestShape = true;
    public const float LineWidth = 0.1f;
    public const float DisplayShapePercentage = 0.75f;
    public static readonly Vector2 DisplayShapeOffset = Vector2.zero;

    public static readonly Color LeftDrawingColor = Color.red;
    public static readonly Color RightDrawingColor = Color.blue;
    [Range(0.1f, 1f)] public const float PlayerDrawDarkenFactor = 0.7f; //player's raw stroke is darker than the system-drawn shape

    [Header("Shape Draw Animation")]
    public LineRenderer shapeLineRendererPrefab;

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

    public const float HorizontalFovDeg = 50f;
    public const float VerticalFovDeg = 28.125f; // 50 * (9/16), for 16:9
    #endregion

    #region Game Play settings

    public const float coinInsertionCountdownTime = 100f;

    public const float globalSfxVolume = 0.08f;
    public const float globalMusicVolume = 0.08f;


    public static class ProjectileSpawn
    {
        public static readonly Vector3 shieldRelativePos = new Vector3(3.2f, 0, 0f);
        public static readonly Vector3 shieldScale = new Vector3(1f, 1f, 1f); 
        public static readonly Vector3 spellRelativePos = new Vector3(3.4f, 0.25f, 1f);
        public static readonly Vector3 spellScale = new Vector3(1f, 1f, 1f);
        public static readonly Vector3 spellChargeEffectScale = new Vector3(6.5f, 6.5f ,1f);
        public static readonly Quaternion relativeRotation = Quaternion.identity;
    }
    public static readonly Rect DrawingRectLeft = new Rect(0.05f, 0.05f, 0.425f, 0.9f);
    public static readonly Rect DrawingRectRight = new Rect(0.525f, 0.05f, 0.425f, 0.9f);
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

    [Tooltip("Seconds for the matched spell shape to fully draw out.")]
    [SerializeField] private float shapeDrawDuration = 0.8f;
    public float ShapeDrawDuration => shapeDrawDuration;

    //-------------------------------------------------------------------------------------------------------------
    public static GameConstants Instance;

    public MapData[] mapPresets;
    //public CharacterEntity characterPrefab;
    public CharacterEntity player1Prefab;
    public CharacterEntity player2Prefab;

    public SpellProjectileLookUpTable lookUpTable;
    public ShapesCollectionSO allShapes;

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
