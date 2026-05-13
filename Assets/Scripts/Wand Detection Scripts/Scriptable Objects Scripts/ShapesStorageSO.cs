using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapesStorage", menuName = "Shapes/Shapes Storage")]
public class ShapesStorageSO : ScriptableObject, IEnumerable<ShapeInfoSO>
{
    [SerializeField] private ShapeInfoSO[] shapes;

    private HashSet<ShapeInfoSO> shapesSet;

    public IEnumerable<ShapeInfoSO> Shapes => shapesSet;

    private void OnEnable()
    {
        BuildShapesSet();
    }

    private void BuildShapesSet()
    {
        shapesSet = new HashSet<ShapeInfoSO>();
        foreach (ShapeInfoSO shape in shapes)
        {
            if (shape == null)
            {
                Debug.LogWarning($"[{name}]: Shape in ShapesStorage not found");
                continue;
            }

            if (shape.CheckValidShape())
                if (!shapesSet.Add(shape))
                    Debug.LogWarning($"[{name}]: Duplicate {shape.ShapeName} shape in ShapesStorage");
        }

        if (shapesSet.Count == 0)
            Debug.Log($"[{name}]: No valid shapes in ShapesStorage");
    }

    public IEnumerator<ShapeInfoSO> GetEnumerator() => shapesSet.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}