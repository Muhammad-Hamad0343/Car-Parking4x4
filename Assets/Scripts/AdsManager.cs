using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using UnityEngine.Advertisements;
using System;
using GoogleMobileAds.Ump.Api;

using UnityEngine.SceneManagement;
using UnityEngine.Events;
using GoogleMobileAds.Common;
using Unity.VisualScripting;

public enum RewardedInterstitialType { Coins, Cars, }

public class AdsManager : MonoBehaviour
{
    #region Singleton
    public static AdsManager Instance;
    public void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    ConsentForm consentForm;

    public bool enableTestMode = true;

    [Header("UI")]
    public GameObject loadingADCanvas;
    public GameObject ProtectiveLayer;
    [Header("Banner PlaceHolder")]
    public GameObject bannerPlaceholder;
    [Header("App ID")]
    public string appID = "ca-app-pub-3940256099942544~3347511713";

    [Header("Admob Ads")]
    public string appOpenID = "ca-app-pub-3940256099942544/9257395921";
    [Space(5)]
    public string bannerID = "ca-app-pub-3940256099942544/6300978111";
    public AdPosition smallBannerPosition = AdPosition.TopRight;
    [Space(5)]
    public string largeBannerID = "ca-app-pub-3940256099942544/6300978111";
    public AdPosition largeBannerPosition = AdPosition.BottomRight;
    //public AdPosition LargeBannerPosition = AdPosition.BottomLeft;
    [Space(5)]
    public string interstitialID = "ca-app-pub-3940256099942544/1033173712";
    [Space(5)]
    public string interstitialGameplayID = "ca-app-pub-3940256099942544/1033173712";
    [Space(5)]
    public string rewardedID = "ca-app-pub-3940256099942544/5224354917";
    [Space(5)]
    public string rewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";

    [HideInInspector] public BannerView bannerView, largeBannerView;
    [HideInInspector] public InterstitialAd interstitialAd, interstitialGamePlayAd;
    [HideInInspector] public RewardedAd rewardedAd;
    [HideInInspector] public AppOpenAd appOpen;
    [HideInInspector] public RewardedInterstitialAd rewardInterstitial;

    [HideInInspector] public bool isBannerLoaded = false;
    [HideInInspector] public bool isLargeBannerLoaded = false;

    [HideInInspector] public bool isNonRewardedLoaded = false;
    [HideInInspector] public bool isRewardedLoaded = false;

    [HideInInspector] public bool doubleRewardButtonPressed = false;
    [HideInInspector] public bool freeCashButtonPressed = false;
    [HideInInspector] public bool modeUnlockButtonPressed = false;
    [HideInInspector] public int carIndex = 0;
    private int rewardID;

    public GameObject AdLoadedStatus;

    //Banner CallBacks
    public event EventHandler<EventArgs> onBannerAdLoaded;
    public event EventHandler<EventArgs> onBannerAdClosed;
    public event EventHandler<LoadAdError> onBannerAdFailedToLoad;

    //Rewarded Callbacks
    public event EventHandler<EventArgs> onAdMobRewardedClosed;
    public event EventHandler<EventArgs> onAdMobRewardedLoaded;
    public event EventHandler<EventArgs> onAdMobRewardedOpen;
    public event EventHandler<LoadAdError> onAdMobRewardedFailedToLoad;
    public event EventHandler<Reward> onAdMobRewardedUserReward;

    public UnityAction RewardedAdFailedOnLevel;
    public UnityAction RewardedAdFailedOnDismissRewardedAd;
       
    private void Start()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
        {
            SplashManager.SplashStatus();
            return;
        }
       // CoroutineRunner.Instance.StartCoroutine(InitializeAds());        
    }
    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
          //  Debug.Log("No Internet Connection!");
            //  SceneManager.LoadScene("mainmenu");
        }
        else
        {
            if(PlayerPrefs.GetInt("InitializeAds") == 0)
            {               
                CoroutineRunner.Instance.StartCoroutine(InitializeAds());
                if(SceneManager.GetActiveScene().buildIndex != 0)
                {
                    MenuManager.instance.BannerAds = true;
                   // BanneradsFirsttime = true;
                }
                PlayerPrefs.SetInt("InitializeAds", 1);
            }           
        }
      
    }
    IEnumerator InitializeAds()
    {
       
#if UNITY_ANDROID && !UNITY_EDITOR
        WebviewWarmer.PreWarming(); // Prewarming
        yield return new WaitUntil(() => WebviewWarmer.IsOpertaionCompleted);
#endif

        yield return null;

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        yield return new WaitForSeconds(0.5f);
       
        yield return null;

        if (!PlayerPrefs.GetInt("consentShown").Equals(1))
        {
            ConsentRequestParameters request = new ConsentRequestParameters();
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }
        else
        {
            if (enableTestMode)
            {
                bannerID = largeBannerID = "ca-app-pub-3940256099942544/6300978111";
                interstitialID = "ca-app-pub-3940256099942544/1033173712";
                interstitialGameplayID = "ca-app-pub-3940256099942544/1033173712";
                rewardedID = "ca-app-pub-3940256099942544/5224354917";
                appOpenID = "ca-app-pub-3940256099942544/9257395921";
                rewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
            }

            MobileAds.Initialize(HandleInitCompleteAction);

            SplashManager.SplashStatus();
        }
    }
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        //   Debug.Log("InitializationStatus-Ads ");
        //if(AnalyticsManager.Instance.App_Open_Eenabled)
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
          //  Debug.Log("App-Open-ads");
            RequestAppOpenAd();
        }
        //    if(AnalyticsManager.Instance.Banner_Enabled)
                LoadBannerAd();
         //   if(AnalyticsManager.Instance.Interstitial_Enabled)
                LoadInterstitialAd();
        //    if(AnalyticsManager.Instance.Rect_Banner_Enabled)
        //      LoadLargeBannerAd();

        LoadInterstitialGamePlayAd();
       
    }
 

    #region ConsentMessage
    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Consent gathering failed.
            UnityEngine.Debug.LogError(error);
            SplashManager.SplashStatus();
            return;
        }

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(formError);
                SplashManager.SplashStatus();
                return;
            }
            
            //Consent has been gathered.
            if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained && !PlayerPrefs.GetInt("consentShown").Equals(1))
            {
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    // TODO: Request an ad.
                    MobileAds.RaiseAdEventsOnUnityMainThread = true;
                    if (enableTestMode)
                    {
                        bannerID = largeBannerID = "ca-app-pub-3940256099942544/6300978111";
                        interstitialID = "ca-app-pub-3940256099942544/1033173712";
                        interstitialGameplayID = "ca-app-pub-3940256099942544/1033173712";
                        rewardedID = "ca-app-pub-3940256099942544/5224354917";
                        appOpenID = "ca-app-pub-3940256099942544/9257395921";
                        rewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
                    }

                    MobileAds.Initialize(initStatus => { });
                    //App Open Ad Call
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        RequestAppOpenAd();
                    }

                //    if (AnalyticsManager.Instance.Banner_Enabled)
                        LoadBannerAd();
                //    if (AnalyticsManager.Instance.Interstitial_Enabled)
                        LoadInterstitialAd();
                    //     if (AnalyticsManager.Instance.Rect_Banner_Enabled)
                    //      LoadLargeBannerAd();

                    LoadInterstitialGamePlayAd();
                    SplashManager.SplashStatus();

                    PlayerPrefs.SetInt("consentShown", 1);
                });
            }
            if(ConsentInformation.ConsentStatus == ConsentStatus.NotRequired)
            {
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    // TODO: Request an ad.
                    MobileAds.RaiseAdEventsOnUnityMainThread = true;
                    if (enableTestMode)
                    {
                        bannerID = largeBannerID = "ca-app-pub-3940256099942544/6300978111";
                        interstitialID = "ca-app-pub-3940256099942544/1033173712";
                        interstitialGameplayID = "ca-app-pub-3940256099942544/1033173712";
                        rewardedID = "ca-app-pub-3940256099942544/5224354917";
                        appOpenID = "ca-app-pub-3940256099942544/9257395921";
                        rewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
                    }

                    MobileAds.Initialize(initStatus => { });
                    //App Open Ad Call
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        RequestAppOpenAd();
                    }
               //     if (AnalyticsManager.Instance.Banner_Enabled)
                        LoadBannerAd();
                //    if (AnalyticsManager.Instance.Interstitial_Enabled)
                        LoadInterstitialAd();
                    //     if (AnalyticsManager.Instance.Rect_Banner_Enabled)
                    //       LoadLargeBannerAd();

                    LoadInterstitialGamePlayAd();
                    SplashManager.SplashStatus();

                    PlayerPrefs.SetInt("consentShown", 1);
                });
            }
        });
    }

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm _consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                // TODO: Request an ad.
                MobileAds.RaiseAdEventsOnUnityMainThread = true;
                if (enableTestMode)
                {
                    bannerID = largeBannerID = "ca-app-pub-3940256099942544/6300978111";
                    interstitialID = "ca-app-pub-3940256099942544/1033173712";
                    interstitialGameplayID = "ca-app-pub-3940256099942544/1033173712";
                    rewardedID = "ca-app-pub-3940256099942544/5224354917";
                    appOpenID = "ca-app-pub-3940256099942544/9257395921";
                    rewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
                }

                MobileAds.Initialize(initStatus => { });
                //App Open Ad Call
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    RequestAppOpenAd();
                }
                LoadBannerAd();
                LoadInterstitialAd();
                //    LoadLargeBannerAd();

                LoadInterstitialGamePlayAd();
                //SplashManager.SplashStatus();

                PlayerPrefs.SetInt("consentShown", 1);
            });
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        consentForm = _consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            consentForm.Show(OnShowForm);
            Debug.Log("Showing consent form ");
        }

    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }
        else
        {
        }

        PlayerPrefs.SetInt("consentShown", 0);

        // Handle dismissal by reloading form.
        LoadConsentForm();

    }
    #endregion

    #region Banner
    public void CreateBannerView()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (bannerView != null)
        {
            DestroyAd();
        }

        bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.TopRight); //Top
    }

    void CenterBanner()
    {
        // Get the screen width
        float screenWidth = Screen.width;

        //Get the banner width
        float bannerWidth = bannerView.GetWidthInPixels();

        // Calculate the position to center the banner
        float xPosition = (screenWidth - bannerWidth) / 2;

        // Set the banner position
        bannerView.SetPosition((int)xPosition,50 ); //0
    }

    public void LoadBannerAd()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

      //  Debug.Log("Load-Banner-Ad");
        if (bannerView == null)
        {
            CreateBannerView();
        }

        ListenToAdEvents();

        var adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
        bannerView.Hide();

        StartCoroutine(WaitaSecond());       
    }

    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () => {
                isBannerLoaded = true;
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) => {
                isBannerLoaded = false;
        };
    }

    public void DestroyAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    public void ShowBanner()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        //if(IsInvoking(nameof(HideBanner)))
        //    CancelInvoke(nameof(HideBanner));
        if (isBannerLoaded)
        {
          //  Debug.Log("Show Banner Call and isShowing");
            bannerPlaceholder.SetActive(true);
            bannerView.Show();
        }
        else
        {
       //    Debug.Log("Show Banner Call and Not Showing");
        }
    }

    public void HideBanner()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        //if(IsInvoking(nameof(ShowBanner)))
        //    CancelInvoke(nameof(ShowBanner));
        if (isBannerLoaded)
        {
          //  Debug.Log("Hide Banner Call and hiding");
            bannerPlaceholder.SetActive(false);
            bannerView.Hide();
        }
        else
        {
          //  Debug.Log("No Banner view to hide");
        }
    }

    IEnumerator WaitaSecond()
    {
        yield return new WaitForSeconds(10);
        PlayerPrefs.SetInt("BanneradsFirsttime", 1);
    }
    #endregion

    #region Large Banner
    public void CreateLargeBannerView()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (largeBannerView != null)
        {
            DestroyLargeBannerAd();
        }

        largeBannerView = new BannerView(largeBannerID, AdSize.MediumRectangle, largeBannerPosition);
    }

    public void LoadLargeBannerAd()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (largeBannerView == null)
        {
         //   CreateLargeBannerView();
        }

        ListenToLargeBannerAdEvents();

        var adRequest = new AdRequest();
        largeBannerView.LoadAd(adRequest);
        largeBannerView.Hide();
    }

    private void ListenToLargeBannerAdEvents()
    {
        //Raised when an ad is loaded into the banner view.
       largeBannerView.OnBannerAdLoaded += () => {
            isLargeBannerLoaded = true;
       };
        // Raised when an ad fails to load into the banner view.
        largeBannerView.OnBannerAdLoadFailed += (LoadAdError error) => {
                isBannerLoaded = false;
        };
    }

    public void DestroyLargeBannerAd()
    {
        if (largeBannerView != null)
        {
            largeBannerView.Destroy();
            largeBannerView = null;
        }
    }

    public void ShowLargeBanner()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        //if (IsInvoking(nameof(HideLargeBanner)))
        //    CancelInvoke(nameof(HideLargeBanner));
        if (isLargeBannerLoaded)
        {
            largeBannerView.Show();
        }
    }

    public void HideLargeBanner()
    {
        if(PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        //if (IsInvoking(nameof(ShowLargeBanner)))
        //    CancelInvoke(nameof(ShowLargeBanner));
        if (isLargeBannerLoaded)
        {
            largeBannerView.Hide();
        }
    }
    #endregion

    #region Interstitial

    private Action InterstitialCallBack;
    public void LoadInterstitialAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    return;
                }

                interstitialAd = ad;
                RegisterEventHandlers(interstitialAd);
            });
    }
    public void ShowInterstitialAd(Action callback = null)
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
        {
            callback();
            return;
        }

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            if(callback != null)
                InterstitialCallBack = callback;
            loadingADCanvas.gameObject.SetActive(true);
            StartCoroutine(ShowInterstitialAdNow());
        }
        else
        {
            callback();
        }
    }
    public void ShowAdmodElseUnity()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            loadingADCanvas.gameObject.SetActive(true);
            StartCoroutine(ShowInterstitialAdNow());
        }
    }

    private IEnumerator ShowInterstitialAdNow()
    {
        yield return new WaitForSecondsRealtime(1f);
        interstitialAd.Show();
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        //Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        { 
            loadingADCanvas.gameObject.SetActive(false);
                if (InterstitialCallBack != null)
                {
                    InterstitialCallBack();
                    InterstitialCallBack = null;
                }
                LoadInterstitialAd();
        };


        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            if (InterstitialCallBack != null)
                {
                    InterstitialCallBack();
                    InterstitialCallBack = null;
                }
                LoadInterstitialAd();
        };
    }
    #endregion

    #region Interstitial GamePlay

    private Action InterstitialGamePlayCallBack;
    public void LoadInterstitialGamePlayAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (interstitialGamePlayAd != null)
        {
            interstitialGamePlayAd.Destroy();
            interstitialGamePlayAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialGameplayID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    return;
                }

                interstitialGamePlayAd = ad;
                RegisterEventHandlersGamePlay(interstitialGamePlayAd);
            });
    }
    public void ShowInterstitialGamePlayAd(Action callback = null)
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
        {
            callback();
            return;
        }

        if (interstitialGamePlayAd != null && interstitialGamePlayAd.CanShowAd())
        {
            if (callback != null)
                InterstitialGamePlayCallBack = callback;
            loadingADCanvas.gameObject.SetActive(true);
            StartCoroutine(ShowInterstitialGamePlayAdNow());
        }
        else
        {
            callback();
        }
    }
    //public void ShowAdmodElseUnity()
    //{
    //    if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
    //        return;

    //    if (interstitialGamePlayAd != null && interstitialGamePlayAd.CanShowAd())
    //    {
    //        loadingADCanvas.gameObject.SetActive(true);
    //        StartCoroutine(ShowInterstitialAdNow());
    //    }
    //}

    private IEnumerator ShowInterstitialGamePlayAdNow()
    {
        yield return new WaitForSecondsRealtime(1f);
        interstitialGamePlayAd.Show();
    }

    private void RegisterEventHandlersGamePlay(InterstitialAd ad)
    {
        //Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            loadingADCanvas.gameObject.SetActive(false);
            if (InterstitialGamePlayCallBack != null)
            {
                InterstitialGamePlayCallBack();
                InterstitialGamePlayCallBack = null;
            }
            LoadInterstitialGamePlayAd();
        };


        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            if (InterstitialGamePlayCallBack != null)
            {
                InterstitialGamePlayCallBack();
                InterstitialGamePlayCallBack = null;
            }
            LoadInterstitialGamePlayAd();
        };
    }
    #endregion

    #region Rewarded

    private bool IsReviveAdClicked = false;
    private bool IsReviveAdComplete = false;
    
    public void LoadRewardedAd()
    {
        //if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
        //    return;

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(rewardedID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    //StartCoroutine(VehicleUnlockRewardDelay(id));
                    StartCoroutine(DelayMessage(rewardID));
                    return;
                }
                Debug.Log("rewardID---=>"+ rewardID);
                rewardedAd = ad;
                switch (rewardID)
                {                    
                    case 1:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 2:
                        GameManager.instance.NoAdsPanel();
                        break;
                    case 3:
                        GameManager.instance.NoAdsPanel();
                        break;
                    case 4:
                        GameManager.instance.NoAdsPanel();
                        break;
                    case 5:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 6:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 7:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 8:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 9:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 10:
                        MenuManager.instance.NoAdsPanel();
                        break; 
                    case 11:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 12:
                        MenuManager.instance.NoAdsPanel();
                        break;
                    case 13:
                        MenuManager.instance.NoAdsPanel();
                        break;
                }
                rewardedAd.Show((Reward reward)=>
                {
                    switch (rewardID)
                    {
                        case 1:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) + 100);
                            MenuManager.UpdateGameCash();
                            break;
                        case 2:
                            if (GameManager.instance != null)
                            {
                                GameManager.instance.Manage2XReward();
                            }
                            break;
                        case 3:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) + 1);
                            if (GameManager.instance != null)
                            {
                                GameManager.instance.LevelSkipReward();
                            }
                            break;
                        case 4:
                            IsReviveAdComplete = true;
                            //Debug.LogError("IsReviveAdComplete status is " + IsReviveAdComplete);
                            GameManager.instance.LevelRevive();
                            break;

                        case 5:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 1, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 6:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 2, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 7:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 3, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 8:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 4, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 9:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 5, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 10:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 6, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 11:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 7, 1);
                            MenuManager.instance.ChangeVehicle();
                            break;
                        case 12:
                            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 8, 1);
                            MenuManager.instance.ChangeVehicle();
                            break; 
                        case 13:                          
                            MenuManager.instance.UnLockLevel();
                            break;                            
                    }
                });
                RegisterEventHandlers(rewardedAd);
            });
    }

    public void ShowAdmobRewardedVideo(int id)
    {
        if (!AnalyticsManager.Instance.Rewarded_Enabled) return;

        rewardID = id;
        if(id == 4)
        {
            IsReviveAdComplete = false;
            IsReviveAdClicked = true;
          //  Debug.Log("Reward 4 Ad button Clicked");
        }
        LoadRewardedAd();
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {

        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            if(IsReviveAdClicked && !IsReviveAdComplete)
            {
                //RewardedAdFailedOnDismissRewardedAd?.Invoke();
                IsReviveAdClicked = false;
                IsReviveAdComplete = false;
            }
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
        };
    }
    IEnumerator DelayMessage(int id)
    {
        yield return new WaitForSeconds(0f);
        switch (id)
        {
            case 1:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 2:
                GameManager.instance.doubleRewardButton.SetActive(false);
                GameManager.instance.StopAnimation();
                GameManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                GameManager.instance.NoAdsPanel();
                break;
            case 3:
                GameManager.instance.levelSkipButton.SetActive(false);
                GameManager.instance.StopAnimation();
                GameManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                GameManager.instance.NoAdsPanel();
                break;
            case 4:
                GameManager.instance.levelSkipButton.SetActive(false);
                GameManager.instance.StopAnimation();
                GameManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                RewardedAdFailedOnLevel?.Invoke();
                GameManager.instance.NoAdsPanel();
                break;
            case 5:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 6:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 7:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 8:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 9:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 10:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 11:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break; 
            case 12:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                MenuManager.instance.NoAdsPanel();
                break;
            case 13:
                MenuManager.instance.StopAnimation();
                MenuManager.instance.FreeAdsPanel("No Ads Available");
                yield return new WaitForSeconds(0.5f);
                Debug.Log("Close panel i ");
                MenuManager.instance.NoAdsPanel();
                break;
        }
    }
    #endregion

    #region App Open
    private void RequestAppOpenAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
        {
            return;
        }

        AdRequest request = new AdRequest();
        // Clean up the old ad before loading a new one.
        if (appOpen != null)
        {
            appOpen.Destroy();
            appOpen = null;
        }

        Debug.Log("Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(appOpenID, request, ((appOpenAd, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || appOpenAd == null)
                {
                    SplashManager.SplashStatus();
                    Debug.LogError("app open ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }
                appOpen = appOpenAd;
                if (appOpen != null && appOpen.CanShowAd())
                {
                    loadingADCanvas.gameObject.SetActive(true);
                    StartCoroutine(ShowAppOpen());
                }
                else
                {
                    DestroyAppOpen();
                }
                RegisterEventHandlers(appOpenAd);
            }));
    }
    IEnumerator ShowAppOpen()
    {
        yield return new WaitForSeconds(0.75f);
        appOpen.Show();
        loadingADCanvas.gameObject.SetActive(false);
    }
    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
               SplashManager.SplashStatus();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
                SplashManager.SplashStatus();
        };
    }
    public void HideAppOpen()
    {

    }
    public void DestroyAppOpen()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (IsInvoking(nameof(ShowAppOpen)))
        {
            CancelInvoke(nameof(ShowAppOpen));
        }
        if (appOpen != null)
        {
            appOpen.Destroy();
        }
        appOpen = null;
    }
    #endregion

    #region RewardedInterstitial

    private int CarIndexToUnlock = 0;
    private Action RewardedInterstitialCallback;
    public void LoadRewardedInterstitialAd(RewardedInterstitialType type)
    {
        // Clean up the old ad before loading a new one.
        if (rewardInterstitial != null)
        {
            rewardInterstitial.Destroy();
            rewardInterstitial = null;
        }


        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("Reward");

        // send the request to load the ad.
        RewardedInterstitialAd.Load(rewardedInterstitialID, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    StartCoroutine(HandleRewardInterstitialDelayMessage());
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardInterstitial = ad;
                RegisterRewardInterstitialEvents(rewardInterstitial);
                rewardInterstitial.Show((Reward reward)=> 
                {
                    if (type == RewardedInterstitialType.Coins)
                    {
                        PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) + 1000);
                        MenuManager.UpdateGameCash();
                        PlayerPrefs.SetInt(CustomPlayerPrefs.freeCoinGiveAway, 1);
                        MenuManager.instance.NoAdsPanel();
                    }
                    else if (type == RewardedInterstitialType.Cars)
                    {
                        switch (CarIndexToUnlock)
                        {
                            case 2:
                                PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 2, 1);
                                MenuManager.instance.ChangeVehicle();
                                if(RewardedInterstitialCallback != null)
                                {
                                    RewardedInterstitialCallback();
                                    RewardedInterstitialCallback = null;
                                }
                                break;
                            case 3:
                                PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 3, 1);
                                MenuManager.instance.ChangeVehicle();
                                if (RewardedInterstitialCallback != null)
                                {
                                    RewardedInterstitialCallback();
                                    RewardedInterstitialCallback = null;
                                }
                                break;
                            case 4:
                                PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 4, 1);
                                MenuManager.instance.ChangeVehicle();
                                if (RewardedInterstitialCallback != null)
                                {
                                    RewardedInterstitialCallback();
                                    RewardedInterstitialCallback = null;
                                }
                                break;
                        }
                        MenuManager.instance.NoAdsPanel();
                    }
                });
            });
    }
    void RegisterRewardInterstitialEvents(RewardedInterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            MenuManager.instance.NoAdsPanel();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
        };
    }

    public void ShowRewardInterstitialAdDelay(RewardedInterstitialType type, int carIndex = 0, Action Callback = null)
    {
        if (!AnalyticsManager.Instance.Rewarded_Int_Enabled) return;

        if(carIndex != 0)
        {
            CarIndexToUnlock = carIndex;
        }
        if (Callback != null)
        {
            RewardedInterstitialCallback = Callback;
        }
        LoadRewardedInterstitialAd(type);
    }
    IEnumerator HandleRewardInterstitialDelayMessage()
    {
        MenuManager.instance.StopAnimation();
        MenuManager.instance.FreeAdsPanel("No Ads Available");
        yield return new WaitForSeconds(0.5f);
        MenuManager.instance.NoAdsPanel();
    }

    //void ShowRewardInterstitialAd()
    //{
    //    if (rewardInterstitial != null && rewardInterstitial.CanShowAd()) 
    //    {
    //        rewardInterstitial.Show((Reward reward) =>
    //        {
    //            PlayerPrefs.SetInt(CustomPlayerPrefs.gameCoins, PlayerPrefs.GetInt(CustomPlayerPrefs.gameCoins) + 1000);
    //            MenuManager.UpdateGameCash();
    //            PlayerPrefs.SetInt(CustomPlayerPrefs.freeCoinGiveAway, 1);
    //            MenuManager.instance.NoAdsPanel();
    //        });
    //    }
    //}
    #endregion
}
