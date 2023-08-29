using UnityEditor;

[CustomEditor(typeof(DungeonParameters))]
public class DungeonParametersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DungeonParameters parameters = (DungeonParameters)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("algorithm"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("General Dungeon Generation Parameters", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonHeight"));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomMargin"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumRooms"));

        EditorGUILayout.Space();


        switch (parameters.algorithm)
        {
            case DungeonGenerationAlgorithm.SimpleRandomWalk:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("iterations"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("walkLength"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("walkType"));
                break;

            case DungeonGenerationAlgorithm.BinarySpacePartition:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumRoomWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumRoomHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("favoredSplitDirection"));
                break;

            // Add more here as more algorithms are implemented
        }

        serializedObject.ApplyModifiedProperties();
    }
}
