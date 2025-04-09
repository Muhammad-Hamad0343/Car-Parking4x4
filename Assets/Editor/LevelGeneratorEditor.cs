using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelGenerator levelDesigner = (LevelGenerator)target;

        if (GUILayout.Button("Update Shape Dictionary"))
        {
            levelDesigner.UpdateShapeDictionary();
        }

        if (GUILayout.Button("Generate Level"))
        {
            levelDesigner.GenerateLevel();
        }
        if (GUILayout.Button("Draw"))
        {
            levelDesigner.StartPaint();
        }
    }
}
