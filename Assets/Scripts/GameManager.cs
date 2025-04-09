using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using NWH.Common.Cameras;
using NWH.VehiclePhysics2;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.VehicleGUI;
using NWH.Common.Input;
using UnityEngine.Audio;

public enum GameMode
{
    offroad,
    parking,
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public delegate void Gamewin();
    public static event Gamewin onGameWin;

    public GameMode activeMode;

    public GameObject[] PlayersVehicles;
    public GameObject Player;

    //public GameObject Test_cube ;

    [Header("Player Manager")]
  //  [SerializeField] private VehicleController[] _playerCars;
    [SerializeField] private MobileVehicleInputProvider vehicleInput;
    [SerializeField] private CameraChanger playerCamera;
    [SerializeField] private DashGUIController guiController;
    public VehicleController activePlayerVehicle;
    private int activePlayerVehicleIndex = 1;

    [Header("Ui Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject missionInstrutionPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject preCompletePanel;
    [SerializeField] private GameObject completePanel;
    [SerializeField] private GameObject preFailPanel;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private GameObject failPanel;
    public GameObject doubleRewardButton;
    public GameObject levelSkipButton;
    [SerializeField] private GameObject noAdsPanel;
    [SerializeField] private TextMeshProUGUI adsStatusText;
    [SerializeField] private Button NextLevelButton;


    [Header("Control Setting")]
    [SerializeField] private Image controlImage;
    [SerializeField] private Sprite steeringSprite;
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private GameObject steeringWheel;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private Toggle steeringControlToggle;
    [SerializeField] private Toggle buttonControlsToggle;

    [Header("Level Manager")]
    [SerializeField] private LevelDetails[] _level;
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;
    private int activeLevelIndex;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float countdownDuration = 60f;
    private float currentTime;
    private bool isTimerRunning = false;

    [Header("Level Reward")]
    [SerializeField] private TextMeshProUGUI coinRewardText;
    [SerializeField] private TextMeshProUGUI timeRewardText;
    [SerializeField] private TextMeshProUGUI totalRewardText;
    private int timeReward = 0;
    private int hitReward = 0;
    private int totalReward = 0;

    [Header("Level Revive")]
    [SerializeField] private Image reviveSlider;
    [SerializeField] private TextMeshProUGUI reviveTimerText;
    [SerializeField] private GameObject SkipButton;
    private float countDownTimer = 10f;
    private bool isRevive;
    private bool revived = false;

    [Header("Music & Sfx")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private Slider musicToggle;
    [SerializeField] private Slider sfxToggle;
    [SerializeField] private AudioMixer VehicleAudio;
    [SerializeField] private AudioSource LevelCompleteAudio;
    [SerializeField] private GameObject Star_1;
    [SerializeField] private GameObject Star_2;
    

    [Header("Parking Mode")]
    public int hits;

   

    [SerializeField] private bool canHit = true;

    bool triggerOnce = false;
    private bool DetectCollision;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Singleton();
      //  ActivePlayer();
    }

   
    private void Start()
    {
        //PlayersVehicles[PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle)].SetActive(true);
        
        Player = PlayersVehicles[PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle)];

        canHit = true;
        DetectCollision = true;
        ActiveLevel();
        SetTimer();
        SetControls();
        ManageSound_Music();
       // VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle)) * 20);
        AdsManager.Instance.ShowBanner();
        AdsManager.Instance.RewardedAdFailedOnLevel += HandleRewardedAdFailedOnLevel;
        AdsManager.Instance.RewardedAdFailedOnDismissRewardedAd += HandleRewardedAdFailedOnDismissRewardedAd;

        if (activeMode == GameMode.parking)
            timerText.text = HitsStatus();
    }



    /*private void Start()
    {
        PlayersVehicles[PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle)].SetActive(true);
        Player = PlayersVehicles[PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle)];
        canHit = true;
        DetectCollision = true;
       ActiveLevel();
        SetTimer();
        SetControls();
        ManageSound_Music();
        AdsManager.Instance.ShowBanner();
        AdsManager.Instance.RewardedAdFailedOnLevel += HandleRewardedAdFailedOnLevel;
        AdsManager.Instance.RewardedAdFailedOnDismissRewardedAd += HandleRewardedAdFailedOnDismissRewardedAd;

        if (activeMode == GameMode.parking)
            timerText.text = HitsStatus();
    }*/

    private void OnDestroy()
    {
        AdsManager.Instance.RewardedAdFailedOnLevel -= HandleRewardedAdFailedOnLevel;
        AdsManager.Instance.RewardedAdFailedOnDismissRewardedAd -= HandleRewardedAdFailedOnDismissRewardedAd;
    }
        
    public void OnEnable()
    {
        onGameWin += FinishedLineTrigger;
    }
    public void OnDisable()
    {
        onGameWin -= FinishedLineTrigger;
    }
    private void Update()
    {
        if (activeMode == GameMode.offroad)
            GameTimer();
        if (isRevive)
            ReviveTimer();     
    }
    public void FreeAdsPanel(string message)
    {
        adsStatusText.text = message;
    }
    public void NoAdsPanel()
    {
        noAdsPanel.SetActive(false);
        adsStatusText.text = "please Wait";
    }
    #region Private Event
    void Singleton()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    void PanelHandler(GameObject fromPanel, GameObject toPanel)
    {
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
    }
    void FinishedLineTrigger()
    {
        if (DetectCollision)
            WinCamera();

    }
    #endregion

    #region ControlSetting
    public void OnClickChangeControls()
    {
        switch (vehicleInput.steeringInputType)
        {
            case MobileVehicleInputProvider.HorizontalAxisType.SteeringWheel:
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 2);
                controlImage.sprite = buttonSprite;
              //  vehicleInput.steeringInputType = MobileVehicleInputProvider.HorizontalAxisType.Button;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.TouchScreen;
                steeringWheel.SetActive(false);
                buttonPanel.SetActive(true);
                break;
            case MobileVehicleInputProvider.HorizontalAxisType.Button:
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
                controlImage.sprite = steeringSprite;
           //     vehicleInput.steeringInputType = MobileVehicleInputProvider.HorizontalAxisType.SteeringWheel;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.SteeringWheel;
                steeringWheel.SetActive(true);
                buttonPanel.SetActive(false);
                break;
        }
    }
    void SetControls()
    {
        switch (PlayerPrefs.GetInt(CustomPlayerPrefs.controlType))
        {
            case 1:
                controlImage.sprite = steeringSprite;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.SteeringWheel;
                steeringControlToggle.isOn = true;
                buttonControlsToggle.isOn = false;
                steeringWheel.SetActive(true);
                buttonPanel.SetActive(false);
                break;
            case 2:
                controlImage.sprite = buttonSprite;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.TouchScreen;
                steeringWheel.SetActive(false);
                buttonPanel.SetActive(true);
                steeringControlToggle.isOn = false;
                buttonControlsToggle.isOn = true;
                break;
        }
    }
    public void OnSteeringToggleValueChanged()
    {
        switch (steeringControlToggle.isOn)
        {
            case true:
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
                controlImage.sprite = steeringSprite;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.SteeringWheel;
                buttonControlsToggle.isOn = false;
                steeringWheel.SetActive(true);
                buttonPanel.SetActive(false);
                break;
            case false:
                controlImage.sprite = buttonSprite;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.TouchScreen;
                steeringControlToggle.isOn = false;
                buttonControlsToggle.isOn = true;
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 2);
                steeringWheel.SetActive(false);
                buttonPanel.SetActive(true);
                break;
        }
    }
    public void OnButtonToggleValueChanged()
    {
        switch (buttonControlsToggle.isOn)
        {
            case true:
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 2);
                controlImage.sprite = buttonSprite;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.TouchScreen;
                steeringControlToggle.isOn = false;
                steeringWheel.SetActive(false);
                buttonPanel.SetActive(true);
                break;
            case false:
                controlImage.sprite = steeringSprite;
                RCC_Settings.Instance.mobileController = RCC_Settings.MobileController.SteeringWheel;
                buttonControlsToggle.isOn = false;
                steeringControlToggle.isOn = true;
                PlayerPrefs.SetInt(CustomPlayerPrefs.controlType, 1);
                steeringWheel.SetActive(true);
                buttonPanel.SetActive(false);
                break;
        }
    }
    #endregion

    #region Player
    void ActivePlayer()
    {
        activePlayerVehicleIndex = PlayerPrefs.GetInt(CustomPlayerPrefs.activeVehicle);
    //    activePlayerVehicle = _playerCars[activePlayerVehicleIndex - 1];
        playerCamera = activePlayerVehicle.gameObject.GetComponentInChildren<CameraChanger>();
        guiController.vehicleController = activePlayerVehicle;
        //if (PlayerPrefs.GetInt(CustomPlayerPrefs.musicToggle).Equals(1))
        //{
        //    activePlayerVehicle.soundManager.masterVolume = 0;
        //}
        //else
        //    activePlayerVehicle.soundManager.masterVolume = 1;
    }
    #endregion

    #region LevelHandler
   
    void ActiveLevel()
    {
        foreach (LevelDetails data in _level)
            data.levelObject.SetActive(false);
        activeLevelIndex = PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel);
        _level[activeLevelIndex - 1].levelObject.SetActive(true);
        //   activePlayerVehicle.gameObject.transform.position = _level[activeLevelIndex - 1].spawnPosition;
      //  Debug.Log(_level[activeLevelIndex - 1].spawnPosition + "possssssssssssssssssssss");
        Player.gameObject.transform.position = _level[activeLevelIndex - 1].spawnPosition;
        //    activePlayerVehicle.gameObject.transform.eulerAngles = _level[activeLevelIndex - 1].spawnRotation;
        Player.gameObject.transform.eulerAngles = _level[activeLevelIndex - 1].spawnRotation;
        Player.SetActive(true);

        levelTitleText.text = _level[activeLevelIndex - 1]._Level;
        levelDescriptionText.text = _level[activeLevelIndex - 1].levelDescription;
        //   activePlayerVehicle.gameObject.SetActive(true);
        countdownDuration = _level[activeLevelIndex - 1].levelTime;
        if (activeMode == GameMode.offroad)
            PanelHandler(hudPanel, missionInstrutionPanel);
        else
            hudPanel.SetActive(true);
    }




    /*void ActiveLevel()
    {        
        foreach (LevelDetails data in _level)
            data.levelObject.SetActive(false);
        activeLevelIndex = PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel);
        Debug.Log("active_Level_" + activeLevelIndex);
       _level[activeLevelIndex - 1].levelObject.SetActive(true);
        //   activePlayerVehicle.gameObject.transform.position = _level[activeLevelIndex - 1].spawnPosition;

        Debug.Log("Jeep_Pos_1_" + Player.transform.position);

        Debug.Log("spawnpos_" + _level[activeLevelIndex - 1].spawnPosition);
        
        Player.transform.localPosition = _level[activeLevelIndex - 1].spawnPosition;

        Debug.Log("Jeep_Pos_3_" + Player.transform.position);

     //   Invoke(nameof(Check), 0.5f);
        //    activePlayerVehicle.gameObject.transform.eulerAngles = _level[activeLevelIndex - 1].spawnRotation;

        Player.transform.eulerAngles = _level[activeLevelIndex - 1].spawnRotation;
                
        levelTitleText.text = _level[activeLevelIndex - 1]._Level;
        levelDescriptionText.text = _level[activeLevelIndex - 1].levelDescription;
        //   activePlayerVehicle.gameObject.SetActive(true);
        countdownDuration = _level[activeLevelIndex - 1].levelTime;
        if (activeMode == GameMode.offroad)
            PanelHandler(hudPanel, missionInstrutionPanel);
        else
            hudPanel.SetActive(true);
    }*/
    public void OnClickOk_DescriptionPanel()
    {
        //activePlayerVehicle.Start();
        PanelHandler(missionInstrutionPanel, hudPanel);
        StartTimer();
    }
    public Array GetWaypointArray()
    {
        return _level[activeLevelIndex - 1].waypoints;
    }
    #endregion

    #region GameTimer
    void SetTimer()
    {
        if (activeMode == GameMode.parking)
            return;
        currentTime = countdownDuration;
    }
    void StartTimer()
    {
        if (activeMode == GameMode.parking)
        {
            return;
        }

        isTimerRunning = true;
    }
    void GameTimer()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            timerText.text = FormatTime(currentTime);
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerText.text = "00:00";
                isTimerRunning = false;
                //GameOver here
                if (revived)
                    GameFail();
                else
                    ReviveFail();
            }
        }
    }
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #endregion

    #region Level Revive
    
    void ReviveTimer()
    {
        Debug.Log("Time to Revive");
        if (countDownTimer > 0)
        {
            countDownTimer -= Time.deltaTime;
            reviveTimerText.text = countDownTimer.ToString("0");
            reviveSlider.fillAmount = (countDownTimer / 10);
            if (countDownTimer <= 7f)
            {
                SkipButton.SetActive(true);
            }
            if (countDownTimer <= 0f)
            {
                countDownTimer = 0f;
                isRevive = false;
                revivePanel.SetActive(false);
                GameFail();
            }
        }
    }
    public void LevelRevive()
    {
        switch (activeMode)
        {
            case GameMode.offroad:
                currentTime = 60f;
                isTimerRunning = true;
                //activePlayerVehicle.vehicleRigidbody.isKinematic = false;
                Player.GetComponent<Rigidbody>().isKinematic = false;
                revivePanel.SetActive(false);
                hudPanel.SetActive(true);
                Time.timeScale = 1f;
                revived = true;
                break;
            case GameMode.parking:
                hits = 0;
                //CancelInvoke("HandleRewardedAdFailedOnDismissRewardedAd");
                timerText.text = HitsStatus();
                revivePanel.SetActive(false);
              //  activePlayerVehicle.vehicleRigidbody.isKinematic = false;
                Player.GetComponent<Rigidbody>().isKinematic = false;
                hudPanel.SetActive(true);
                Time.timeScale = 1f;
                revived = true;
                break;
        }
    }


    public void OnClickRevive()
    {
        isRevive = false;
        countDownTimer = 10f;
        revivePanel.SetActive(false);
        noAdsPanel.SetActive(true);
        DetectCollision = true;

        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowAdmobRewardedVideo(4);
        //Invoke("HandleRewardedAdFailedOnDismissRewardedAd", 2);
    }
    private void HandleRewardedAdFailedOnLevel()
    {
        isRevive = false;
        revivePanel.SetActive(false);
        GameFail();
    }

    private void HandleRewardedAdFailedOnDismissRewardedAd()
    {
        isRevive = false;
        revivePanel.SetActive(false);
        CameraMouseDrag cam = playerCamera.GetComponentInChildren<CameraMouseDrag>();
        cam.verticalMinAngle = 90;
        cam.verticalMaxAngle = 90;
        //PanelHandler(hudPanel, preFailPanel);
   //     activePlayerVehicle.soundManager.masterVolume = 0;
      //  activePlayerVehicle.vehicleRigidbody.isKinematic = true;
        Player.GetComponent<Rigidbody>().isKinematic = true;
    //    activePlayerVehicle.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(GameFailDelayCo());
        //Debug.LogError("Failed called on ad callback");
    }
    public void OnClickNoThanks()
    {
        isRevive = false;
        revivePanel.SetActive(false);
        GameFail();
    }
    #endregion


    #region ButtonFunction
    public void OnClickPause()
    {
        isTimerRunning = false;
        Time.timeScale = 0;
         VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle)) * 20); // Convert to dB
    //    AdsManager.Instance.HideBanner();
        AdsManager.Instance.ShowInterstitialGamePlayAd(() =>
        {
            PanelHandler(hudPanel, pausePanel);
            sfxToggle.value = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
            musicToggle.value = PlayerPrefs.GetFloat(CustomPlayerPrefs.musicToggle);
        //    AdsManager.Instance.ShowLargeBanner();
        });

    }
    public void OnClicKResume()
    {
        isTimerRunning = true;
        Time.timeScale = 1;
        //    AdsManager.Instance.ShowBanner();
        //    AdsManager.Instance.HideLargeBanner();
        VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle)) * 20); // Convert to dB
        PanelHandler(pausePanel, hudPanel);
    }
    public void OnClickRestart()
    {
    //    AdsManager.Instance.HideLargeBanner();
        Scene cene = SceneManager.GetActiveScene();
        Time.timeScale = 1;
        MenuManager.isLevel = false;
        SceneManager.LoadScene(cene.name);
    }
    public void OnClickMainMenu()
    {
     //   AdsManager.Instance.HideLargeBanner();
        //AdsManager.Instance.ProtectiveLayer.SetActive(true);
        Time.timeScale = 1;
        MenuManager.isLevel = false;
        StartCoroutine(MainMenuDelay());
        //AdsManager.Instance.ShowInterstitialAd(() =>
        //{
           
        //});

    }
    IEnumerator MainMenuDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");

    }
    public void OnClickNext()
    {
      //  AdsManager.Instance.HideLargeBanner();
        Time.timeScale = 1;
        MenuManager.isLevel = true;
       
        switch (activeMode)
        {
            case GameMode.offroad:
                PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode, 1);
               
                if (!AllVehiceUnlocked())
                {
                    if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 3 || PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 5 ||
                        PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 7 || PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 9 ||
                        PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 11 || PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 13 ||
                        PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) == 15)
                    { 
                        MenuManager.EvenLevel = true;
                    }
                }
                break;
            case GameMode.parking:
                PlayerPrefs.GetInt(CustomPlayerPrefs.activeGameMode, 2);
             //   PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel + 1);
               
                if (!AllVehiceUnlocked())
                {
                    if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) == 3 || PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) == 5 ||
                        PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) == 7 || PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) == 9)
                    {
                        MenuManager.EvenLevel = true;
                    }
                }
                break;
        }
       
    //    PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel + 1);

        PlayerPrefs.SetInt(CustomPlayerPrefs.activeLevel, PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) + 1);
        PlayerPrefs.Save();
 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle)) * 20);
        //   SceneManager.LoadScene("MainMenu");
    }

    private bool AllVehiceUnlocked()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 1.ToString()) == 1 && PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 2.ToString()) == 1 && PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 3.ToString()) == 1 && PlayerPrefs.GetInt(CustomPlayerPrefs.vehicleUnlock + 4.ToString()) == 1)
            return true;
        else return false;
    }
    #endregion

    #region GameWin
    public static void TriggerGameWin(GameManager instance)
    {
        if (!instance.triggerOnce)
        {
            onGameWin();
            instance.triggerOnce = true;
        }
    }
    void WinCamera()
    {
       // Debug.Log("SfxValue-=>" + sfxToggle.value);
       // Debug.Log("soundasd-=>" + PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle));
        if (PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle) < 0.2)
        {
            Star_1.SetActive(false);
            Star_2.SetActive(false);
           

            //Star_1.mute = true;
            //Star_2.mute = true;
        }
        else
        {
            Star_1.SetActive(true);
            Star_2.SetActive(true);
            Star_1.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
            Star_2.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
            //Star_1.mute = false;
            //Star_2.mute = false;
        }
        //CameraMouseDrag cam = playerCamera.GetComponentInChildren<CameraMouseDrag>();
        //cam.verticalMinAngle = 90;
        //cam.verticalMaxAngle = 90;
      
        PanelHandler(hudPanel, preCompletePanel);
        isTimerRunning = false;
        VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(0.0001f) * 20); // Convert to dB

        LevelCompleteAudio.volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
        LevelCompleteAudio.Play();
        Player.GetComponent<Rigidbody>().isKinematic = true;

        
        Invoke(nameof(GameWin), 2f);
    }
    private void GameWin()
    {
        LevelCompleteAudio.Stop();
        //  AdsManager.Instance.HideBanner();
        //     activePlayerVehicle.soundManager.masterVolume = 0;
       // Debug.Log("GameWin__Ad--=>");
        AdsManager.Instance.ShowInterstitialGamePlayAd(() =>
        {
         //   activePlayerVehicle.GetComponent<Rigidbody>().isKinematic = true;
            Player.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(GameWinDelay());
        });
        

    }

    IEnumerator GameWinDelay()
    {
        yield return new WaitForSeconds(1f);
       
        switch (activeMode)
        {
            case GameMode.offroad:
              //  Debug.Log("ActiveLevel--=>>" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) == 15)
                {
                    NextLevelButton.interactable = false;
                    PanelHandler(preCompletePanel, completePanel);
                }
                else
                {
                    NextLevelButton.interactable = true;
                    PanelHandler(preCompletePanel, completePanel);
                }
                PanelHandler(preCompletePanel, completePanel);
         //       AdsManager.Instance.ShowLargeBanner();
           //     activePlayerVehicle.GetComponent<Rigidbody>().isKinematic = true;
                Player.GetComponent<Rigidbody>().isKinematic = true;
                PlayerPrefs.SetInt("PlayedLevel" + (PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) - 1).ToString(), 1);
                PlayerPrefs.SetInt("RecentOffroadLevelPlayed", PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) <= PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) && PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) < 15)
                {
                    PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) + 1);
                }
                timeReward = (int)currentTime * 2;
                totalReward = timeReward + 500;
                coinRewardText.GetComponent<NumberCounter>().Value = 500;
                timeRewardText.GetComponent<NumberCounter>().Value = timeReward;
                totalRewardText.GetComponent<NumberCounter>().Value = totalReward;
                PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) + totalReward);

                Firebase.Analytics.FirebaseAnalytics.LogEvent("OffRoad_Level_Complete" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                break;
            case GameMode.parking:
                //Debug.Log("ActiveLevel--=>>" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) == 25)
                {
                    NextLevelButton.interactable = false;
                    PanelHandler(preCompletePanel, completePanel);
                }
                else
                {
                    NextLevelButton.interactable = true;
                    PanelHandler(preCompletePanel, completePanel);
                }
            //    AdsManager.Instance.ShowLargeBanner();
             //   activePlayerVehicle.GetComponent<Rigidbody>().isKinematic = true;
                Player.GetComponent<Rigidbody>().isKinematic = true;
                PlayerPrefs.SetInt("PlayedLevelParking" + (PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) - 1).ToString() , 1);
                PlayerPrefs.SetInt("RecentParkingLevelPlayed", PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) <= PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) && PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel) < 25)
                {
                    PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlockedPakring, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) + 1);
                }
                int hitCheck = hits switch
                {
                    0 => 3,
                    1 => 2,
                    2 => 1,
                    3 => 0,
                    _ => throw new NotImplementedException(),
                };
                hitReward = hitCheck;
                hitReward = hitCheck * 25;
                totalReward = hitReward + 500;
                coinRewardText.GetComponent<NumberCounter>().Value = 500;
                timeRewardText.GetComponent<NumberCounter>().Value = hitReward;
                totalRewardText.GetComponent<NumberCounter>().Value = totalReward;
                PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) + totalReward);

                PlayerPrefs.SetInt(CustomPlayerPrefs.firstTime_User, 1);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("Parking_Level_Complete" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                break;
        }
        InGameReviewManager.instance.Launch(); 
        //PlayerPrefs.SetInt(CustomPlayerPrefs.firstTime_User, 1);
    }
    Coroutine textAniamtion;
    public void OnClick2xReward()
    {
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
     //   AdsManager.Instance.HideLargeBanner();
        AdsManager.Instance.ShowAdmobRewardedVideo(2);
    }

    public void StopAnimation()
    {
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
    }
    IEnumerator AnimateText()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        adsStatusText.text = "please Wait.";
        yield return new WaitForSecondsRealtime(0.5f);
        adsStatusText.text = "please Wait..";
        yield return new WaitForSecondsRealtime(0.5f);
        adsStatusText.text = "please Wait...";
        textAniamtion = StartCoroutine(AnimateText());

    }


    public void Manage2XReward()
    {
        doubleRewardButton.SetActive(false);
        PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) + 500);
        coinRewardText.GetComponent<NumberCounter>().Value = 1000;
        totalReward += 500;
        totalRewardText.GetComponent<NumberCounter>().Value = totalReward;
    }
    #endregion

    #region GameFail
    void ReviveFail()
    { 
        isRevive = true;
        VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(0.0001f) * 20); // Convert to dB
    //    activePlayerVehicle.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetComponent<Rigidbody>().isKinematic = true;
        PanelHandler(hudPanel, revivePanel);
    }
    void GameFail()
    {      
        //CameraMouseDrag cam = playerCamera.GetComponentInChildren<CameraMouseDrag>();
        //cam.verticalMinAngle = 90;
        //cam.verticalMaxAngle = 90;
        //PanelHandler(hudPanel, preFailPanel);
    //    activePlayerVehicle.soundManager.masterVolume = 0;
   //     activePlayerVehicle.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetComponent<Rigidbody>().isKinematic = true;
        GameFailedDelay();
    }
    private void GameFailedDelay()
    {
        //    AdsManager.Instance.HideBanner();
        AdsManager.Instance.ShowInterstitialGamePlayAd(() =>
        {
       //     activePlayerVehicle.vehicleRigidbody.isKinematic = true;
            Player.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(GameFailDelayCo());

        });
    }
    IEnumerator GameFailDelayCo()
    {
        yield return new WaitForSeconds(1);
        switch (activeMode)
        {
            case GameMode.offroad:
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) >= 15)
                {
                    levelSkipButton.SetActive(false);
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("OffRoad_Level_Fail" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                }
                break;
            case GameMode.parking:
                if (PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) >= 25)
                {
                    levelSkipButton.SetActive(false);
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Parking_Level_Fail" + PlayerPrefs.GetInt(CustomPlayerPrefs.activeLevel));
                }
                break;
        }
        hudPanel.SetActive(false);
        PanelHandler(preFailPanel, failPanel);
        yield return null;
        //   AdsManager.Instance.ShowLargeBanner();
    }
    public void OnClickSkiplevel()
    {
        noAdsPanel.SetActive(true);
        if (textAniamtion != null)
            StopCoroutine(textAniamtion);
        textAniamtion = StartCoroutine(AnimateText());
        AdsManager.Instance.ShowAdmobRewardedVideo(3);
    }
    public void LevelSkipReward()
    {
     //   AdsManager.Instance.HideLargeBanner();
        Time.timeScale = 1;
        MenuManager.isLevel = true;
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region Sound_Music
    void ManageSound_Music()
    {
        //switch (PlayerPrefs.GetInt(CustomPlayerPrefs.soundToggle))
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
        //        break;o
        //}
        sfxAudioSource.volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
        musicAudioSource.volume = Mathf.Clamp(PlayerPrefs.GetFloat(CustomPlayerPrefs.musicToggle), 0, 0.25f);

        VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle)) * 20);
        Star_1.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);
        Star_2.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(CustomPlayerPrefs.soundToggle);

    }
    public void OnSfxToggleValueChanged()
    {
        //switch (sfxToggle.isOn)
        //{
        //    case true:
        //        PlayerPrefs.SetInt(CustomPlayerPrefs.soundToggle, 1);
        //        sfxToggle.isOn = true;
        //        sfxAudioSource.mute = false;
        //        sfxAudioSource.Play();
        //        break;
        //    case false:
        //        PlayerPrefs.SetInt(CustomPlayerPrefs.soundToggle, 0);
        //        sfxAudioSource.mute = true;
        //        sfxAudioSource.Stop();
        //        break;
        //}
      
        PlayerPrefs.SetFloat(CustomPlayerPrefs.soundToggle, sfxToggle.value);
        sfxAudioSource.volume = sfxToggle.value;
        VehicleAudio.SetFloat("RCCVolume", Mathf.Log10(sfxAudioSource.volume) * 20);

      /*  if (sfxToggle.value < 0.2)
        {
            Star_1.SetActive(false);
            Star_2.SetActive(false);


            //Star_1.mute = true;
            //Star_2.mute = true;
        }
        else
        {
            Star_1.SetActive(true);
            Star_2.SetActive(true);
            Star_1.GetComponent<AudioSource>().volume = sfxToggle.value;
            Star_2.GetComponent<AudioSource>().volume = sfxToggle.value;
            //Star_1.mute = false;
            //Star_2.mute = false;
        }*/
    }
    public void OnMusicToggleValueChanged()
    {
        //switch (musicToggle.isOn)
        //{
        //    case true:
        //        PlayerPrefs.SetInt(CustomPlayerPrefs.musicToggle, 1);
        //        musicToggle.isOn = true;
        //        musicAudioSource.mute = false;
        //        activePlayerVehicle.soundManager.masterVolume = 1;
        //        musicAudioSource.Play();
        //        break;
        //    case false:
        //        PlayerPrefs.SetInt(CustomPlayerPrefs.musicToggle, 0);
        //        musicAudioSource.mute = true;
        //        activePlayerVehicle.soundManager.masterVolume = 0;
        //        musicAudioSource.Stop();
        //        break;
        //}
        PlayerPrefs.SetFloat(CustomPlayerPrefs.musicToggle, musicToggle.value);
        musicAudioSource.volume = Mathf.Clamp(PlayerPrefs.GetFloat(CustomPlayerPrefs.musicToggle), 0, 0.25f);
    }
    #endregion

    #region ParkingMode
    public void TopDownCamera()
    {
        GameFail();
    }
    public void ManageHit(PropBlink instance)
    {
        if(canHit)
        {
            hits++;
            canHit = false;
        }
        if (hits >= 3)
        {
            instance.StartBlinking();
         //   activePlayerVehicle.GetComponent<Rigidbody>().isKinematic = true;
            Player.GetComponent<Rigidbody>().isKinematic = true;
            DetectCollision = false;
            if (revived)
                TopDownCamera();
            else
                ReviveFail();
        }
        timerText.text = HitsStatus();
    }
    string  HitsStatus()
    {
        int hitStatus = hits switch
        {
            0 => 3,
            1 => 2,
            2 => 1,
            3 => 0,
            _ => 0,
        };
        return hitStatus.ToString();
    }
    public void HitExit()
    {
        canHit = true;
    }
    #endregion
}
[Serializable]
public class LevelDetails
{
    public string _Level;
    public GameObject levelObject;
    public string levelDescription;
    public int levelTime;
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Waypoint[] waypoints;
}
