using UnityEditor;
using UnityEngine;

namespace AHP.Editor
{
    [CustomEditor(typeof(GridGenerator))]
    public class GridGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var generator = (GridGenerator)target;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Generation Controls", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Generate Layout", GUILayout.Height(30)))
                {
                    generator.GenerateLayout();
                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("Reseed + Generate", GUILayout.Height(30)))
                {
                    if (generator.Config != null)
                    {
                        generator.Config.Seed = Random.Range(int.MinValue, int.MaxValue);
                        EditorUtility.SetDirty(generator.Config);
                    }
                    generator.GenerateLayout();
                    SceneView.RepaintAll();
                }
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Placement Controls", EditorStyles.boldLabel);

            GUI.enabled = generator.LastLayout != null;
            if (GUILayout.Button("Build Placement", GUILayout.Height(30)))
            {
                generator.BuildPlacement();
                SceneView.RepaintAll();
            }
            GUI.enabled = true;

            // Display last log
            if (!string.IsNullOrEmpty(generator.LastLog))
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Last Generation Log", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(generator.LastLog, MessageType.Info);
            }
        }
    }
}