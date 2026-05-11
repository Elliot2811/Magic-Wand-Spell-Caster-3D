using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShape", menuName = "Shapes/Shape Info")]
public class ShapeInfoSO : ScriptableObject, IEnumerable<ShapeVariantSO>
{
    [SerializeField] private string shapeName;
    [SerializeField] private ShapeVariantSO[] variants;

    private HashSet<ShapeVariantSO> variantsSet;

    public string ShapeName => shapeName;

    public IEnumerable<ShapeVariantSO> Variants => variantsSet;

    private void OnEnable()
    {
        BuildVariantSet();
    }

    public bool ValidShape
    {
        get
        {
            if (variantsSet == null)
                BuildVariantSet();

            return variantsSet.Count > 0;
        }
    }

    public Vector2[] RandomVariantData
    {
        get
        {
            if (variantsSet == null)
                BuildVariantSet();

            if (variantsSet.Count > 0)
                return variantsSet.First().OriginalPoints;
            return null;
        }
    }

    public void BuildVariantSet()
    {
        if (variantsSet != null)
            return;

        variantsSet = new HashSet<ShapeVariantSO>();
        foreach (ShapeVariantSO variant in variants)
        {
            if (variant == null)
            {
                Debug.LogWarning($"[{name}]: Variant instance in {shapeName} not found");
                continue;
            }

            if (variant.ValidDrawing)
                if (!variantsSet.Add(variant))
                    Debug.LogWarning($"[{name}]: Duplicate variant in {shapeName}");
        }

        if (variantsSet.Count == 0)
            Debug.LogWarning($"[{name}]: No valid variants in {shapeName}.");
    }

    public IEnumerator<ShapeVariantSO> GetEnumerator() => variantsSet.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => variantsSet.GetEnumerator();
}