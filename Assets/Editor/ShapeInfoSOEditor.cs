using UnityEditor;

[CustomEditor(typeof(ShapeInfoSO))]
public class ShapeInfoSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty shapeNameProp = serializedObject.FindProperty("shapeName");
        SerializedProperty variantsProp = serializedObject.FindProperty("variants");

        EditorGUILayout.PropertyField(shapeNameProp);

        int newValue = EditorGUILayout.DelayedIntField("Number Of Variants", variantsProp.arraySize);
        if (newValue < 0) newValue = 0;

        if (newValue != variantsProp.arraySize)
            variantsProp.arraySize = newValue;

        EditorGUILayout.PropertyField(variantsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}