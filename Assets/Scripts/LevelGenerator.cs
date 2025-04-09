using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{

    public GameObject cubePrefab;  // Drag your CubePrefab into this field in the Inspector
    public float spacing = 1.5f;

   public  void StartPaint()
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int i = 0; i < alphabet.Length; i++)
        {
            char letter = alphabet[i];
            float xOffset = i * spacing;
            float yOffset = Mathf.Sin(i * 0.5f) * 5f;  // Adjust the sine function as needed for the desired arrangement
            float zOffset = Mathf.Cos(i * 0.5f) * 5f;

            Vector3 cubePosition = new Vector3(xOffset, yOffset, zOffset);
            Quaternion cubeRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            GameObject cube = Instantiate(cubePrefab, cubePosition, cubeRotation);
            cube.GetComponentInChildren<TextMesh>().text = letter.ToString();
        }
    }
    [System.Serializable]
    public struct ShapeDefinition
    {
        public char shapeChar;
        public Vector3[] baseShape;
    }

    public GameObject[] propPrefabs;
    public string levelShapeString;
    public float size = 1f;
    public List<ShapeDefinition> shapeDefinitions = new List<ShapeDefinition>();

    private Dictionary<char, Vector3[]> shapeDictionary;

    void OnValidate()
    {
        UpdateShapeDictionary();
    }

    public void UpdateShapeDictionary()
    {
        // Initialize the shape dictionary with different shapes
        shapeDictionary = new Dictionary<char, Vector3[]>();

        foreach (ShapeDefinition shapeDef in shapeDefinitions)
        {
            shapeDictionary.Add(shapeDef.shapeChar, shapeDef.baseShape);
        }
    }

    public void GenerateLevel()
    {
        // Clear existing props
        Transform[] existingProps = GetComponentsInChildren<Transform>();
        foreach (Transform prop in existingProps)
        {
            if (prop != transform) // Skip the root object
            {
                DestroyImmediate(prop.gameObject);
            }
        }

        // Generate props based on the provided shape string
        float zOffset = 0f; // Offset to position props vertically
        for (int i = 0; i < levelShapeString.Length; i++)
        {
            char shapeChar = char.ToUpper(levelShapeString[i]);

            if (shapeDictionary.ContainsKey(shapeChar))
            {
                Vector3[] propPositions = shapeDictionary[shapeChar];

                foreach (Vector3 position in propPositions)
                {
                    int numberOfProps = Mathf.RoundToInt(size); // Adjust density based on size
                    for (int j = 0; j < numberOfProps; j++)
                    {
                        int randomIndex = Random.Range(0, propPrefabs.Length);
                        GameObject selectedProp = propPrefabs[randomIndex];

                        Vector3 propPosition = new Vector3(position.x * size, 0, position.z * size + zOffset);
                        Instantiate(selectedProp, propPosition, Quaternion.identity, transform).transform.localScale = Vector3.one * size;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Shape '{shapeChar}' not recognized.");
            }

            zOffset += 2f * size; // Increase the offset for the next set of props
        }

        Debug.Log("Level generated!");
    }
}
