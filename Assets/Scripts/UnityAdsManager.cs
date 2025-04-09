using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class UnityAdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
	public static UnityAdsManager Instance;

    [SerializeField]
    private string gameID = "18658";
    public string videoPlacement = "video";
    public string rewardedVideoPlacement = "rewardedVideo";

    [HideInInspector] public bool isNonRewardedLoaded = false;
    [HideInInspector] public bool isRewardedLoaded = false;

    [SerializeField]
	private bool enableTestMode;

	private void Awake()
	{
		if(Instance != null)
		{
			Destroy (this.gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad (this.gameObject);
		}
	}

	private void Start ()
	{
        if (Advertisement.isSupported /*&& !PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1)*/)
        {
            Debug.Log("Advertisment status is " + Advertisement.isSupported);
            Advertisement.Initialize(gameID, enableTestMode, this);
            Debug.Log("Unity ads init");
        }
        else
        {
            Debug.Log("Advertisment status is " + Advertisement.isSupported);
        }
	}

	public void ShowUnityVideoAd()
	{
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;
        StartCoroutine(DelayShowUnityAd());
    }
    IEnumerator DelayShowUnityAd()
    {
        yield return new WaitForSeconds(0.5f);
        Advertisement.Show(videoPlacement, this);
    }
    int rewardID;
    public void ShowUnityRewardedVideoAd(int id)
	{
        Advertisement.Show(rewardedVideoPlacement, this);
    }

    public void LoadUnityRewardedAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;
        Advertisement.Load(rewardedVideoPlacement, this);
    }

    public void ShowUnityRewardedAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;
        Advertisement.Show(rewardedVideoPlacement, this);
    }

    public void LoadNonRewardedAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;
        Advertisement.Load(videoPlacement, this);
    }

    public void ShowNonRewardedAd()
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;
        Advertisement.Show(videoPlacement, this);
    }

    public void OnInitializationComplete()
    {
        LoadNonRewardedAd();
        //LoadUnityRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {

    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds).Equals(1))
            return;

        if (placementId == videoPlacement)
            isNonRewardedLoaded = true;
        if (placementId == rewardedVideoPlacement)
            isRewardedLoaded = true;
        Debug.Log("unity ads Loaded");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Ads Failed to load " + error);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == videoPlacement)
        {
            isNonRewardedLoaded = false;
            LoadNonRewardedAd();
        }
        if (placementId == rewardedVideoPlacement)
        {
            isRewardedLoaded = false;
            //LoadUnityRewardedAd();
        }
    }
}