using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellProjectileLookUpTable", menuName = "Shapes/Spell Projectile Look Up Table")]
public class SpellProjectileLookUpTable : ScriptableObject, IEnumerable<SpellProjectileLookUpTable.SpellProjectilePair>
{
    [System.Serializable]
    public struct SpellProjectilePair
    {
        public ShapeInfoSO shape;
        public ScriptableObjectSpell spell;
    }

    public SpellProjectileLookUpTable()
    {
    }

    [SerializeField] private SpellProjectilePair[] pairs;

    private Dictionary<ShapeInfoSO, ScriptableObjectSpell> lookupDictionary;

    public void AddPair(ShapeInfoSO shape, ScriptableObjectSpell spell)
    {
        if (shape == null || spell == null)
        {
            Debug.LogWarning("Cannot add null shape or spell to SpellProjectileLookUpTable.");
            return;
        }

        if (lookupDictionary == null)
            BuildDictionary();

        if (lookupDictionary.ContainsKey(shape))
        {
            Debug.LogWarning($"Shape {shape.ShapeName} already exists in SpellProjectileLookUpTable. Skipping.");
            return;
        }

        lookupDictionary.Add(shape, spell);
    }

    //public ShapeInfoSO[] GetShapes()
    //{
    //    if (lookupDictionary == null)
    //        BuildDictionary();
    //    ShapeInfoSO[] shapes = new ShapeInfoSO[lookupDictionary.Count];
    //    lookupDictionary.Keys.CopyTo(shapes, 0);
    //    return shapes;
    //}

    public bool TryGetSpell(ShapeInfoSO shape, out ScriptableObjectSpell spell)
    {
        return lookupDictionary.TryGetValue(shape, out spell);
    }

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        if (lookupDictionary != null)
            return;

        lookupDictionary = new Dictionary<ShapeInfoSO, ScriptableObjectSpell>();
        foreach (SpellProjectilePair pair in pairs)
        {
            if (pair.shape == null || pair.spell == null)
            {
                Debug.LogWarning("Null shape or spell in SpellProjectilePair. Skipping.");
                continue;
            }

            if (lookupDictionary.ContainsKey(pair.shape))
            {
                Debug.LogWarning($"Duplicate shape {pair.shape.ShapeName} in SpellProjectileLookUpTable. Skipping.");
                continue;
            }

            lookupDictionary.Add(pair.shape, pair.spell);
        }
    }

    public IEnumerator<SpellProjectilePair> GetEnumerator()
    {
        foreach (SpellProjectilePair pair in pairs)
        {
            yield return new SpellProjectilePair { shape = pair.shape, spell = pair.spell };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}