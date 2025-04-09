using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffest : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // Adjust the scrolling speed as needed

    private Renderer rend;
    private Material material;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        material = rend.material;
    }

    private void Update()
    {
        // Calculate the new texture offset based on time
        float offset = Time.time * scrollSpeed;

        // Apply the offset to the "_BaseMap" texture property
        material.SetTextureOffset("_BaseMap", new Vector2(offset, 0));
    }
}
