using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct UserInfo
{
    public int Health;
    public int Attack;
    public int Shield;

    public Dictionary<string, object> GetStatsDictionary()
    {
        Dictionary<string, object> statsDict = new Dictionary<string, object>();
        statsDict.Add("H", Health);
        statsDict.Add("A", Attack);
        statsDict.Add("S", Shield);

        return statsDict;
    }
}

public class AppController : MonoBehaviour
{
    [Header("USER DATA")]
    // =========== USER PROGRESSION INFO =========== //
    public string UserName;
    public UserInfo userInfo;
    // ==============================================//
    [Header("SCREEN")]
    [SerializeField] LobbyScreen homeScreen;
    [SerializeField] PhotonMatchmakingScreen matchmakingScreen;
    // ==============================================//
    [Header("SCENE")]
    public string GameplaySceneName;
    // ==============================================//
    [Header("PREFAB PATH")]
    public string PUN_GamePlayerPrefabPath = "MultiplayerMode/Prefabs/PUNGamePlayer";
    // ==============================================//
    public int GameStartCountDownTime;
    // ==============================================//
    public MPConnectionHelper mpConnectionHelper = new MPConnectionHelper();
    private MPSceneLoader mpSceneLoader = new MPSceneLoader();
    private static AppController instance;
    public static AppController Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        homeScreen.OnStartButtonPressedEvent += HomeScreenStartButtonPressed;
        matchmakingScreen.OnCancelButtonPressedEvent += MatchmakingScreenCancelButtonPressed;
    }

    private void OnDisable()
    {
        homeScreen.OnStartButtonPressedEvent -= HomeScreenStartButtonPressed;
        matchmakingScreen.OnCancelButtonPressedEvent -= MatchmakingScreenCancelButtonPressed;
    }

    void StartMultiplayerGame()
    {
        // Initialize Multiplayer Connection
        mpConnectionHelper.InitialiseConnection();
        mpConnectionHelper.Connection.SetPlayerName(UserName);

        // Initialize Multiplayer Game Scene Loader
        mpSceneLoader.Init(mpConnectionHelper.Connection,GameplaySceneName);

        // Setup Player Custom Properties
        mpConnectionHelper.Connection.SetPlayerCustomProperties(userInfo.GetStatsDictionary());

        // Start Connection
        mpConnectionHelper.Connection.Connect();
    }

    void HomeScreenStartButtonPressed()
    {
        StartMultiplayerGame();

        homeScreen.gameObject.SetActive(false);
        matchmakingScreen.gameObject.SetActive(true);
    }

    void MatchmakingScreenCancelButtonPressed()
    {
        if (matchmakingScreen.gameObject != null)
            matchmakingScreen.gameObject.SetActive(false);

        homeScreen.gameObject.SetActive(true);

        if (mpConnectionHelper != null && mpConnectionHelper.Connection != null)
        {
            mpConnectionHelper.Connection.Disconnect();
        }
    }
}