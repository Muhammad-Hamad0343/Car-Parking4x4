using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
//using Firebase.RemoteConfig;

public class AnalyticsManager : MonoBehaviour
{
    [HideInInspector] public bool firebaseInitialized = false;
    private DependencyStatus dependencyStatus;

    public bool App_Open_Eenabled;
    public bool Banner_Enabled;
    public bool Interstitial_Enabled;
    public bool Interstitial_GamePlay_Enabled;
    public bool Rewarded_Enabled;
    public bool Rewarded_Int_Enabled;
    public bool Rect_Banner_Enabled;

    #region Singleton
    public static AnalyticsManager Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        try
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    //InitializeFirebase();
                }
            });
        }
        catch
        {
            Debug.Log("Error in Firebase Setup");
        }

        CheckRemoteConfigValues();
    }
  
    public Task CheckRemoteConfigValues()
    {
        Debug.Log("Fetching data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task => {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");


                //Debug.Log("Total Values fetched: " + remoteConfig.AllValues.Count);


                App_Open_Eenabled = remoteConfig.GetValue("app_open_enabled").BooleanValue;
                Banner_Enabled = remoteConfig.GetValue("banner_enabled").BooleanValue;
                Interstitial_Enabled = remoteConfig.GetValue("interstitial_enabled").BooleanValue;
                Interstitial_GamePlay_Enabled = remoteConfig.GetValue("interstitial_GamePlay_enabled").BooleanValue;
                Rewarded_Enabled = remoteConfig.GetValue("rewarded_enabled").BooleanValue;
                Rewarded_Int_Enabled = remoteConfig.GetValue("rewarded_int_enabled").BooleanValue;
                Rect_Banner_Enabled = remoteConfig.GetValue("rect_banner_enabled").BooleanValue;


                Debug.Log(remoteConfig.GetValue("app_open_enabled").BooleanValue + " App_Open_Eenabled");
                Debug.Log(remoteConfig.GetValue("banner_enabled").BooleanValue + " Banner_Enabled");
                Debug.Log(remoteConfig.GetValue("interstitial_enabled").BooleanValue + " Interstitial_Enabled");
                Debug.Log(remoteConfig.GetValue("interstitial_GamePlay_enabled").BooleanValue + " InterstitialGamePlay_Enabled");
                Debug.Log(remoteConfig.GetValue("rewarded_enabled").BooleanValue + " Rewarded_Enabled");
                Debug.Log(remoteConfig.GetValue("rewarded_int_enabled").BooleanValue + " Rewarded_Int_Enabled");
                Debug.Log(remoteConfig.GetValue("rect_banner_enabled").BooleanValue + " Rect_Banner_Enabled");

                /*print("Total values: "+remoteConfig.AllValues.Count);

                foreach (var item in remoteConfig.AllValues)
                {
                    print("Key :" + item.Key);
                    print("Value: " + item.Value.StringValue);
                }*/

            });
    }



    private async void InitializeFirebase()
    {
        //FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        //FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        //firebaseInitialized = true;

        ////Remote Config
        //System.Collections.Generic.Dictionary<string, object> defaults =
        //    new System.Collections.Generic.Dictionary<string, object>();

        //// These are the values that are used if we haven't fetched data from the
        //// server
        //// yet, or if we ask for values that the server doesn't have:
        //defaults.Add("config_test_string", "default local string");
        //defaults.Add("config_test_int", 1);
        //defaults.Add("config_test_float", 1.0);
        //defaults.Add("config_test_bool", false);

        //FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        //await FetchDataAsync();
        //Debug.Log("Remote config ready!");
    }



    #region Remote Config
    //Start a fetch request.
    //public Task FetchDataAsync()
    //{
    //    Debug.Log("Fetching data...");
    //    Task fetchTask =
    //    FirebaseRemoteConfig.DefaultInstance.FetchAsync(
    //        TimeSpan.Zero);
    //    return fetchTask.ContinueWithOnMainThread(FetchComplete);
    //}

    //private void FetchComplete(Task fetchTask)
    //{
    //    if (!fetchTask.IsCompleted)
    //    {
    //        return;
    //    }

    //    var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
    //    var info = remoteConfig.Info;
    //    if (info.LastFetchStatus != LastFetchStatus.Success)
    //    {
    //        //Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
    //        return;
    //    }

    //    // Fetch successful. Parameter values must be activated to use.
    //    remoteConfig.ActivateAsync()
    //      .ContinueWithOnMainThread(
    //        task =>
    //        {
    //            Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
    //            Debug.Log("Valued Fetched");
    //            Debug.Log("Splash ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("splashLoadingAppOpen").BooleanValue);
    //            Debug.Log("unlock All game ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("unlockAllGameAD").BooleanValue);
    //            Debug.Log("unlock level locks ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("unlockAllLevelsAD").BooleanValue);
    //            Debug.Log("level loading ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("loadingLevelAD").BooleanValue);
    //            Debug.Log("game pause ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("gamePauseAD").BooleanValue);
    //            Debug.Log("game fail ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("gameFailAD").BooleanValue);
    //            Debug.Log("game complete ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("gameCompleteAD").BooleanValue);
    //            Debug.Log("game quit ad" + FirebaseRemoteConfig.DefaultInstance.GetValue("gameQuitAD").BooleanValue);

    //            //Debug.Log("ShowAD: " + PlayerPrefs.GetInt("ShowAD"));
    //        });
    //}
    #endregion
}
