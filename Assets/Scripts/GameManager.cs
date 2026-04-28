using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public statsSetter stats;
    void Awake()
    {
        Wavedash.SDK.Init(new Dictionary<string, object>
      { { "debug", true } });
        var user = Wavedash.SDK.GetUser();
        Debug.Log($"Playing as: {user["username"]}!");
    }

    async void Start()
    {
        var leaderboard = await Wavedash.SDK.GetOrCreateLeaderboard(
          "OVERRIDE high-scores",
          WavedashConstants.LeaderboardSortMethod.DESCENDING,
          WavedashConstants.LeaderboardDisplayType.NUMERIC
        );

        await Wavedash.SDK.UploadLeaderboardScore(leaderboard["id"].ToString(), Convert.ToInt32(stats.trust), true);
        Wavedash.SDK.SetAchievement("first_win");
    }
}