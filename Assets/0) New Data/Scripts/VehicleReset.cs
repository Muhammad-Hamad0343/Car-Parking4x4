using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleReset : MonoBehaviour
{
    float timerBeginToReset = 0;
    void Update()
    {
        if(transform.eulerAngles.z > 90 || transform.eulerAngles.z < -90)
        {
            timerBeginToReset += Time.deltaTime;

            if(timerBeginToReset > 5)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
                Debug.Log("Vehicle Reset occured");
                timerBeginToReset = 0;
            }
        }
    }
}
