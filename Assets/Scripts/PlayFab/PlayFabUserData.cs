using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;

public class PlayFabUserData : MonoBehaviour {

	public static string equipedWeapon="nor";
	public static string catalogVersion = "Weapon";

	public static string username ="";
    public static string email = "";

    public static Dictionary<string, object> userEntityData;
    public static Dictionary<string, bool> userReadonlyData;

    public static int goldCurrencyCount;
    public static int diamondCurrencyCount;

    public static int lv = 0;
    public static int exp = 0;

	public static int totalKill = 0;
	public static int totalDeath = 0;
	public static int killPerDeath = 0;

	public static int totalWin = 0;

}
