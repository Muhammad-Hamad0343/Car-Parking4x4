using System.Collections;
using UnityEngine;

public class PropBlink : MonoBehaviour
{
    [SerializeField] private Material blinkMaterial;   // Assign your blinking material in the Inspector
    [SerializeField] private bool self;

    private Renderer propRenderer;
    private Material defaultMaterial; // Assign your default material in the Inspector
    private bool isBlinking = false;

    void Start()
    {
        if (self)
        {
            propRenderer = GetComponent<Renderer>();
        }
        else
        {
            propRenderer = GetComponentInChildren<Renderer>();
        }
        defaultMaterial=propRenderer.material; // Set the default material initially
    }

    public void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(Blink());
        }
    }

    IEnumerator Blink()
    {
        while (isBlinking)
        {
            // Switch between default material and blinking material
            propRenderer.material = (propRenderer.material == defaultMaterial) ? blinkMaterial : defaultMaterial;

            yield return new WaitForSeconds(0.4f); // Adjust the blink speed as needed
        }

        propRenderer.material = defaultMaterial; // Reset the material when blinking stops
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 12)
        {
            StartBlinking();
            Handheld.Vibrate();
            if (GameManager.instance.hits <= 3)
            {
                GameManager.instance.ManageHit(this);
            }
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 12)
        {
            GameManager.instance.HitExit();
        }
    }
}
