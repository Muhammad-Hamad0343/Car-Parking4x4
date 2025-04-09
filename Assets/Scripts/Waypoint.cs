using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerLayer; // Specify the player layer.
    private PlayerController controller;
    private void Start()
    {
      //  controller = GameManager.instance.activePlayerVehicle.GetComponent<PlayerController>();

    }

    // Add a trigger collider to the waypoint.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            Debug.Log("FinishPoint__3--=>");
            //  controller.PlayCheckPointAudio();
            if (this.gameObject.CompareTag("FinishPoint"))
            {
                GameManager.TriggerGameWin(GameManager.instance);
            }
            else
            {
                this.gameObject.SetActive(false);
                if (controller != null)
                {
                    controller.OnWaypointReached(this);
                }
            }
        }
    }
}
