using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeVariant", menuName = "Shapes/Shape Variant")]
public class ShapeVariantSO : ScriptableObject
{
    [SerializeField] private Vector2[] points;

    private Vector2[] recalcAndNormPoints;
    private bool isInitialized = false;

    public Vector2[] OriginalPoints
    {
        get { return points; }
    }

    public Vector2[] Points
    {
        get
        {
            if (!isInitialized)
            {
                if (!CheckValidDrawing())
                    return null;

                CaculateNewPoints();
            }
            return recalcAndNormPoints;
        }
    }

    private void OnEnable()
    {
        isInitialized = false;
        recalcAndNormPoints = null;
    }

    public bool CheckValidDrawing()
    {
        if (points.Length <= 1)
        {
            Debug.LogWarning($"[{name}]: Not enough points to resample");
            return false;
        }

        Vector2 firstPoint = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            if (firstPoint != points[i])
                return true;
        }

        Debug.LogWarning($"[{name}]: Variant points should not be in the same place.");
        return false;
    }

    /// <summary>
    /// Lazy recaculation and normalization of original points to new points.
    /// </summary>
    private void CaculateNewPoints()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"[{name}]: GameManager Instance not found.");
            return;
        }
        if (GameManager.PointCount <= 1)
        {
            Debug.LogWarning($"[{name}]: GameManager Point Count must be greater than 1.");
        }

        recalcAndNormPoints =
            PointsManipulation.ResampleAndNormalize(
                points,
                GameManager.PointCount,
                GameManager.ConserveScale
                );

        isInitialized = true;
    }
}