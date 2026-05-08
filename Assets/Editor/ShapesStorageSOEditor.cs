using UnityEditor;

[CustomEditor(typeof(ShapesStorageSO))]
public class ShapesStorageSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty shapesProp = serializedObject.FindProperty("shapes");

        int newValue = EditorGUILayout.DelayedIntField("Number Of Shapes", shapesProp.arraySize);

        if (newValue < 0) newValue = 0;

        if (newValue != shapesProp.arraySize)
            shapesProp.arraySize = newValue;

        EditorGUILayout.PropertyField(shapesProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
