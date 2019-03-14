using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
public class BuyGoldPageController : MonoBehaviour {
    
    public GameObject processPanel;
    public Text hintLabel;
    public GameObject backButton;
    public Text singlePaymentDiamondCost;
    public Text singlePaymentGoldGet;
    public Text multiplePaymentGoldGet;

    public Text goldCurrencyCount;
    public Text diamondCurrencyCount;

	public void ClickSingleBuyGoldButton()
	{
        if (PlayFabUserData.diamondCurrencyCount>4)
        {
            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest()
                {
                    FunctionName = "ExchangeGold",
                    FunctionParameter = "",
                    GeneratePlayStreamEvent = true
                },
                OnExecuteCloudScript,
                OnPlayFabError);
        }

    }


    void OnExecuteCloudScript(ExecuteCloudScriptResult result)
    {
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object diamondCurrencyResult, goldCurrencyResult;
        jsonResult.TryGetValue("diamondCurrencyResult", out diamondCurrencyResult);
        jsonResult.TryGetValue("goldCurrencyResult", out goldCurrencyResult);

        diamondCurrencyCount.text = diamondCurrencyResult.ToString();
        goldCurrencyCount.text = goldCurrencyResult.ToString();

        hintLabel.text = "Success";
        backButton.SetActive(true);
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError(error.Error);

        hintLabel.text = "Fail";
		backButton.SetActive(true);
    }
}
