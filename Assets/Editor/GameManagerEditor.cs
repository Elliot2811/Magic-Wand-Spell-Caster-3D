using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (prop.name == "m_Script") continue;

            if ((
                prop.name == "lineRenderer" ||
                prop.name == "lineWidth" ||
                prop.name == "displayPercentage"
                ) &&
                !serializedObject.FindProperty("displayClosestShape").boolValue
                )
                continue;

            EditorGUILayout.PropertyField(prop);
        }

        serializedObject.ApplyModifiedProperties();
    }
}