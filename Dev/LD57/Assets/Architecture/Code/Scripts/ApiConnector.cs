using System;
using System.Collections.Generic;
using GraphQlClient.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ApiConnector : MonoBehaviour
{
    private const string API_KEY = "Ojqnsf56qqsXTzIhINrWnjq2wBGX6eq3NC4TaKmzi0RWVsBKdOa7M5jTpXbH6jye";
    private List<PlayerData> _playerDatas = new(6);
    private Options _options;

    [SerializeField] private GraphApi graphApi;

    public async void AddPlayer(string playerId, int playerScore, int playerFishCaught)
    {
        GameManager.Instance.isDataGot = false;
        GraphApi.Query createPlayerQuery = graphApi.GetQueryByName("addPlayer", GraphApi.Query.Type.Mutation);
        createPlayerQuery.SetArgs(new{objects = new{player_name = playerId,
            player_score = playerScore, player_fish = playerFishCaught}});
	    UnityWebRequest request = await graphApi.Post(createPlayerQuery);
        GameManager.Instance.isDataGot = true;
    }
    
    public async void UpdatePlayerScore(string playerId, int playerScore, int playerFishCaught)
    {
        GameManager.Instance.isDataGot = false;
        GraphApi.Query updateScoreMutation = graphApi.GetQueryByName("updatePlayerScore", GraphApi.Query.Type.Mutation);
        updateScoreMutation.SetArgs(new {pk_columns = new{objects = new{player_name = playerId,
            player_score = playerScore, player_fish = playerFishCaught}}});
	    UnityWebRequest request = await graphApi.Post(updateScoreMutation);
        GameManager.Instance.isDataGot = true;
    }
    public async void DisplayTopScore()
    {
        GameManager.Instance.isDataGot = false;
        GraphApi.Query displayTopPlayers = graphApi.GetQueryByName("topPlayers", GraphApi.Query.Type.Query);
        displayTopPlayers.SetArgs(new { limit = 5, order_by = new {player_score = Options.desc}});
	    UnityWebRequest request = await graphApi.Post(displayTopPlayers);
        JObject jObject = JObject.Parse(request.downloadHandler.text);
        JArray returnValues = (JArray)jObject["data"]["players"];

        for (int i = 0; i < returnValues.Count; i++)
        {
            JObject entry = (JObject) returnValues[i];
            PlayerData playerData = new();
            playerData.playerName = (string) entry["player_name"];
            playerData.playerScore = (int) entry["player_score"];
            playerData.playerFish = (int) entry["player_fish"];
            _playerDatas.Add(playerData);
        }
        GameManager.Instance.isDataGot = true;
    }

    public List<PlayerData> GetPlayerData()
    {
        return _playerDatas;
    }
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public int playerScore;
    public int playerFish;
}

public enum Options
{
    desc,
    asc,
}
