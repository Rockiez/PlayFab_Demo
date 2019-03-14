using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class GameInfo {

	public string[] gunNames;

    public static List<CatalogItem> catalogItems;

    public static string[] levelRankNames;
    public static int[] levelExps;

    public static Dictionary<string, string> titleData;
}
