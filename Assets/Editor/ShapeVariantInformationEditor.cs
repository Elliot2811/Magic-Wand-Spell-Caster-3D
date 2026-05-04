using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeVariantInformation))]
public class ShapeVariantInformationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty numPointsProp = serializedObject.FindProperty("numberOfPoints");
        SerializedProperty pointsProp = serializedObject.FindProperty("points");

        int newValue = EditorGUILayout.DelayedIntField("Number Of Points", pointsProp.arraySize);
        newValue = Mathf.Max(0, newValue);

        if (newValue != pointsProp.arraySize)
        {
            pointsProp.arraySize = newValue;
        }

        // Sync count to array
        numPointsProp.intValue = pointsProp.arraySize;

        EditorGUILayout.PropertyField(pointsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}