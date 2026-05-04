using UnityEditor;

[CustomEditor(typeof(ShapesStorage))]
public class ShapesStorageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty numShapesProp = serializedObject.FindProperty("numberOfShapes");
        SerializedProperty shapesProp = serializedObject.FindProperty("shapes");

        int newValue = EditorGUILayout.DelayedIntField("Number Of Shapes", shapesProp.arraySize);

        if (newValue < 0) newValue = 0;

        if (newValue != shapesProp.arraySize)
        {
            shapesProp.arraySize = newValue;
        }

        // Sync back to numberOfShapes (read-only reflection of array size)
        numShapesProp.intValue = shapesProp.arraySize;

        EditorGUILayout.PropertyField(shapesProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
