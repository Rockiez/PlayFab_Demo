using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Text.RegularExpressions;
public class IAPManager : MonoBehaviour {

    public Text diamondCurrencyCount;



    public void OnError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
    }


    public static void PurchaseDiamond(int num,Action<string>successCallBack,Action<PlayFabError>errorCallBack = null)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "PurchaseDiamond", 
            FunctionParameter = num,
            GeneratePlayStreamEvent = true, 
        };
        PlayFabClientAPI.ExecuteCloudScript(
			request,       
			(result) =>{
            	JsonObject jsonResult = (JsonObject)result.FunctionResult;
				object sNewDm;
            	jsonResult.TryGetValue("diamondCurrencyResult", out sNewDm);
                successCallBack?.Invoke(sNewDm.ToString());
			}, 
			(error) =>{
                	errorCallBack(error);
        	}
		);
        return ;
    }

    public void OnGetDiamondSuccess(string newDiamond)
    {
        diamondCurrencyCount.text = newDiamond.ToString();
        PlayFabUserData.diamondCurrencyCount = int.Parse(newDiamond);
    }

    public void Click300DiaButton()
    {
        PurchaseDiamond(300, OnGetDiamondSuccess, OnError);

    }
    public void Click680DiaButton()
    {
        PurchaseDiamond(680, OnGetDiamondSuccess, OnError);

    }
    public void Click1280DiaButton()
    {
        PurchaseDiamond(1280, OnGetDiamondSuccess, OnError);

    }
    public void Click3280DiaButton()
    {
        PurchaseDiamond(3280, OnGetDiamondSuccess, OnError);

    }
}
