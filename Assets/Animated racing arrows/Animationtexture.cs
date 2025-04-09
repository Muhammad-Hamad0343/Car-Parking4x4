using UnityEngine;
using System.Collections;

public class Animationtexture : MonoBehaviour 
{

    public Vector2 AnimationSpeed = new Vector2(-0.5f, 0.0f);

    Vector2 uvOffset = Vector2.zero;
    Material material;

    void Start()
    {
        // Assuming the material is assigned to the first renderer's first material slot
        material = GetComponent<Renderer>().materials[0];
    }

    void LateUpdate()
    {
        uvOffset += (AnimationSpeed * Time.deltaTime);
        if (GetComponent<Renderer>().enabled)
        {
            material.SetTextureOffset("_BaseMap", uvOffset); // Use "_BaseMap" for URP
        }
    }
}
