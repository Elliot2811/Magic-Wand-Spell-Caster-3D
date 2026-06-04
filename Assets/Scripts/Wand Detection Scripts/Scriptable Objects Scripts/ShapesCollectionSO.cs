using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapesStorage", menuName = "Shapes/Shapes Storage")]
public class ShapesCollectionSO : ScriptableObject, IEnumerable<ShapeInfoSO>
{
    [SerializeField] private ShapeInfoSO[] shapes;

    private HashSet<ShapeInfoSO> shapesSet;

    public IEnumerable<ShapeInfoSO> Shapes => shapesSet;

    public int Count => shapesSet.Count;

    public ShapeInfoSO GetShapeInfoSO(int index)
    {
        shapesSet.TryGetValue(shapes[index], out ShapeInfoSO shape);

        return shape;
    }
    public bool TryGetShapeInfoSO(int index, out ShapeInfoSO shape) => shapesSet.TryGetValue(shapes[index], out shape);

    public ShapeInfoSO GetShapeInfoSO(string shapeName)
    {
        foreach (ShapeInfoSO shape in shapesSet)
        {
            if (shape.ShapeName == shapeName)
                return shape;
        }
        return null;
    }

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

            if (shape.ValidShape)
                if (!shapesSet.Add(shape))
                    Debug.LogWarning($"[{name}]: Duplicate {shape.ShapeName} shape in ShapesStorage");
        }

        if (shapesSet.Count == 0)
            Debug.Log($"[{name}]: No valid shapes in ShapesStorage");
    }

    public IEnumerator<ShapeInfoSO> GetEnumerator() => shapesSet.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}