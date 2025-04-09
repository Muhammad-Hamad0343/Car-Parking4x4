using UnityEditor;
using UnityEngine;
using System.Collections;

public enum Direction
{
    x_Axis,
    z_Axis
}
public class ObjectGeneratorEditor : EditorWindow
{
    public GameObject LevelParent;
    private GameObject objectToGenerate;
    private Vector3 generationDirection = Vector3.forward;
    private Vector3 generationAngle = Vector3.zero;
    public float increament = 2;
    private Direction axis;

    private Vector3 lastSpawnPosition=Vector3.zero;

    [MenuItem("Parking Map/Open Object Generator Window")]
    private static void ShowWindow()
    {
        GetWindow<ObjectGeneratorEditor>("Object Generator");
    }
    private void OnGUI()
    {
        GUILayout.Label("Object Generator", EditorStyles.boldLabel);

        objectToGenerate = EditorGUILayout.ObjectField("Object to Generate", objectToGenerate, typeof(GameObject), true) as GameObject;
        LevelParent = EditorGUILayout.ObjectField("Current Level Parent", LevelParent, typeof(GameObject), true) as GameObject;
        generationDirection = EditorGUILayout.Vector3Field("Generation Direction", generationDirection);
        generationAngle = EditorGUILayout.Vector3Field("Generation Rotation", generationAngle);
        lastSpawnPosition = EditorGUILayout.Vector3Field("Last Spwan Position", lastSpawnPosition);
        axis = (Direction)EditorGUILayout.EnumPopup("Axis", axis);
        increament = EditorGUILayout.FloatField("Increament", increament);

        if (GUILayout.Button("Generate Object"))
        {
            GenerateObject();
        }

        if (GUILayout.Button("Reset"))
        {
            ResetValues();
        }
    }

    private void ResetValues()
    {
        objectToGenerate = null;
        LevelParent = null;
        generationDirection = Vector3.forward;
        generationAngle = Vector3.zero;
        increament = 2;
        axis = Direction.x_Axis;
        lastSpawnPosition = Vector3.zero;
    }

    private void GenerateObject()
    {
        if (objectToGenerate != null)
        {
            // Instantiate the object in the specified direction
            GameObject newObject = Instantiate(objectToGenerate, GetGenerationPosition(), Quaternion.identity);
            Selection.activeGameObject = newObject;
            newObject.transform.eulerAngles = generationAngle;
            newObject.transform.parent = LevelParent.transform;

            // Save the last spawn position
            lastSpawnPosition = newObject.transform.position;
        }
        else
        {
            Debug.LogWarning("Object to generate is not assigned.");
        }
    }
    Vector3 GetGenerationPosition()
    {
        Vector3 newSpawnPosition = Vector3.zero;
        if (lastSpawnPosition != Vector3.zero)
            newSpawnPosition = lastSpawnPosition;
        else
            newSpawnPosition = generationDirection;

        switch (axis)
        {
            case Direction.x_Axis:
                newSpawnPosition.x += increament;
                break;
            case Direction.z_Axis:
                newSpawnPosition.z += increament;
                break;
                // Add more cases if needed
        }

        return newSpawnPosition;

    }
}
