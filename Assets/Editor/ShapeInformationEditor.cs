using UnityEditor;

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
        EditorGUILayout.PropertyField(numVariantsProp);

        if (numVariantsProp.intValue != 0 && variantsProp.arraySize != numVariantsProp.intValue)
        {
            variantsProp.arraySize = (numVariantsProp.intValue > 0) ? numVariantsProp.intValue : 1;
        }

        EditorGUILayout.PropertyField(variantsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}