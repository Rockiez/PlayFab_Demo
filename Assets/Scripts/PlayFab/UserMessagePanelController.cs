using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class UserMessagePanelController : MonoBehaviour {

    public GameObject userMessagePanel;

    public Text userId;
    public Text currentRank;
    public Text nextRank;

    public Button backButton;


    public Slider expSlider;
    public Text expText;

    public Text totalWin;
    public Text totalKill;
    public Text killPerDeath;

    public Text modifyPasswordLabel;
    public GameObject processingWindow;

    void OnEnable () {
        modifyPasswordLabel.text = "";
        processingWindow.SetActive(false);

        userId.text = "Player ID："+PlayFabAuthService.PlayFabId;    

        currentRank.text = PlayFabUserData.lv.ToString();
        nextRank.text = (PlayFabUserData.lv + 1).ToString();


        expSlider.minValue = 0;
        expSlider.maxValue = 1000;
        expSlider.value = PlayFabUserData.exp;
        expText.text = PlayFabUserData.exp.ToString() + "/ 1000" ;

        totalWin.text = "Total Win：" + PlayFabUserData.totalWin.ToString();
        totalKill.text = "Total Kill：" + PlayFabUserData.totalKill.ToString();
        killPerDeath.text = "Kill Per Death：" + PlayFabUserData.killPerDeath.ToString();
    }

    public void ClickModifyPasswordButton()
    {
        if (PlayFabUserData.email == null)
        {
            modifyPasswordLabel.text = "Password can't be changed if the account isn't bound to the mailbox,";
            return;
        }
        processingWindow.SetActive(true);

        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = PlayFabUserData.email
        };
        Debug.Log(PlayFabUserData.email);
        PlayFabClientAPI.SendAccountRecoveryEmail(request,OnSendAccountRecoveryEmail,OnPlayFabError);
    }

    void OnSendAccountRecoveryEmail(SendAccountRecoveryEmailResult result)
    {
        processingWindow.SetActive(false);
        modifyPasswordLabel.text = "The Email used to change the password has been sent to the account mailbox.";
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);
    }

    public void ClickLogoutButton()
    {
        Photon.Pun.PhotonNetwork.Disconnect();
        PlayFabAuthenticationAPI.ForgetAllCredentials();
        backButton.onClick.Invoke();

}
}
