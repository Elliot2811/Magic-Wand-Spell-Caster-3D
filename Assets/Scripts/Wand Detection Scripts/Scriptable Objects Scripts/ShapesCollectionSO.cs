using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapesStorage", menuName = "Shapes/Shapes Storage")]
public class ShapesCollectionSO : ScriptableObject, IEnumerable<ShapeInfoSO>
{
    public ShapeInfoSO[] shapes;

    private HashSet<ShapeInfoSO> shapesSet;

    private ShapeInfoSO[] storedShapes;

    public IEnumerable<ShapeInfoSO> Shapes => shapesSet;

    public int Count => shapesSet.Count;

    public ShapeInfoSO GetShapeInfoSO(int index)
    {
        return storedShapes[index];
    }

    private void OnEnable()
    {
        if (shapes != null)
            BuildShapesSet();
    }

    public bool TryGetShapeInfoSO(int index, out ShapeInfoSO shape)
    {
        if (index < 0 || index >= storedShapes.Length)
        {
            shape = null;
            return false;
        }

        shape = storedShapes[index];
        return true;
    }

    public ShapeInfoSO[] GetAllShapes()
    {
        return storedShapes;
    }

    public ShapeInfoSO GetShapeInfoSO(string shapeName)
    {
        foreach (ShapeInfoSO shape in shapesSet)
        {
            if (shape.ShapeName == shapeName)
                return shape;
        }
        return null;
    }

    public void BuildShapesSet()
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
        {
            Debug.Log($"[{name}]: No valid shapes in ShapesStorage");
            return;
        }

        BuildCompletedArray();
    }

    private void BuildCompletedArray()
    {
        storedShapes = new ShapeInfoSO[shapesSet.Count];

        int i = 0;
        foreach (ShapeInfoSO shape in shapesSet)
        {
            storedShapes[i] = shape;
            i++;
        }
    }

    public IEnumerator<ShapeInfoSO> GetEnumerator()
    {
        foreach (ShapeInfoSO shape in storedShapes)
            yield return shape;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}