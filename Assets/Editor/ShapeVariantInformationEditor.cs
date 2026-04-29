using UnityEditor;

[CustomEditor(typeof(ShapeVariantInformation))]
public class ShapeVariantInformationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty numPointsProp = serializedObject.FindProperty("numberOfPoints");
        SerializedProperty pointsProp = serializedObject.FindProperty("points");

        EditorGUILayout.PropertyField(numPointsProp);

        if (numPointsProp.intValue != 0 &&  pointsProp.arraySize != numPointsProp.intValue)
        {
            pointsProp.arraySize = (numPointsProp.intValue > 0) ? numPointsProp.intValue : 1;
        }

        EditorGUILayout.PropertyField(pointsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
