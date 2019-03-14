using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class LoginPanelController : MonoBehaviourPunCallbacks, IConnectionCallbacks
{

	public GameObject loginPanel;		
	public GameObject userMessage;		
	public Button backButton;
    //public GameObject lobbyPanel;
    public GameObject mainPanel;

    public GameObject roomPanel;		
	public Text connectionState;

    public InputField Username;
    public InputField Password;

    private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;


    void Start() {
        if (!(PhotonNetwork.IsConnected)) {
			SetLoginPanelActive ();	

		} 
		else
			SetMainPanelActive ();
		connectionState.text = "";
        if (PhotonNetwork.InRoom)
        {
            mainPanel.SetActive(false);
            roomPanel.SetActive(true);
        }

        PlayFabAuthService.OnLoginSuccess += RequestPhotonToken;
        PlayFabAuthService.OnPlayFabError += OnPlayFabError;
        PlayFabAuthService.OnLogMessage += LogMessage;
    }



	void Update(){		
        connectionState.text = PhotonNetwork.NetworkClientState.ToString ();
	}

	public void SetLoginPanelActive(){
		loginPanel.SetActive (true);			
		backButton.gameObject.SetActive (false);
        mainPanel.SetActive (false);				
		if(roomPanel!=null)
			roomPanel.SetActive (false);			
	}
	public void SetMainPanelActive(){				
		loginPanel.SetActive (false);			
		backButton.gameObject.SetActive (true);
        mainPanel.SetActive (true);				
	}

	public void ClickLogInButton(){							
        _AuthService.Email = Username.text;
        _AuthService.Password = Password.text;
        _AuthService.AuthenticateEmailPassword();

        PhotonNetwork.GameVersion = "1.0";


    }

    public void ClickGuestButton()
    {
        PhotonNetwork.GameVersion = "1.0";
        _AuthService.SilentlyAuthenticate();
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }


    public override void OnJoinedLobby()
    {
        userMessage.GetComponentInChildren<Text>().text
                   = "Welcome，" + PhotonNetwork.LocalPlayer.NickName;
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        SetLoginPanelActive();
    }

    private void RequestPhotonToken()
    {
        LogMessage("PlayFab authenticated. Requesting photon token...");

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, AuthenticateWithPhoton, OnPlayFabError);
    }


    private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
    {
        LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", PlayFabAuthService.PlayFabId);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

        //We finally tell Photon to use this authentication parameters throughout the entire application.
        PhotonNetwork.AuthValues = customAuth;

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.LocalPlayer.NickName = (PlayFabAuthService.PlayFabId.Split(new Char[] { '-' }))[0];
        SetMainPanelActive();

    }



    private void OnPlayFabError(PlayFabError obj)
    {
        LogMessage(obj.ErrorMessage);
    }
    public void LogMessage(string message)
    {
        userMessage.GetComponentInChildren<Text>().text
           = message;
        Debug.Log("PlayFab : " + message);
    }
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        LogMessage(debugMessage);
    }
}
