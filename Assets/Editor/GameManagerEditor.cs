using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty numShapesProp = serializedObject.FindProperty("numberOfShapes");
        SerializedProperty shapesProp = serializedObject.FindProperty("shapes");

        EditorGUILayout.PropertyField(numShapesProp);

        if (numShapesProp.intValue != 0 && shapesProp.arraySize != numShapesProp.intValue)
        {
            shapesProp.arraySize = (numShapesProp.intValue > 0) ? numShapesProp.intValue : 1;
        }

        EditorGUILayout.PropertyField(shapesProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
