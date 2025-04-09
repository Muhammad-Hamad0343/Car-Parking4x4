using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlayerPrefs : MonoBehaviour
{
    public static string gameInit { get { return "gameInit"; } }
    public static string gameCoins { get { return "gameCoins"; } }
    public static string vehicleUnlock { get { return "vehicleUnlock"; } }
    public static string activeVehicle { get { return "activeVehicle"; } }
    public static string playerAvatar { get { return "playerAvatar"; } }
    public static string controlType { get { return "controlType"; } }
    public static string soundToggle { get { return "soundToggle"; } }
    public static string musicToggle { get { return "musicToggle"; } }
    public static string activeGameMode { get { return "activeGameMode"; } }
    public static string levelUnlocked { get { return "levelUnlocked"; } }
    public static string levelUnlockedPakring { get { return "levelUnlockedPakring"; } }
    public static string activeLevel { get { return "activeLevel"; } }
    public static string removeAds { get { return "removeAds"; } }
    public static string allGame_IAP { get { return "allGame_IAP"; } }
    public static string allLevel_IAP { get { return "allLevel_IAP"; } }
    public static string removeAds_IAP { get { return "removeAds_IAP"; } }
    public static string freeCoinGiveAway { get { return "freeCoinGiveAway"; } }
    public static string firstTime_User { get { return "firstTime"; } }
}
