using UnityEngine;

public class GameConstants : MonoBehaviour
{
    #region Shape Matching settings
    public const float SampleSpeedSec = 0.01f; // Time for controller to sample next point of drawing.

    public const int PointCount = 128; // Drawing is converted to a fixed number of points for shape matching.

    // Weightage should add up to 1.
    public const float DollarWeightage = 0.75f; // Weightage for dollar recognizer method
    public const float AspectWeightage = 0.15f; // Weightage for closeness of aspect ratio
    public const float LineWeightage = 0.10f; // Weightage for closeness of line length of drawings normalized between 0 to 1

    public const float MinAccuracy = 0.70f; // Minimum accuracy to match a shape
    public const float MinAccDiff = 0.10f; // Minimum accuracy difference between best and second best shape
    #endregion

    #region Display settings
    public const bool DisplayBestShape = true;
    public const float LineWidth = 0.1f;

    public static readonly Color LeftDrawingColor = Color.red;
    public static readonly Color RightDrawingColor = Color.blue;
    public static readonly Color LeftWandModelColor = new Color(1f, 0.35f, 0.35f);
    public static readonly Color RightWandModelColor = new Color(0.35f, 0.35f, 1.8f);
    [Range(0.1f, 1f)] public const float PlayerDrawDarkenFactor = 0.7f; //player's raw stroke is darker than the system-drawn shape

    [Header("Shape Draw Animation")]
    public LineRenderer shapeLineRendererPrefab;

    public const int CatmullResolution = 10;
    public const float SharpAngleThreshold = 45f;
    #endregion

    #region Wand 3D model settings
    public const float DistanceToCamera = 10f;

    public const bool CursorVisible = false;

    public const float HorizontalFovDeg = 50f;
    public const float VerticalFovDeg = 28.125f; // 50 * (9/16), for 16:9
    #endregion

    #region Game Play settings

    public const float CoinInsertionCountdownTime = 100f;

    public const float GlobalSfxVolume = 0.08f;
    public const float GlobalMusicVolume = 0.08f;
    public const float TotalGameTime = 90f;


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
    #endregion

    public const float ControllerCallibrationTime = 2f;

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
