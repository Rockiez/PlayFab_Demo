using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomPanelController : MonoBehaviourPunCallbacks{

	public GameObject lobbyPanel;
	public GameObject roomPanel;
	public Button backButton;
	public Text roomName;				
	public GameObject[] Column1;		
	public GameObject[] Column2;		
	public Button readyButton;		
	public Text promptMessage;		

	int teamSize;
	Text[] texts;
    Image image;
    Color readyColor = new Color32(4,185,127,255);
    Color unReadyColor = new Color32(255, 105, 81, 255);

    ExitGames.Client.Photon.Hashtable costomProperties;

    public override void OnEnable()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;

        if (!(PhotonNetwork.IsConnected)) return;

        roomName.text = PhotonNetwork.CurrentRoom.Name.ToUpper();
        promptMessage.text = "";

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate ()
        {
            PhotonNetwork.LeaveRoom();
            lobbyPanel.SetActive(true);
            roomPanel.SetActive(false);
        });

        teamSize = PhotonNetwork.CurrentRoom.MaxPlayers / 2;

        DisableTeamPanel();
        UpdateTeamPanel(false);

        for (int i = 0; i < teamSize; i++)
        {
            if (!Column1[i].activeSelf)
            {
                Column1[i].SetActive(true);
                texts = Column1[i].GetComponentsInChildren<Text>();
                image = Column1[i].GetComponent<RectTransform>().Find("PlayStateImage").GetComponent<Image>();
                texts[0].text = PhotonNetwork.NickName;
                if (PhotonNetwork.IsMasterClient)
                {
                    texts[1].text = "Master";
                    image.color = readyColor;
                }
                else
                {
                    texts[1].text = "UnReady";
                    image.color = unReadyColor;
                }

                costomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "Column","Column1" },
                    { "RowNum",i },
                    { "isReady",false },
                    { "PlayerNum",0 }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(costomProperties);
                break;
            }
            else if (!Column2[i].activeSelf)
            {
                Column2[i].SetActive(true);
                texts = Column2[i].GetComponentsInChildren<Text>();
                //if (PhotonNetwork.IsMasterClient) texts[1].text = "Master";
                //else texts[1].text = "UnReady";
                if (PhotonNetwork.IsMasterClient)
                {
                    texts[1].text = "Master";
                    image.color = readyColor;
                }
                else
                {
                    texts[1].text = "UnReady";
                    image.color = unReadyColor;
                }
                costomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "Column","Column2" },
                    { "RowNum",i },
                    { "isReady",false },
                    { "PlayNum",0 }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(costomProperties);
                break;
            }
        }
        ReadyButtonControl();

        base.OnEnable();
    }





    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {

        DisableTeamPanel(); 
        UpdateTeamPanel(true);
    }


    public override void OnMasterClientSwitched (Player newMasterClient) {
		ReadyButtonControl ();
	}


    public override void OnPlayerLeftRoom(Player otherPlayer){
		DisableTeamPanel ();
		UpdateTeamPanel (true);	
	}


	void DisableTeamPanel(){
		for (int i = 0; i < Column1.Length; i++) {
            Column1[i].SetActive (false);
		}
		for (int i = 0; i < Column2.Length; i++) {
            Column2[i].SetActive (false);
		    

        }
	}


	void UpdateTeamPanel(bool isUpdateSelf){
		GameObject go;
        foreach (Player p in PhotonNetwork.PlayerList) {
            if (!isUpdateSelf && p.IsLocal) continue;

            costomProperties = p.CustomProperties;	
			if (costomProperties ["Column"].Equals ("Column1")) {	
				go = Column1 [(int)costomProperties ["RowNum"]];	
				go.SetActive (true);	
				texts = go.GetComponentsInChildren<Text> ();
                image = go.GetComponent<RectTransform>().Find("PlayStateImage").GetComponent<Image>();

            }
            else {											
				go = Column2 [(int)costomProperties ["RowNum"]];	
				go.SetActive (true);
				texts = go.GetComponentsInChildren<Text> ();
                image = go.GetComponent<RectTransform>().Find("PlayStateImage").GetComponent<Image>();
            }
            texts [0].text = p.NickName;
            if (p.IsMasterClient)
            {
                texts[1].text = "Master";
                image.color = readyColor;
            }
            else if ((bool)costomProperties ["isReady"]) {
				texts [1].text = "Ready";
                image.color = readyColor;
			}
            else
            {
                texts[1].text = "UnReady";
                image.color = unReadyColor;
            }
        }
	}
    
	void ReadyButtonControl(){
		if (PhotonNetwork.IsMasterClient) {
			readyButton.GetComponentInChildren<Text> ().text = "Start Game";
			readyButton.onClick.RemoveAllListeners ();	
			readyButton.onClick.AddListener (delegate() {
				ClickStartGameButton ();		
			});
		} else {
            if((bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"])	
				readyButton.GetComponentInChildren<Text> ().text = "Cancel Ready";		
			else 
				readyButton.GetComponentInChildren<Text> ().text = "Ready";
			readyButton.onClick.RemoveAllListeners ();	
			readyButton.onClick.AddListener (delegate() {		
				ClickReadyButton ();							
			});
		}
	}
    

    
	public void ClickReadyButton(){
        bool isReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties ["isReady"];	
		costomProperties = new ExitGames.Client.Photon.Hashtable (){ { "isReady",!isReady } };	
        PhotonNetwork.LocalPlayer.SetCustomProperties (costomProperties);
		Text readyButtonText = readyButton.GetComponentInChildren<Text> ();
        if (isReady)
        {
            readyButtonText.text = "Ready";
        }
        else
        {
            readyButtonText.text = "Cancel Ready";
        }
	}
    
	public void ClickStartGameButton(){
        foreach (Player p in PhotonNetwork.PlayerList) {
			if (p.IsLocal) continue;
			if ((bool)p.CustomProperties ["isReady"] == false) {
                promptMessage.text = "Someone is not ready, the game can't start";
				return;	
			}
		}
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            promptMessage.text = "The Column is not full, the game can't start";
            return;
        }
        promptMessage.text = "";
        photonView.RPC("setPlayerNum", RpcTarget.MasterClient);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        photonView.RPC ("LoadGameScene", RpcTarget.All, "MainScene");
        PhotonNetwork.IsMessageQueueRunning = false;

    }


    [PunRPC]
    public void setPlayerNum()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){ { "PlayerNum",i } });
        }
    }

    [PunRPC]
	public void LoadGameScene(string sceneName){	
		PhotonNetwork.LoadLevel (sceneName);
	}
}
