using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject[] leaderboardEntries;
    public static Leaderboard instance;
    void Awake() { instance = this; }

    public void OnLoggedIn()
    {
        leaderboardCanvas.SetActive(true);
        //SetLeaderboardEntry(-8700);
        DisplayLeaderboard();
    }

    public void DisplayLeaderboard()
    {
        GetLeaderboardRequest getLeaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "FastestTime",
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(getLeaderboardRequest,
            result => UpdateLeaderboardUI(result.Leaderboard),
            error => Debug.Log(error.ErrorMessage)
        );
    }

    void UpdateLeaderboardUI(List<PlayerLeaderboardEntry> leaderboard)
    {
        for (int x = 0; x < leaderboardEntries.Length; x++)
        {
            leaderboardEntries[x].SetActive(x < leaderboard.Count);

            if (x >= leaderboard.Count) 
                continue;

            leaderboardEntries[x].transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = (leaderboard[x].Position + 1) + ". " + leaderboard[x].DisplayName;
            leaderboardEntries[x].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = (-(float)leaderboard[x].StatValue * 0.001f).ToString("F2");
        }
    }

    public void SetLeaderboardEntry(int newScore)
    {
        // NOTE: the original version of this game used server-side automation in Javascript to update
        // the leaderboard. Microsoft removed that feature for new PlayFab applications, so we'll demonstrate
        // updating a leaderboard using two different methods below. The 'legacy method' calls the custom server-side
        // javascript (cloud script). The alternative uses the latest PlayFabClientAPI to update the player's best score

        bool useLegacyMethod = false;

        if (useLegacyMethod)
        {
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpdateHighscore",
                FunctionParameter = new { score = newScore }
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log(result);
                //Debug.Log("SUCCESS");
                //Debug.Log(result.FunctionName);
                //Debug.Log(result.FunctionResult);
                //Debug.Log(result.FunctionResultTooLarge);
                //Debug.Log(result.Error);
                DisplayLeaderboard();
                Debug.Log(result.ToJson());
            },
            error =>
            {
                Debug.Log(error.ErrorMessage);
                Debug.Log("ERROR");
            }
            );
        }
        else
        {
        // ...and here's how we do it
        // NOTE: by default, clients can't update player statistics
        // So for the code below to succeed:
        // 1. Log into PlayFab (from your web browser)
        // 2. Select your Title.
        // 3. Select Settings from the left-menu.
        // 4. Select the API Features tab.
        // 5. Find and activate Allow client to post player statistics.
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate { StatisticName = "FastestTime", Value = newScore },
                }
            },
            result => { Debug.Log("User statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); }
            );
        }
    }
}
