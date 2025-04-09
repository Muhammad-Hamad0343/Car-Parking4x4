using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;
using System.Net.Mime;
using UnityEngine.PlayerLoop;

public class MenuManager : MonoBehaviour
{
    public delegate void updateCash();
    public static event updateCash onCashUpdated;
    public delegate void PlayClickSfx();
    public static event PlayClickSfx playSfx;

    public static MenuManager instance;

    [Header("UI-Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject vehicleSelectionPanel;
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject avatarSelectionPanel;
    [SerializeField] private GameObject notEnoughCoinsPanel;
    [SerializeField] private GameObject noAdsPanel;
    [SerializeField] private TextMeshProUGUI adsStatusText;
    [SerializeField] private GameObject freeCoinsPanel;

    [Header("Player Avatar")]
    [SerializeField] private Image avatarImage;
    [SerializeField] private GameObject saveButton;
    [SerializeField] private Sprite evaSprite;
    [SerializeField] private Sprite jackSprite;
    [SerializeField] private Sprite maxSprite;

    [Header("In App Panel")]
    public GameObject unlockAllGamePanel;
    public GameObject unlockAllLevelPanel;
    public GameObject removeAds;
    public TextMeshProUGUI removeAdsPriceText;
    public TextMeshProUGUI unloclAllGamePriceText;
    public TextMeshProUGUI unlockAllLevelPriceText;

    [Header("Coins Panel")]
    [SerializeField] private TextMeshProUGUI gameCoinsText;
    [SerializeField] private AudioSource coinsSfx;
    [SerializeField] private AudioSource CoinSound;

    [Header("Setting Panel")]
    [SerializeField] private Toggle steeringControlToggle;
    [SerializeField] private Toggle buttonControlsToggle;
    [SerializeField] private Slider sfxToggle;
    [SerializeField] private Slider musicToggle;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Vehicle Selection")]
    [SerializeField] private Vehicles[] _vehicles;
    [SerializeField] private GameObject[] TickIcons;
    [SerializeField] private GameObject[] LockIcons;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject rewardUnlockButton;
    [SerializeField] private GameObject priceLabel;
    [SerializeField] private GameObject vehicleLock;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image speedSlider;
    [SerializeField] private Image brakeSlider;
    [SerializeField] private Image gripSlider;
    [SerializeField] private Image tyreSlider;
    [SerializeField] private int currentSelectedVehicle = 1;
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject[] purchaseButton;

    [Header("Level Selection")]
    [SerializeField] private LevelData[] _Levels;
    [SerializeField] private ScrollRect levelScroller;
    [SerializeField] private GameObject NoThanksBtn;
    [SerializeField] private GameObject UnlockCarOnEvenLevelPanel;
    [SerializeField] private GameObject VehicleUnlockedPrompt;
    [SerializeField] private Sprite RewardParkingSprite;
    [SerializeField] private Sprite LockedOffroadSprite;
    [SerializeField] private Sprite DefaultParkingSprite;
    [SerializeField] private Sprite DefaultOffroadSprite;
    [SerializeField] private Sprite DoneParkingSprite;
    [SerializeField] private Sprite DoneOffroadSprite;
   // [SerializeField] private Sprite LockOffroadSprite;
    [SerializeField] private Sprite LockedParkingSprite;
    
    //[SerializeField] private RectTransform Content;
    //[SerializeField] private RectTransform ParkingLevelContent;
    //[SerializeField] private RectTransform OffRoadLevelContent;

    public static bool isLevel = false;
    public static bool EvenLevel = false;
    public static bool freecash;

   public bool BannerAds; 
    private void Awake()
    {
        Application.targetFrameRate = 60;
        MakeSingleton();
    }
    private void Start()
    {
        AdsManager.Instance.ShowBanner();
        AdsManager.Instance.ProtectiveLayer.SetActive(false);
        AvatarSelection();
        ActiveVehicle(PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle));
        ManageAvatar();
        onCashUpdated();
        ManageControlsSetting();
        ManageSound_Music();

        if (isLevel)
        {
            PanelHandler(mainPanel, levelSelectionPanel);
            if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode).Equals(1))
            {
                ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked));
                if (EvenLevel)
                {
                    UnlockCarOnEvenLevelPanel.SetActive(true);
                }
            }
            else if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode).Equals(2))
            {
                ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring));
                if (EvenLevel)
                {
                    UnlockCarOnEvenLevelPanel.SetActive(true);
                }
            }
            isLevel = false;
            EvenLevel = false;

        }
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds_IAP).Equals(1) || PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            removeAds.SetActive(false);
        StartCoroutine(DisableAfterSec());
     //   PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, +25000);
    }


    private void Update()
    {
        if (PlayerPrefs.GetInt("BanneradsFirsttime") == 1 && SceneManager.GetActiveScene().buildIndex != 0 && BannerAds)
        {
            //Debug.Log("Ads Banner Showing ");
            AdsManager.Instance.ShowBanner();
            BannerAds = false;
            PlayerPrefs.SetInt("BanneradsFirsttime", 0);
        }
    }
    IEnumerator DisableAfterSec()
    {
        yield return new WaitForSeconds(0.25f);
        // mainCam.GetComponent<RG_RotateAround>().enabled = false;
    }

    private void OnEnable()
    {
        onCashUpdated += CoinsUpdate;
        playSfx += SoundSfx;

    }
    private void OnDisable()
    {
        onCashUpdated -= CoinsUpdate;
        playSfx -= SoundSfx;
    }

    #region Private Funtion
    public static void UpdateGameCash()
    {
        onCashUpdated();
    }
    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
   
    private void PanelHandler(GameObject fromPanel, GameObject toPanel)
    {
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
    }
    void AvatarSelection()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.playerAvatar) == 0)
        {
            //    PanelHandler(mainPanel, avatarSelectionPanel);
            avatarSelectionPanel.SetActive(true);
            SelectAvatar(1);
        }
        else
        {
            //  AdsManager.Instance.ShowBanner();
        }
    }

    void CoinsUpdate()
    {
        gameCoinsText.GetComponent<NumberCounter>().Value = PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins);
        coinsSfx.Play();
    }
    void ManageAvatar()
    {
        Sprite avatarSprite = PlayerPrefs.GetInt(CustomPlayerPrefs.playerAvatar) switch
        {
            1 => evaSprite,
            2 => jackSprite,
            3 => maxSprite,
            _ => null,
        };
        avatarImage.sprite = avatarSprite;
    }
    private void SoundSfx()
    {
        sfxAudioSource.Play();
    }
    bool InternetStatus()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            return true;
        }
        else
            return false;
    }
    #endregion

    #region ButtonEvents
    public void SelectAvatar(int id)
    {
        int result = id switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            _ => throw new System.NotImplementedException(),
        };
        PlayerPrefs.SetInt(CustomPlayerPrefs.playerAvatar, result);
        saveButton.SetActive(true);
        playSfx();
    }
    public void OnClickSaveAvatar()
    {
        ManageAvatar();
        playSfx();

        PanelHandler(avatarSelectionPanel, mainPanel);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Main_Menu");
        //AdsManager.Instance.ShowInterstitialAd(() =>
        //{
        //    PanelHandler(avatarSelectionPanel, mainPanel);
        ////    AdsManager.Instance.ShowBanner();
        //});
    }


    public void OnClickStart()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.firstTime_User) == 0)  // First Time User
        {
            AdsManager.Instance.ShowInterstitialAd(() =>
            {


                loadingPanel.SetActive(true);
                PlayerPrefs.SetInt(CustomPlayerPrefs.activeGameMode, 2);
                //   SceneManager.LoadScene("ParkingMode");
            });

        }
        else
        {
            //  mainCam.GetComponent<RG_RotateAround>().enabled = true;
            // mainCam.GetComponent<RG_RotateAround>().offset = new Vector3(1.5f, 1.5f, -2);
            PanelHandler(mainPanel, vehicleSelectionPanel);
            unloclAllGamePriceText.text = InAppManager.Instance.GetPrice(InAppManager.allGame);
            ChangeVehicle();
            playSfx();
        }


        //if (!PlayerPrefs.GetInt(CustomPlayerPrefs.freeCoinGiveAway).Equals(1))
        //{
        //    freeCoinsPanel.SetActive(true);
        //    NoThanksBtn.SetActive(true);
        //}
        //else
        //{
        //    freeCoinsPanel.SetActive(false);
        //
        //    
        //    if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1) && InternetStatus())
        //    {
        //        Debug.LogError("Panel!!");
        //        unlockAllGamePanel.SetActive(true);
        //    }
        //        
        //}
    }
    public void OnClickBackVehicleSelection()
    {
        PanelHandler(vehicleSelectionPanel, mainPanel);
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds_IAP).Equals(1) || PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            removeAds.SetActive(false);
        else
        {
            if (InternetStatus())
                removeAds.SetActive(true);
        }
        ActiveVehicle(PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle));
        playSfx();
        // mainCam.GetComponent<RG_RotateAround>().offset = new Vector3(0, 0, 0);
        //mainCam.GetComponent<RG_RotateAround>().enabled = false;
    }

    public void OnClickQuit()
    {
        playSfx();
        //   AdsManager.Instance.HideBanner();
        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            //    AdsManager.Instance.ShowLargeBanner();
            PanelHandler(mainPanel, quitPanel);
        });
    }
    public void OnClickYes_Quit()
    {
        playSfx();
        Application.Quit();
    }
    public void OnClickNo_Quit()
    {
        playSfx();
        //   AdsManager.Instance.ShowBanner();
        //    AdsManager.Instance.HideLargeBanner();
        PanelHandler(quitPanel, mainPanel);
    }
    public void OnClickCloseUnLockAllGamePanel()
    {
        playSfx();
        unlockAllGamePanel.SetActive(false);
    }
    public void OnClickSetting()
    {
        playSfx();
        //    AdsManager.Instance.HideBanner();

        PanelHandler(mainPanel, settingPanel);
        sfxToggle.value = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
        musicToggle.value = PlayerPrefs.GetFloat(CustomPlayerPrefs.musicToggle);
        //    AdsManager.Instance.ShowLargeBanner();
        //AdsManager.Instance.ShowInterstitialAd(() =>
        //{
        //    //PanelHandler(mainPanel, settingPanel);
        //    //sfxToggle.value = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
        //    //musicToggle.value = PlayerPrefs.GetFloat(CustomPlayerPrefs.musicToggle);
        //    //AdsManager.Instance.ShowLargeBanner();
        //});
    }

    public void OnClickBackModeSelection()
    {
        playSfx();
        PanelHandler(modeSelectionPanel, vehicleSelectionPanel);
        ChangeVehicle();

    }

    //active game mode 1 = Offroad mode (15 levels), active game mode 2 = parking mode (10 levels)
    public void OnClickOffroadMode()
    {
        playSfx();
        PanelHandler(modeSelectionPanel, levelSelectionPanel);
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.allLevel_IAP).Equals(1) || PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            unlockAllLevelPanel.SetActive(false);
        else
        {
            if (InternetStatus())
                unlockAllLevelPanel.SetActive(true);
        }
        PlayerPrefs.SetInt(CustomPlayerPrefs.activeGameMode, 1);
        ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked));

        Firebase.Analytics.FirebaseAnalytics.LogEvent("OffRoad_Mode");
    }
    public void OnClickParkingMode()
    {
        playSfx();
        PanelHandler(modeSelectionPanel, levelSelectionPanel);
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.allLevel_IAP).Equals(1) || PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            unlockAllLevelPanel.SetActive(false);
        else
        {
            if (InternetStatus())
                unlockAllLevelPanel.SetActive(true);
        }
        PlayerPrefs.SetInt(CustomPlayerPrefs.activeGameMode, 2);
        ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring));

        Firebase.Analytics.FirebaseAnalytics.LogEvent("Parking_Mode");
    }
    public void OnClickFreeCash()
    {
        playSfx();
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowAdmobRewardedVideo(1);
    }
    public void FreeAdsPanel(string message)
    {
        adsStatusText.text = message;
    }
    Coroutine textAniamtion;
    public void NoAdsPanel()
    {
        //Debug.Log("Close panel ");
        noAdsPanel.SetActive(false);
        adsStatusText.text = "please Wait.";
    }
    public void StopAnimation()
    {
        StopCoroutine(textAniamtion);
    }
    IEnumerator AnimateText()
    {
        yield return new WaitForSeconds(0.5f);
        adsStatusText.text = "please Wait.";
        yield return new WaitForSeconds(0.5f);
        adsStatusText.text = "please Wait..";
        yield return new WaitForSeconds(0.5f);
        adsStatusText.text = "please Wait...";
        textAniamtion = StartCoroutine(AnimateText());

    }
    public void OnClickPrivacyPolicy()
    {
        playSfx();
        Application.OpenURL("https://tngmobileapps.com/privacy/");
    }
    public void OnClickRateUs()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.offroad.racing.jeep.driving.game");
    }
    public void OnClickMoreGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=TNG+Games");
    }
    public void OnClickRemoveAds()
    {
        InAppManager.Instance.PurchaseRemoveADS();
    }
    public void OnClickBuyAllGame()
    {
        InAppManager.Instance.PurchaseAllGame();
    }
    public void OnClickBuyAllLevel()
    {
        InAppManager.Instance.PurchaseAllLevel();
    }

    

    public void OnClickGet1000FreeCoins()
    {
        playSfx();
        freeCoinsPanel.SetActive(false);
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowRewardInterstitialAdDelay(RewardedInterstitialType.Coins);
    }

    public void ResetScaleToZero()
    {
        NoThanksBtn.transform.localScale = Vector3.zero;
        NoThanksBtn.gameObject.SetActive(false);
    }

    #endregion


    #region vehiceleSelection
    void ActiveVehicle(int id)
    {
        foreach (Vehicles v in _vehicles)
            v.vehicleObject.SetActive(false);
        _vehicles[id - 1].vehicleObject.SetActive(true);
        speedSlider.fillAmount = _vehicles[id - 1].vehicleProperties.speed;
        brakeSlider.fillAmount = _vehicles[id - 1].vehicleProperties.Brake;
        gripSlider.fillAmount = _vehicles[id - 1].vehicleProperties.Grip;
        tyreSlider.fillAmount = _vehicles[id - 1].vehicleProperties.Tyre;
        _vehicles[id - 1].vehicleObject.transform.localPosition = _vehicles[id - 1].spawnPosition;
        currentSelectedVehicle = id;
        int i = id - 1;
        for (int j = 0; j < TickIcons.Length; j++)
        {
            TickIcons[j].SetActive(j == i);
        }
    }
    public void ChangeVehicle()
    {
        foreach (Vehicles g in _vehicles)
            g.vehicleObject.SetActive(false);
        _vehicles[currentSelectedVehicle - 1].vehicleObject.SetActive(true);
        speedSlider.fillAmount = _vehicles[currentSelectedVehicle - 1].vehicleProperties.speed;
        brakeSlider.fillAmount = _vehicles[currentSelectedVehicle - 1].vehicleProperties.Brake;
        gripSlider.fillAmount = _vehicles[currentSelectedVehicle - 1].vehicleProperties.Grip;
        tyreSlider.fillAmount = _vehicles[currentSelectedVehicle - 1].vehicleProperties.Tyre;
        _vehicles[currentSelectedVehicle - 1].vehicleObject.transform.localPosition = _vehicles[currentSelectedVehicle - 1].spawnPosition;
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + currentSelectedVehicle) == 1)
        {
            selectButton.SetActive(true);
            buyButton.SetActive(false);
            rewardUnlockButton.SetActive(false);
            priceLabel.SetActive(false);
            vehicleLock.SetActive(false);
            SetLockIcons();
        }
        else
        {
            selectButton.SetActive(false);
            priceLabel.SetActive(true);
            buyButton.SetActive(true);
            rewardUnlockButton.SetActive(true);
            vehicleLock.SetActive(true);
            priceText.text = _vehicles[currentSelectedVehicle - 1].vehicleProperties.price.ToString();
        }
    }
    private void SetLockIcons()
    {
        for (int i = 1; i < LockIcons.Length; i++)
        {
            if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + i) == 1)
            {
                LockIcons[i - 1].SetActive(false);
            }
        }
    }

    public void SetAllLockIconsFalse()
    {
        foreach (GameObject item in LockIcons)
        {
            item.SetActive(false);
        }
    }
    public void OnClickNext_Vehicle()
    {
        playSfx();
        if (currentSelectedVehicle < _vehicles.Length)
        {
            currentSelectedVehicle++;
            ChangeVehicle();
        }
        else
        {
            currentSelectedVehicle = 1;
            ChangeVehicle();
        }
    }
    public void OnClickPrevious_Vehicle()
    {
        playSfx();
        if (currentSelectedVehicle > 1)
        {
            currentSelectedVehicle--;
            ChangeVehicle();
        }
        else
        {
            currentSelectedVehicle = 4;
            ChangeVehicle();
        }
    }
     int Cno = 0 ;
    public void SelectVehicle(int index)
    {      
        currentSelectedVehicle = index;
        if (Cno != currentSelectedVehicle)
        {
            ChangeVehicle();
            Cno = currentSelectedVehicle;
        }
        int i = index - 1;
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + index) == 1)
        {
            for (int j = 0; j < TickIcons.Length; j++)
            {
                TickIcons[j].SetActive(j == i);
            }
        }
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Select_Vehicle" + index);
    }
    public void OnClicKBuy()
    {
        playSfx();
        Debug.Log("Vehicles_index - " + currentSelectedVehicle);

        if (PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) >= _vehicles[currentSelectedVehicle - 1].vehicleProperties.price)
        {
            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + currentSelectedVehicle, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) - _vehicles[currentSelectedVehicle - 1].vehicleProperties.price);
            onCashUpdated();
            ChangeVehicle();
        }
        else
        {

            switch (currentSelectedVehicle)
            {
                case 1:
                    //Nothing
                    break;
                case 2:
                    InAppManager.Instance.PurchaseFerrari();
                    break;
                case 3:
                    InAppManager.Instance.PurchaseTrailBlazer();
                    break;
                case 4:
                    InAppManager.Instance.PurchaseWildRunner();
                    break;
                case 5:
                    InAppManager.Instance.PurchaseNissan();
                    break;
                case 6:
                    InAppManager.Instance.PurchaseDirtHawk();
                    break;
                case 7:
                    InAppManager.Instance.PurchaseLambo();
                    break;
                case 8:
                    InAppManager.Instance.PurchaseMudMaster();
                    break;

            }

           /* for (int i = 0; i < purchaseButton.Length; i++)
            {
                if (i == currentSelectedVehicle - 1)
                    purchaseButton[i].SetActive(true);
            }
            //Debug.Log("Vehicles_index - " + _vehicles[currentSelectedVehicle].name);
            notEnoughCoinsPanel.SetActive(true);*/
        }
    }


    public void OnClickOk_NotEnoughCoinsPanel()
    {
        playSfx();
        notEnoughCoinsPanel.SetActive(false);
        for (int i = 1; i < purchaseButton.Length; i++)
        {
            purchaseButton[i].SetActive(false);
        }
    }
    public void OnClicKSelectVehicle()
    {
        // mainCam.GetComponent<RG_RotateAround>().enabled = false;
        playSfx();

        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            PlayerPrefs.SetInt(CustomPlayerPrefs.activeVehicle, currentSelectedVehicle);
            PanelHandler(vehicleSelectionPanel, modeSelectionPanel);
        });
    }
    public void OnClickVehicleRewardUnlock()
    {
        playSfx();
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowAdmobRewardedVideo(GetSeletecCarId());

    }
    int GetSeletecCarId()
    {
        int carStatus = currentSelectedVehicle switch
        {
            1 => 5,
            2 => 6,
            3 => 7,
            4 => 8,
            5 => 9,
            6 => 10,
            7 => 11,
            8 => 12,            
            _ => 5,
        };
        return carStatus;


    }

    public void OnClickVehicleRewardUnlockOnEvenLevel()
    {
        playSfx();
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowRewardInterstitialAdDelay(RewardedInterstitialType.Cars, CheckWhichCarToUnlock(), () => StartCoroutine(ShowAndHidePrompt()));
        UnlockCarOnEvenLevelPanel.SetActive(false);
    }


    IEnumerator ShowAndHidePrompt()
    {
        yield return new WaitForSeconds(0.1f);
        VehicleUnlockedPrompt.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        VehicleUnlockedPrompt.SetActive(false);
    }

    private int CheckWhichCarToUnlock()
    {
        int carToUnlock = 0;
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 2.ToString()) == 0)
        {
            carToUnlock = 2;
        }
        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 3.ToString()) == 0)
        {
            carToUnlock = 3;
        }
        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 4.ToString()) == 0)
        {
           // Debug.Log("Unlock_this_Car");
            carToUnlock = 4;
        }
        return carToUnlock;
    }

    #endregion

    #region SettingPanel
    void ManageControlsSetting()
    {
        switch (PlayerPrefs.GetInt(CustomPlayerPrefs.controlType))
        {
            case 1:
                steeringControlToggle.isOn = true;
                buttonControlsToggle.isOn = false;
                break;
            case 2:
                steeringControlToggle.isOn = false;
                buttonControlsToggle.isOn = true;
                break;
        }
    }
    void ManageSound_Music()
    {
        //switch (PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle))
        //{
        //    case 1:
        //        sfxToggle.isOn = true;
        //        sfxAudioSource.mute = false;
        //        break;
        //    case 0:
        //        sfxToggle.isOn = false;
        //        sfxAudioSource.mute = true;
        //        break;
        //}
        //switch (PlayerPrefs.GetInt(CustomPlayerPrefs.musicToggle))
        //{
        //    case 1:
        //        musicToggle.isOn = true;
        //        musicAudioSource.mute = false;
        //        break;
        //    case 0:
        //        musicToggle.isOn = false;
        //        musicAudioSource.mute = true;
        //        break;
        //}
        sfxAudioSource.volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle, 1);
        musicAudioSource.volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.musicToggle, 1);
        CoinSound.volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);

    }
    public void OnSteeringToggleValueChanged()
    {
        playSfx();
        switch (steeringControlToggle.isOn)
        {
            case true:
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
                buttonControlsToggle.isOn = false;
                break;
            case false:
                steeringControlToggle.isOn = false;
                buttonControlsToggle.isOn = true;
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 2);
                break;
        }
    }
    public void OnButtonToggleValueChanged()
    {
        playSfx();
        switch (buttonControlsToggle.isOn)
        {
            case true:
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 2);
                steeringControlToggle.isOn = false;
                break;
            case false:
                buttonControlsToggle.isOn = false;
                steeringControlToggle.isOn = true;
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
                break;
        }
    }
    public void OnSfxSliderValueChanged()
    {

        PlayerPrefs.SetFloat(CustomPlayerPrefs.soundToggle, sfxToggle.value);
        sfxAudioSource.volume = sfxToggle.value;
        sfxAudioSource.Play();
    }
    public void OnMusicToggleValueChanged()
    {
        playSfx();
        PlayerPrefs.SetFloat(CustomPlayerPrefs.musicToggle, musicToggle.value);
        musicAudioSource.volume = musicToggle.value;
    }
    public void OnClickSave_SettingPanel()
    {
        playSfx();
        PanelHandler(settingPanel, mainPanel);
        //AdsManager.Instance.HideLargeBanner();
        //AdsManager.Instance.ShowBanner();
    }
    #endregion


    #region LevelSelection

    public void ManageLevels(int currentModeLevelStatus)
    {
        CheckRewardvideobutton();
        //****************************************** New Logic for Level Selection *****************************************

        if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 2) // Parking Mode
        {
            //Debug.Log("current - level --=>"+ currentModeLevelStatus);
            //Debug.Log("active- Level- --=>" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
            //Debug.Log("levelUnlockedPakring) --=>>>>" + PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring));
            // Show all levels for Parking Mode, potentially up to 17 or more if you add levels
            for (int i = 0; i < _Levels.Length; i++)
            {
                if (i < 25) // Maximum levels for Parking Mode (adjust as needed)
                {
                    _Levels[i].lockImage.GetComponent<Image>().sprite = RewardParkingSprite;
                    _Levels[i].Medal.SetActive(false);
                    _Levels[i].levelButton.gameObject.SetActive(true); // Make level button visible
                    _Levels[i].levelButton.gameObject.GetComponent<Image>().sprite = DefaultParkingSprite;
                }
                else
                {
                    _Levels[i].levelButton.gameObject.SetActive(false); // Hide extra levels beyond 17
                }
            }
        }
        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 1) // Offroad Mode
        {
            // Show only the first 15 levels for Offroad Mode
            for (int i = 0; i < _Levels.Length; i++)
            {
                if (i < 15) // Only show 15 levels for Offroad Mode
                {
                    _Levels[i].lockImage.GetComponent<Image>().sprite = RewardParkingSprite;
                   // _Levels[i].lockImage.GetComponent<Image>().sprite = LockedOffroadSprite;
                    _Levels[i].Medal.SetActive(false);
                    _Levels[i].levelButton.gameObject.SetActive(true); // Make level button visible
                    _Levels[i].levelButton.gameObject.GetComponent<Image>().sprite = DefaultOffroadSprite;
                }
                else
                {
                    _Levels[i].levelButton.gameObject.SetActive(false); // Hide levels beyond 15
                }
            }
        }


        //****************************************** Old Logic for Level Selection *****************************************
        #region Old Logic


        //if(PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 2) // It means it is parking mode
        //{
        //    for (int i = 0; i < 17; i++)
        //    {
        //        _Levels[i].lockImage.GetComponent<Image>().sprite = LockedParkingSprite;
        //        _Levels[i].Medal.SetActive(false);
        //        _Levels[i].levelButton.gameObject.SetActive(i < 17);
        //        _Levels[i].levelButton.gameObject.GetComponent<Image>().sprite = DefaultParkingSprite;

        //    }

        //    //Content.rect.Set(ParkingLevelContent.rect.x, ParkingLevelContent.rect.y, ParkingLevelContent.rect.width, ParkingLevelContent.rect.height);

        //}
        //else if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 1)
        //{
        //    for (int i = 0; i < 15; i++)
        //    {
        //        _Levels[i].lockImage.GetComponent<Image>().sprite = LockedOffroadSprite;
        //        _Levels[i].Medal.SetActive(false);
        //        _Levels[i].levelButton.gameObject.SetActive(true);
        //        _Levels[i].levelButton.gameObject.GetComponent<Image>().sprite = DefaultOffroadSprite;

        //        if(PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 1 /*&& i > 15*/)
        //        {
        //            if(i >= 14)
        //            {
        //                _Levels[i+ _Levels.Length-1].levelButton.gameObject.SetActive(false);
        //            }

        //        }
        //    }

        //    //Content.rect.Set(OffRoadLevelContent.rect.x, OffRoadLevelContent.rect.y, OffRoadLevelContent.rect.width, OffRoadLevelContent.rect.height);
        //}
        #endregion


        for (int i = 1; i <= _Levels.Length; i++)
        {
            if (i <= currentModeLevelStatus)
            {
                _Levels[i - 1].lockImage.SetActive(false);
                _Levels[i - 1].levelButton.interactable = true;
            }
            else
            {
                _Levels[i - 1].lockImage.SetActive(true);
                _Levels[i - 1].levelButton.interactable = false;
            }
            _Levels[i - 1].levelStatusText.text = _Levels[i - 1]._levelTitle;

            if (PlayerPrefs.GetInt(CustomPlayerPrefs.allLevel_IAP).Equals(1) || PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 1)//Offroad
                {
                    for (int k = 0; k < 15; k++)
                    {
                        if (PlayerPrefs.GetInt("PlayedLevel" + k.ToString()) == 1)
                        {
                            _Levels[k].Medal.SetActive(true);
                            _Levels[k].levelButton.gameObject.GetComponent<Image>().sprite = DoneOffroadSprite;
                            //if(k == 0) 
                            //    levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[k].levelButton.GetComponent<RectTransform>());
                            //else
                            //    levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[k - 1].levelButton.GetComponent<RectTransform>());

                            levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[PlayerPrefs.GetInt("RecentOffroadLevelPlayed")].levelButton.GetComponent<RectTransform>());
                          // Debug.Log("OffRoad PlayerLevel mode level Played............" + k);
                        }
                    }


                }

                if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 2)//Parking
                {
                    for (int k = 0; k < 25; k++)
                    {
                        if (PlayerPrefs.GetInt("PlayedLevelParking" + k.ToString()) == 1)
                        {
                            _Levels[k].Medal.SetActive(true);
                            _Levels[k].levelButton.gameObject.GetComponent<Image>().sprite = DoneParkingSprite;
                            //if (k == 0)
                            //    levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[k].levelButton.GetComponent<RectTransform>());
                            //else
                            //    levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[k - 1].levelButton.GetComponent<RectTransform>());
                            levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[PlayerPrefs.GetInt("RecentParkingLevelPlayed")].levelButton.GetComponent<RectTransform>());

                        }
                    }
                }
            }
            else
            {
                if (currentModeLevelStatus >= 2)
                {
                    for (int k = 0; k < currentModeLevelStatus - 1; k++)
                    {
                        _Levels[k].Medal.SetActive(true);
                        if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 2) //Parking
                            _Levels[k].levelButton.gameObject.GetComponent<Image>().sprite = DoneParkingSprite;
                        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode) == 1)//Offroad
                            _Levels[k].levelButton.gameObject.GetComponent<Image>().sprite = DoneOffroadSprite;
                    }
                }
                levelScroller.content.localPosition = levelScroller.GetSnapToPositionToBringChildIntoView(_Levels[currentModeLevelStatus - 1].levelButton.GetComponent<RectTransform>());
            }
        }

        //Parking 
        if (currentModeLevelStatus == PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring))
        {
            _Levels[currentModeLevelStatus].LevelUnlockRewardButtton.SetActive(true);
            for (int i = PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring); i < 24; i++)
            {
                _Levels[i + 1].lockImage.GetComponent<Image>().sprite = LockedParkingSprite;
            }
        } 
        
        //Game Mode / OffRoad
        if (currentModeLevelStatus == PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked))
        {
            _Levels[currentModeLevelStatus].LevelUnlockRewardButtton.SetActive(true);
            for (int i = PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked); i < 14; i++)
            {
                _Levels[i + 1].lockImage.GetComponent<Image>().sprite = LockedParkingSprite;
            }
        }
    }
    public void CheckRewardvideobutton()
    {        
        for(int i = 0;i<_Levels.Length;i++)
        {
            _Levels[i].LevelUnlockRewardButtton.SetActive(false);           
        }
    }
    public void OnClickBackLevelSelection()
    {
        playSfx();
        PanelHandler(levelSelectionPanel, modeSelectionPanel);
       // mainCam.GetComponent<RG_RotateAround>().enabled = true;
    }
    public void OnClickLevel(int id)
    {
        PlayerPrefs.SetInt(CustomPlayerPrefs.activeLevel, id);
        //    AdsManager.Instance.HideBanner();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            AdsManager.Instance.ShowInterstitialAd(() =>
            {
                PanelHandler(levelSelectionPanel, loadingPanel);
            });
        }
        else
        {
            // Debug.Log("Active-Level_ID - " + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
        }
    }


    #region Unlock level by watching reward
    public void OnClickLevelunlockreward()
    {
     //   playSfx();
        // noAdsPanel.SetActive(true);
        playSfx();
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowAdmobRewardedVideo(13);
    }
  
    public void UnLockLevel()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode).Equals(1))
        {
            if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) <= PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) + 1 && PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) < 15)
            {
                PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) + 1);
            }
            ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked));

            PlayerPrefs.SetInt(CustomPlayerPrefs.activeLevel, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked));

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                PanelHandler(levelSelectionPanel, loadingPanel);
            }

           
        }
        else if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode).Equals(2))
        {
            if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) <= PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) + 1 && PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) < 25)
            {
                PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlockedPakring, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) + 1);
            }

            ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring));

            PlayerPrefs.SetInt(CustomPlayerPrefs.activeLevel, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring));

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                PanelHandler(levelSelectionPanel, loadingPanel);
            }
        }
    }
    #endregion

    public void StartLevel()
    {
        Resources.UnloadUnusedAssets();
        switch (PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode))
        {
            case 1:
                SceneManager.LoadScene("GamePlay");
                Firebase.Analytics.FirebaseAnalytics.LogEvent("OffRoad_Level_Play" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                break;
            case 2:
                SceneManager.LoadScene("ParkingMode");
                Firebase.Analytics.FirebaseAnalytics.LogEvent("Parking_Level_Play" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                break;
            case 3:
                break;
        }
    }
    #endregion
   
    // Vehicle unlock after Purchase
    public void VehiclePurchase()
    {
        PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + currentSelectedVehicle, 1);
        PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) - _vehicles[currentSelectedVehicle - 1].vehicleProperties.price);
        onCashUpdated();
        ChangeVehicle();
    }
}

[System.Serializable]
public class Vehicles
{
    public string name;
    public GameObject vehicleObject;
    public Vector3 spawnPosition;
    public VehicleDetails vehicleProperties;
}
[System.Serializable]
public class LevelData
{
    public string _levelTitle;
    public Button levelButton;
    public GameObject lockImage;
    public TextMeshProUGUI levelStatusText;
    public GameObject Medal;
    public GameObject LevelUnlockRewardButtton;
}
public static class ScrollRectExtensions
{
    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 /*- (viewportLocalPosition.y + childLocalPosition.y)*/
        );
        return result;
    }
}
