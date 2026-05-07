using UnityEditor;

[CustomEditor(typeof(ShapeVariantSO))]
public class ShapeVariantSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty pointsProp = serializedObject.FindProperty("points");

        int newValue = EditorGUILayout.DelayedIntField("Number Of Points", pointsProp.arraySize);

        if (newValue < 0) newValue = 0;

        if (newValue != pointsProp.arraySize)
            pointsProp.arraySize = newValue;

        EditorGUILayout.PropertyField(pointsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}