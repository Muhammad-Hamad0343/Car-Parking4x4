//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fuel station. When a vehicle enters the trigger, fuel tank will be filled up.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Fuel Station")]
public class RCC_FuelStation : MonoBehaviour 
{
    public static RCC_FuelStation instance;

    private RCC_CarControllerV3 targetVehicle;      //  Target vehicle.
    public float refillSpeed = 1f;      //  Refill speed.
    private void Start()
    {
        instance = this;
    }
    private void OnTriggerStay(Collider col) {

        targetVehicle = col.gameObject.GetComponentInParent<RCC_CarControllerV3>();

        //  If target vehicle is null, return.
        if (!targetVehicle)
            return;

        //  Refill the tank with given speed * time.
        if (targetVehicle)
            targetVehicle.fuelTank += refillSpeed * Time.deltaTime;

        
        RCC_CarControllerV3.instance.engineRunning = true;
        RCC_CarControllerV3.instance.fuelInput = 1f;
      
        this.gameObject.SetActive(false);
        Invoke("ActiveFueltrigger", 10f);
    }
    public void RefillFuel()        // By rewarded ad
    {
        if (targetVehicle)
            targetVehicle.fuelTank += refillSpeed * Time.deltaTime;

       
        RCC_CarControllerV3.instance.engineRunning = true;
        RCC_CarControllerV3.instance.fuelInput = 1f;


      
        //#if !UNITY_EDITOR
        //            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        //            AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        //            AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, "Coming Soon", toastClass.GetStatic<int>("LENGTH_SHORT"));
        //            toast.Call("show");
        //#endif


        Firebase.Analytics.FirebaseAnalytics.LogEvent("D_Fuel_Refill_ByRewardedAds");

    }


    public void ActiveFueltrigger()
    {
        this.gameObject.SetActive(true);
    }
    public void RefillByCoins()
    {
        //if(GamePlayManager.instance.highScores >= 100)
        //{
        //    GamePlayManager.instance.fuel.value = 100;
        //    GamePlayManager.instance.LowFuelPanel.SetActive(false);
        //    RCC_CarControllerV3.instance.engineRunning = true;
        //    RCC_CarControllerV3.instance.fuelInput = 1f;


        //    GamePlayManager.instance.Player.GetComponent<RCC_CarControllerV3>().throttleInput = 1;
        //    GamePlayManager.instance.Player.GetComponent<RCC_CarControllerV3>().handbrakeInput = 0;
        //    GamePlayManager.instance.Player.GetComponent<Rigidbody>().velocity = Vector3.Lerp(GamePlayManager.instance.Player.GetComponent<Rigidbody>().velocity, Vector3.zero, 1f);

        //    GamePlayManager.instance.SetRCCVolume(1f);
        //    GamePlayManager.instance.fuelConsuption();

        //    GamePlayManager.instance.highScores = GamePlayManager.instance.highScores - 100;

        //    PlayerPrefs.SetInt("highscores", GamePlayManager.instance.highScores);
        //    GamePlayManager.instance.highScores  = PlayerPrefs.GetInt("highscores", 0);
        //}
        //if (GamePlayManager.instance.highScores < 100)
        //{
        //    GamePlayManager.instance.FuelRefillByCoins.GetComponent<Button>().interactable = false;
        //}



        Firebase.Analytics.FirebaseAnalytics.LogEvent("D_Fuel_Refill_ByCoins");

    }
    private void OnTriggerExit(Collider col) {

        //  Setting target vehicle to null when vehicle exits the trigger.
        if (col.gameObject.GetComponentInParent<RCC_CarControllerV3>())
            targetVehicle = null;

    }

}
