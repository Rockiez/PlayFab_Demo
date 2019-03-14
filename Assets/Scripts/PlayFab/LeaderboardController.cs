using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
public class LeaderboardController : MonoBehaviour {

    public GameObject leaderboardPanel;
    public GameObject leaderboardLoadingLabel;


    public Button TotalKillButton;
    public Button KillPerDeathButton;
    public Button TotalWinButton;

    public GameObject[] users;
    public GameObject localUser;


	private Dictionary<string, Dictionary<uint, uint>> leaderboard = new Dictionary<string, Dictionary<uint,uint>>();
	private string leaderboardType="";
    private Text[] localUserTexts;
    private List<StatisticValue> localUserStatistics;
    private GameObject but;
    private GameObject[] inputs;

	void OnEnable () {
        localUserTexts = localUser.GetComponentsInChildren<Text>();
        localUserTexts[1].text = PlayFabUserData.username;
        localUserStatistics = new List<StatisticValue>();
        but = localUser.GetComponentInChildren<Button>().gameObject;
        List<GameObject> inputsList = new List<GameObject>();
        foreach (var user in users)
        {
            inputsList.Add(user.GetComponentInChildren<InputField>().gameObject);
        }

        inputs = inputsList.ToArray();

        TotalKillButton.Select();
		ClickTotalKillButton ();
    }


    public void ClickTotalKillButton(){
        leaderboardLoadingLabel.SetActive(true);

        leaderboardType = "TotalKill";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "-";
        texts[2].text = "-";
        but.SetActive(false);
        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "-";
            texts[1].text = "-";
            texts[2].text = "-";
        }

        foreach (var input in inputs)
        {
            input.SetActive(false);
        }

        var leaderboardRequest = new GetLeaderboardRequest() {
            MaxResultsCount = 100,
            StatisticName = "TotalKill",StartPosition=0,
            ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowAvatarUrl = true
                }
        };
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnGetLeaderboard, OnPlayFabError);
    }

	public void ClickKillPerDeathButton(){
        leaderboardLoadingLabel.SetActive(true);

        leaderboardType = "KillPerDeath";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "-";
        texts[1].text = PlayFabUserData.username;
        texts[2].text = "-";
        but.SetActive(false);
        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "-";
            texts[1].text = "-";
            texts[2].text = "-";

        }
        foreach (var input in inputs)
        {
            input.SetActive(false);
        }

        var leaderboardRequest = new GetLeaderboardRequest()
        {
            MaxResultsCount = 100,
            StatisticName = "KillPerDeath",
            StartPosition = 0,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnGetLeaderboard, OnPlayFabError);
    }

	public void ClickTotalWinButton(){
        leaderboardLoadingLabel.SetActive(true);

        leaderboardType = "TotalWin";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "-";
        texts[1].text = PlayFabUserData.username;
        texts[2].text = "-";
        but.SetActive(false);
        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "-";
            texts[1].text = "-";
            texts[2].text = "-";

        }
        foreach (var input in inputs)
        {
            input.SetActive(false);
        }
        var leaderboardRequest = new GetLeaderboardRequest()
        {
            MaxResultsCount = 100,
            StatisticName = "TotalWin",
            StartPosition = 0,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true            }
        };
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnGetLeaderboard, OnPlayFabError);
    }
    public void ClickSetStatisticsButton()
    {

        leaderboardType = "";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "";
        texts[1].text = "";
        texts[2].text = "";

        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "";
            texts[1].text = "";
            texts[2].text = "";
        }

        foreach (var gb in inputs)
        {
            gb.SetActive(true);
        }
        but.SetActive(true);

        var statisticsRequest = new GetPlayerStatisticsRequest()
        {
        };
        PlayFabClientAPI.GetPlayerStatistics(statisticsRequest, s =>
         {
             int i = 0;
             foreach (StatisticValue statistic in s.Statistics)
             {
                 localUserStatistics.Add(statistic);
                 if (i < 3)
                 {
                     texts = users[i].GetComponentsInChildren<Text>();
                     texts[0].text = statistic.StatisticName;
                     texts[1].text = "";
                     texts[2].text = "";
                     inputs[i].GetComponent<InputField>().text = statistic.Value.ToString();
                     users[i].SetActive(true);
                 }
                 i++;
             }
             localUserTexts[0].text = "";
             localUserTexts[1].text = "";
             localUserTexts[2].text = "";

         }, OnPlayFabError);
    }

    public void OnClickUpdateStatisticsButton()
    {
        var updateStatisticsRequest = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
        };
        int i = 0;
        foreach (var statistics in localUserStatistics)
        {
            updateStatisticsRequest.Statistics.Add(
                new StatisticUpdate() {
                    StatisticName = statistics.StatisticName,
                    Value = int.Parse(inputs[i].GetComponent<InputField>().text) });
            i++;
        }


        PlayFabClientAPI.UpdatePlayerStatistics(updateStatisticsRequest, OnUpdateStatisticsResult, OnPlayFabError);
    }

    void OnUpdateStatisticsResult(UpdatePlayerStatisticsResult result)
    {
        ClickSetStatisticsButton();
    }

    void OnGetLeaderboard(GetLeaderboardResult result){
		leaderboard.Clear ();

		foreach (PlayerLeaderboardEntry entry in result.Leaderboard) {
            if (entry.DisplayName == null)
            {
                leaderboard.Add(entry.PlayFabId, new Dictionary<uint, uint> { { (uint)entry.Position, (uint)entry.StatValue } });
            }
            else
            {
                leaderboard.Add(entry.DisplayName, new Dictionary<uint, uint> { { (uint)entry.Position, (uint)entry.StatValue } });
            }
        }
		SetLeadboard ();
	}


	void SetLeadboard(){
		leaderboardLoadingLabel.SetActive (false);
		int i = 0;
        Text[] texts;
		foreach (KeyValuePair<string, Dictionary<uint, uint>> kvp in leaderboard) {

            if (i < 3)
            {
                texts = users[i].GetComponentsInChildren<Text>();
                texts[0].text = (i + 1).ToString();
                texts[1].text = kvp.Key;
                if (leaderboardType == "TotalKill" || leaderboardType == "TotalWin")
                    texts[2].text = leaderboardType + "：" + kvp.Value[(uint)i].ToString();
                else if (leaderboardType == "KillPerDeath")
                    texts[2].text = leaderboardType + "：" + kvp.Value[(uint)i].ToString();

                users[i].SetActive(true);

            }

            if (kvp.Key == PlayFabUserData.username)
            {
                localUserTexts[0].text = i.ToString();
                if (leaderboardType == "TotalKill" || leaderboardType == "TotalWin")
                    localUserTexts[2].text = leaderboardType + "：" + kvp.Value[(uint)i].ToString();
                else if (leaderboardType == "KillPerDeath")
                    localUserTexts[2].text = leaderboardType + "：" + kvp.Value[(uint)i].ToString();
            }
			i++;
		}
        localUser.SetActive(true);
	}

    void OnPlayFabError(PlayFabError error){
		Debug.LogError ("Get an error:" + error.Error);
	}
}
