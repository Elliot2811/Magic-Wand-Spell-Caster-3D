using UnityEngine;

public class SpellBook : MonoBehaviour
{
    [SerializeField]
    private bool instantiated = true;
    private bool initialized = false;

    [SerializeField]
    private SpellProjectileLookUpTable spellLookupTable;

    [SerializeField]
    private WandListener wandListener;

    [SerializeField]
    private CharacterEntity playerEntity;

    private bool newData = true;
    private ShapeInfoSO shape;

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

        ScriptableObjectSpells spell = FindProjectile(shape);

        if (spell != null)
        {
            playerEntity.FireSpell(spell, 0f);
        }
    }

    private void OnDisable()
    {
        if (wandListener != null)
            wandListener.MatchedShape -= playerDrawing;
    }

    private void Init()
    {
        Init(wandListener, playerEntity, spellLookupTable);
    }

    public void Init(WandListener wandListener, CharacterEntity playerEntity, SpellProjectileLookUpTable lookUpTable)
    {
        this.wandListener = wandListener;
        this.playerEntity = playerEntity;
        this.spellLookupTable = lookUpTable;

        if (wandListener == null)
        {
            Debug.LogWarning("[SpellBook]: No reference to wandListener.");
            return;
        }

        wandListener.MatchedShape += playerDrawing;

        if (playerEntity == null)
        {
            Debug.LogWarning("[SpellBook]: No reference to player entity");
        }

        if (spellLookupTable == null)
        {
            Debug.LogWarning("[SpellBook]: No spell look up table to compare shapes and find projectile");
            return;
        }

        initialized = true;
    }

    private void playerDrawing(ShapeInfoSO shape)
    {
        newData = true;
        this.shape = shape;
    }

    private ScriptableObjectSpells FindProjectile(ShapeInfoSO shapeInfo)
    {
        if (shapeInfo == null)
            return null;

        ScriptableObjectSpells spell;

        spellLookupTable.TryGetSpell(shapeInfo, out spell);
        return spell;
    }
}
