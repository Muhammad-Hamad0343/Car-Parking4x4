using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSlide : MonoBehaviour
{
    [SerializeField] private GameObject slideCamera;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            slideCamera.SetActive(true);
        }
    }
}
