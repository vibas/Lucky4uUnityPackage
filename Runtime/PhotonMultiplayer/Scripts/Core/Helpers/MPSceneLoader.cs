using UnityEngine;
using System.Threading.Tasks;
using System;

/// <summary>
/// Helps in loading scene over network using photon's inbuild API
/// </summary>
public class MPSceneLoader
{
    IMultiplayerConnection multiplayerConnection;
    string GamePlaySceneName;
    bool isGameSceneLoaded = false;
    public Action OnGameplaySceneLoaded;

    public void Init(IMultiplayerConnection connection, string gameplaySceneName)
    {
        this.GamePlaySceneName = gameplaySceneName;
        this.multiplayerConnection = connection;
        multiplayerConnection.OnMaxPlayersJoinedRoomEvent -= InitGameParamsAndLoadGame;
        multiplayerConnection.OnMaxPlayersJoinedRoomEvent += InitGameParamsAndLoadGame;
    }

    private void InitGameParamsAndLoadGame()
    {
        if(!isGameSceneLoaded)
        {
            Debug.Log("Max players joined . Init GameParams And LoadGame");
            LoadSceneAfterDelay(1);
        }
        else
        {
            Debug.LogError("Already inside the game scene");
        }
    }

    async void LoadSceneAfterDelay(int waitTime)
    {
        await Task.Delay(waitTime * 1000);
        multiplayerConnection.LoadSceneOverNetwork(GamePlaySceneName);
        isGameSceneLoaded = true;

        OnGameplaySceneLoaded?.Invoke();
    }
}