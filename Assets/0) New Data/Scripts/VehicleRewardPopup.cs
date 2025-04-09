using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleRewardPopup : MonoBehaviour
{
    [SerializeField] private GameObject[] rewardCars;

    private void OnEnable()
    {
        for (int i = 0; i < rewardCars.Length; i++) 
        {
            rewardCars[i].SetActive(i == CheckWhichCarToUnlock()); 
        }
    }

    private int CheckWhichCarToUnlock()
    {
        int carToUnlock = 0;
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 2.ToString()) == 0)
        {
            carToUnlock = 1;
        }
        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 3.ToString()) == 0)
        {
            carToUnlock = 2;
        }
        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 4.ToString()) == 0)
        {
            carToUnlock = 3;
        }
        return carToUnlock;
    }
}
