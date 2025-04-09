using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private float speed = 5.0f;
    private float rotationSpeed = 2.0f;
    [SerializeField] private AudioSource checkPointAudio;

    private Waypoint[] waypoints; // An array to store your waypoints.
    private int currentWaypointIndex = 0;
    

  private void Start()
    {
        if (GameManager.instance.activeMode == GameMode.parking)
            return;
        waypoints = (Waypoint[])GameManager.instance.GetWaypointArray();
        foreach (Waypoint w in waypoints)
            w.gameObject.SetActive(false);
        waypoints[0].gameObject.SetActive(true);
    }

    public void OnWaypointReached(Waypoint waypoint)
    {
        // Check if the reached waypoint is the next one in the list.
        if (waypoint == waypoints[currentWaypointIndex])
        {
            if (currentWaypointIndex < waypoints.Length - 1)
            {
                currentWaypointIndex++;
                waypoints[currentWaypointIndex].gameObject.SetActive(true);
            }
        }
    }
    public void PlayCheckPointAudio()
    {
        checkPointAudio.Play();
    }

    private void Update()
    {
        //if (currentWaypointIndex < waypoints.Length)
        //{
        //    Waypoint currentWaypoint = waypoints[currentWaypointIndex];

        //    // Calculate the direction from the player to the current waypoint.
        //    Vector3 direction = currentWaypoint.transform.position - transform.position;

        //    // Rotate the player towards the waypoint direction.
        //    transform.rotation = Quaternion.Slerp(
        //        transform.rotation,
        //        Quaternion.LookRotation(direction),
        //        rotationSpeed * Time.deltaTime
        //    );

        //    // Move the player towards the waypoint.
        //    transform.Translate(Vector3.forward * speed * Time.deltaTime);
        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FinishPoint"))
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            GameManager.TriggerGameWin(GameManager.instance);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishPoint") && SceneManager.GetActiveScene().buildIndex == 2)
        {
            for (int i = 0; i < ConffiteParticles.Instance.Conffiteparticle.Length; i++)
            {
                ConffiteParticles.Instance.Conffiteparticle[i].SetActive(true);
            }
            GameManager.TriggerGameWin(GameManager.instance);
            //Debug.LogError("This");

            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
