using UnityEngine;

public class SpellBook : MonoBehaviour
{
    [SerializeField]
    private ShapesStorageSO shapesCollection;

    [SerializeField]
    private Wand wand;

    [SerializeField]
    private EntityBase playerEntity;

    public ShapeInfoSO shape1;
    public ShapeInfoSO shape2;

    public ScriptableObjectSpells spell1;
    public ScriptableObjectSpells spell2;

    private bool newData = true;
    private Vector2[] playerPoints;

    private void Awake()
    {
        if (wand == null)
        {
            Debug.LogError($"[{nameof(SpellBook)}]: No reference to {nameof(Wand)} found on {gameObject.name}.");
            this.gameObject.SetActive(false);
        }

        //Temp area to test the lookup table
        LookUpTable.dict.Add(shape1, spell1);
        LookUpTable.dict.Add(shape2, spell2);
    }


    private void Update()
    {
        if (!newData)
            return;

        newData = false;
        ShapeInfoSO shapeInfo = CompareShapeDrawing();

        ScriptableObjectSpells spell = findProjectile(shapeInfo);

        if (spell != null)
        {
            playerEntity.FireSpell(spell);
        }
    }

    private void OnEnable()
    {
        wand.OnDrawingComplete += playerDrawing;
    }

    private void OnDisable()
    {
        wand.OnDrawingComplete -= playerDrawing;
    }

    private void playerDrawing(Vector2[] points)
    {
        newData = true;
        playerPoints = points;
    }

    private ShapeInfoSO CompareShapeDrawing()
    {
        if (playerPoints == null || playerPoints.Length == 0)
            return null;

        playerPoints = PointsManipulation.ResampleAndNormalize(playerPoints, GameConstants.PointCount);

        if (playerPoints == null)
        {
            Debug.LogError($"[{nameof(SpellBook)}]: Failed to resample and normalize player points.");
            return null;
        }
        
        if (shapesCollection == null)
        {
            Debug.LogError($"[{nameof(SpellBook)}]: No reference to {nameof(ShapesStorageSO)} found on {gameObject.name}.");
            return null;
        }   

        ShapeInfoSO shapeInfo = CompareShapes.FindBestMatch(playerPoints, shapesCollection);

        return shapeInfo;
    }

    private ScriptableObjectSpells findProjectile(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo == null)
            return null;

        ScriptableObjectSpells spell;

        LookUpTable.dict.TryGetValue(shapeInfo, out spell);
        return spell;
    }
}
