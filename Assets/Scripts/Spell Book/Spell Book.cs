using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : MonoBehaviour
{
    [SerializeField]
    private bool instantiated = true;
    private bool initialized = false;

    private GamePlayState gamePlayState;

    [SerializeField]
    private SpellProjectileLookUpTable spellLookupTable;

    private ShapesCollectionSO drawableSpells;
    private ShapeInfoSO[] allShapesInfo;
    private List<ShapeInfoSO> bag;

    private ShapesCollectionSO castableShapes;

    [SerializeField]
    private WandListener wandListener;

    [SerializeField]
    private CharacterEntity playerEntity;

    [SerializeField]
    private int playerNumber = 0; // 0 = left, 1 = right must match CircleBonusEvent's player1Index/player2Index mapping

    private bool newData = true;
    private ShapeInfoSO shape;

    private HashSet<ShapeInfoSO> shapesAlreadyReplacing;

    private void Awake()
    {
        if (!instantiated)
            Init();
    }


    private void Update()
    {
        if (!initialized || !newData)
            return;

        newData = false;

        if (!GameStateManager.Instance.renderSpellBookSpell)
            return;

        ScriptableObjectSpell spell = FindProjectile(shape);
        if (spell != null)
        {
            float damageMultiplier = 1f;
            if (CircleBonusEvent.Instance != null && CircleBonusEvent.Instance.ConsumeBonus(playerNumber))
            {
                damageMultiplier = CircleBonusEvent.Instance.bonusDamageMultiplier;
                Debug.Log($"[SpellBook] player {playerNumber}: circle bonus applied (x{damageMultiplier}).");
            }
            // Function to cast spell
            playerEntity.CastSpell(spell, damageMultiplier, playerNumber);
        }
    }

    private void OnDisable()
    {
        if (wandListener != null)
            wandListener.MatchedShape -= playerDrawing;
    }

    private void Init()
    {
        Init(wandListener, playerEntity, spellLookupTable, drawableSpells, playerNumber);
    }

    public void Init(WandListener wandListener, CharacterEntity playerEntity, SpellProjectileLookUpTable lookUpTable, ShapesCollectionSO allShapes, int playerNumber)
    {
        if (wandListener == null)
        {
            Debug.LogWarning("[SpellBook]: No reference to wandListener.");
            gameObject.SetActive(false);
            return;
        }

        if (playerEntity == null)
        {
            Debug.LogWarning("[SpellBook]: No reference to player entity");
            gameObject.SetActive(false);
            return;
        }

        if (lookUpTable == null)
        {
            Debug.LogWarning("[SpellBook]: No spell look up table to compare shapes and find projectile");
            gameObject.SetActive(false);
            return;
        }

        if (allShapes == null)
        {
            Debug.LogWarning("[SpellBook]: No spell collection to compare shapes and find best shape");
            gameObject.SetActive(false);
            return;
        }

        this.playerNumber = playerNumber;
        this.wandListener = wandListener;
        this.playerEntity = playerEntity;
        this.spellLookupTable = lookUpTable;
        this.drawableSpells = allShapes;

        wandListener.MatchedShape += playerDrawing;

        allShapesInfo = drawableSpells.GetAllShapes();

        bag = new List<ShapeInfoSO>();
        RefillBag();
        CreateCastableShapes(2);

        gamePlayState = (GamePlayState)GameStateManager.Instance.CurrentState;

        if (playerNumber == 0)
        {
            gamePlayState.leftSpellCollection = castableShapes;
        }
        else
        {
            gamePlayState.rightSpellCollection = castableShapes;
        }
        gamePlayState.redrawFlag = true;

        wandListener.shapes = castableShapes;

        shapesAlreadyReplacing = new HashSet<ShapeInfoSO>();

        initialized = true;
    }

    private void playerDrawing(ShapeInfoSO shape)
    {
        newData = true;
        this.shape = shape;
    }

    private ScriptableObjectSpell FindProjectile(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo == null)
            return null;

        ReplaceSpellAfterTime(shapeInfo);

        ScriptableObjectSpell spell;

        spellLookupTable.TryGetSpell(shapeInfo, out spell);
        return spell;
    }

    private void ReplaceSpellAfterTime(ShapeInfoSO shapeInfo)
    {
        if (shapesAlreadyReplacing.Contains(shapeInfo))
            return;

        shapesAlreadyReplacing.Add(shapeInfo);
        StartCoroutine(waitTimeReplaceSpell(shapeInfo, 3f));
    }

    private IEnumerator waitTimeReplaceSpell(ShapeInfoSO shapeInfo, float time)
    {
        yield return new WaitForSeconds(time);

        ReplaceSpell(shapeInfo);
        if (shapesAlreadyReplacing.Contains(shapeInfo))
            shapesAlreadyReplacing.Remove(shapeInfo);
    }

    private void ReplaceSpell(ShapeInfoSO shapeInfo)
    {
        //Debug.Log("ReplaceSpell called");

        int index = System.Array.IndexOf(castableShapes.shapes, shapeInfo);

        if (index < 0)
        {
            Debug.LogWarning($"Shape {shapeInfo.ShapeName} not found in castable shapes.");
            return;
        }

        ShapeInfoSO candidate = null;
        const int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            candidate = GetNext();

            bool dupe = false;
            foreach (ShapeInfoSO held in castableShapes.shapes)
            {
                if (held == candidate)
                {
                    dupe = true;
                    break;
                }
            }

            if (!dupe)
                break;
        }

        gamePlayState.redrawFlag = true;

        castableShapes.shapes[index] = candidate;
        castableShapes.BuildShapesSet();
    }

    private ShapeInfoSO GetNext()
    {
        if (bag.Count == 0)
            RefillBag();

        ShapeInfoSO next = bag[bag.Count - 1];
        bag.RemoveAt(bag.Count - 1);
        return next;
    }

    private void RefillBag()
    {
        //Debug.Log("Refilling draw bag");

        bag.Clear();
        bag.AddRange(allShapesInfo);

        // Full Fisher-Yates shuffle to randomize bag
        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }
    }

    private void CreateCastableShapes(int size)
    {
        Debug.Log("Creating main draw bag");

        ShapeInfoSO[] arr = new ShapeInfoSO[size];

        for (int i = 0; i < size; i++)
        {
            arr[i] = GetNext();
        }

        castableShapes = ShapesCollectionSO.CreateInstance<ShapesCollectionSO>();
        castableShapes.shapes = arr;
        castableShapes.BuildShapesSet();
    }
}
