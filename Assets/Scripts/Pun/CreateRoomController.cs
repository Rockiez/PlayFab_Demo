using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomController : MonoBehaviourPunCallbacks {

	public GameObject createRoomPanel;		
	public GameObject roomLoadingLabel;		
	public Text roomName;					
	public Text WarnLabel;				

    private List<RoomInfo> roomList;

	public void ClickConfirmCreateRoomButton(){
        WarnLabel.text = "";

        RoomOptions roomOptions =new RoomOptions();
        roomOptions.MaxPlayers = 4;

		bool isRoomNameRepeat = false;

        this.roomList = gameObject.GetComponentInParent<LobbyPanelController>().roomInfo;
        if (roomName.text == string.Empty)
        {
            WarnLabel.text = "Room Name is Invalid";

            return;
        }
        if (this.roomList != null)
	    {
	        foreach (RoomInfo info in this.roomList)
	        {
	            if (roomName.text == info.Name)
	            {
	                isRoomNameRepeat = true;
	                break;
	            }
	        }
        }

		if (isRoomNameRepeat) {
            WarnLabel.text = "Repeated Room Name!";
		}
		else {
			PhotonNetwork.CreateRoom (roomName.text, roomOptions, TypedLobby.Default);	

		}
	}
    public override void OnCreatedRoom()
    {
        WarnLabel.text = "";
        createRoomPanel.SetActive(false);
        roomLoadingLabel.SetActive(true);

        
    }
    
    public void ClickCancelCreateRoomButton(){
		createRoomPanel.SetActive (false);
        WarnLabel.text = "";		
	}
}
