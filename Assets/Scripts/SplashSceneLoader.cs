using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashSceneLoader : MonoBehaviour
{
    [SerializeField] private Image Loading;
    [SerializeField] private Text versionNumber;
    [SerializeField] private Text Percent;
   // [SerializeField] private Image LoadingBG;
    private void Awake()
    {
        versionNumber.text = "v " + Application.version;
        PlayerPrefs.SetInt("InitializeAds", 0);
    }
    private void Update()
    {
        Percent.text = (Loading.fillAmount * 100).ToString("F0") + "%";
    }
    private void Start()
    {
        GameInit();
        Loading.fillAmount = 0;
        Loading.DOFillAmount(0.75f, 8.25f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            Loading.DOFillAmount(0.8f, 2.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
               
                Loading.DOFillAmount(1f, 1.75f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync("MainMenu");
                });
            });
        });
        //   LoadingBG.transform.DOLocalMoveY(7, 13.25f).SetEase(Ease.Linear);
        
    }

    IEnumerator LoadScene()
    {
        yield return null;
        yield return new WaitForSeconds(1);
        //if (!MediationAdsController.Instance.AppOpenShown)
        //    SceneManager.LoadSceneAsync("MainMenu");

    }
    public void SplashLoad()
    {
        SceneManager.LoadScene("MainMenu");
        //AdsManager.Instance.DestroyAppOpen();
        MenuManager.isLevel = false;

    }


    void GameInit()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.gameInit) != 1)
        {
            PlayerPrefs.SetInt(CustomPlayerPrefs.activeGameMode, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlockedPakring, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.activeLevel, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, 0);
            PlayerPrefs.SetInt(CustomPlayerPrefs.activeVehicle, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 1, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
            PlayerPrefs.SetFloat(CustomPlayerPrefs.musicToggle, 1f);
            PlayerPrefs.SetFloat(CustomPlayerPrefs.soundToggle, 1f);
            PlayerPrefs.SetInt(CustomPlayerPrefs.removeAds_IAP, 0);
            PlayerPrefs.SetInt(CustomPlayerPrefs.allGame_IAP, 0);
            PlayerPrefs.SetInt(CustomPlayerPrefs.allLevel_IAP, 0);
            PlayerPrefs.SetInt(CustomPlayerPrefs.freeCoinGiveAway, 0);
            PlayerPrefs.SetInt(CustomPlayerPrefs.gameInit, 1);
        }
    }
}

