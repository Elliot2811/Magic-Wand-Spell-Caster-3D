using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeInformation))]
public class ShapeInformationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty shapeNameProp = serializedObject.FindProperty("shapeName");
        SerializedProperty numVariantsProp = serializedObject.FindProperty("numberOfVariants");
        SerializedProperty variantsProp = serializedObject.FindProperty("variants");

        EditorGUILayout.PropertyField(shapeNameProp);

        int newValue = EditorGUILayout.DelayedIntField("Number Of Variants", variantsProp.arraySize);
        newValue = Mathf.Max(0, newValue);

        if (newValue != variantsProp.arraySize)
        {
            variantsProp.arraySize = newValue;
        }

        // Sync count to array
        numVariantsProp.intValue = variantsProp.arraySize;

        EditorGUILayout.PropertyField(variantsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}