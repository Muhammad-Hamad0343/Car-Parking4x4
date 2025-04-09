using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    public delegate void loadGame();
    public static event loadGame onLoadGame;
    //public DOTweenAnimation loadingBar;
    //public Slider Slider;

    private Image loadingImage;
    private float loadingSpeed = 0.05f;
    private bool isloading = false;
    private void Awake()
    {
        onLoadGame += LoadGameTriggered;
    }
    private void Start()
    {
    //    GameInit();
     //   loadingImage = loadingBar.GetComponent<Image>();

    }
    private void Update()
    {
        if (!isloading)
        {
          //  loadingImage.fillAmount += loadingSpeed * Time.deltaTime;
         //   Slider.value += loadingSpeed * Time.deltaTime;
        }
    }
    private void OnEnable()
    {
        onLoadGame += LoadGameTriggered;
    }
    private void OnDestroy()
    {
        onLoadGame -= LoadGameTriggered;
    }

    void LoadGameTriggered()
    {
        isloading = true;
        //Slider.GetComponent<SliderTween>().enabled = true;
        //Slider.GetComponent<SliderTween>().RestartTween();
        //loadingBar.DORestart();
    }

    public void SplashLoad()
    {
        SceneManager.LoadScene("MainMenu");
        //AdsManager.Instance.DestroyAppOpen();
        MenuManager.isLevel = false;

    }

    public static void SplashStatus()
    {
        onLoadGame();
    }

    //void GameInit()
    //{
    //    if (PlayerPrefs.GetInt(CustomPlayerPrefs.gameInit) != 1)
    //    {
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.activeGameMode, 1);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, 1);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlockedPakring, 1);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.activeLevel, 1);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, 0);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.activeVehicle, 1);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 1, 1);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
    //        PlayerPrefs.SetFloat(CustomPlayerPrefs.musicToggle, 1f);
    //        PlayerPrefs.SetFloat(CustomPlayerPrefs.soundToggle, 1f);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.removeAds_IAP, 0);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.allGame_IAP, 0);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.allLevel_IAP, 0);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.freeCoinGiveAway, 0);
    //        PlayerPrefs.SetInt(CustomPlayerPrefs.gameInit, 1);
    //    }
    //}
}
