using UnityEngine;
using System.Threading.Tasks;
using System;

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
            Debug.LogError("Max players joined . Init GameParams And LoadGame");
            LoadSceneAfterDelay(3);
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