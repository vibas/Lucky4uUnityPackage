using System.Collections.Generic;
using PhotonPUNMultiplayer;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using System;
using UnityEngine;

public class PUNMultiplayerHelper
{
    // =============== ACTIONS =======================//
    public Action OnJoinedRoomEvent;
    public Action OnGameplaySceneLoadedEvent;
    
    public Action<Dictionary<string, object>> OnMyPlayerPropertiesChangedEvent;
    public Action<Dictionary<string, object>> OnOtherPlayerPropertiesChangedEvent;
    public Action<Dictionary<string, object>> OnRoomPropertiesUpdatedEvent;
    public Action<byte, object> OnRaisedEventDataReceived;

    public Action OnCountDownStartEvent;
    public Action<string> OnCountDownTimerTickEvent;
    public Action OnCountDownExpiredEvent;

    public Action<int> OnPlayerLeftRoomEvent;

    // ================== HELPERS ======================//
    private MPConnectionHelper mpConnectionHelper;
    private MPSceneLoader mpSceneLoader;

    // ================= PROPERTIES ======================//
    public IMultiplayerConnection _Connection => mpConnectionHelper.Connection;
    public bool IsMyPlayer(int playerNumber) => PhotonNetwork.LocalPlayer.GetPlayerNumber() == playerNumber;
    public bool IsMasterClient => _Connection.IsMasterClient;

    public byte MyTeamCode 
    {
        get 
        {
            byte teamCode = 10;
            if(_Connection is IPUNConnection)
            {
                IPUNConnection pun_connection = _Connection as IPUNConnection;
                PUNConnectionManager punConnectionManager = (PUNConnectionManager)pun_connection;
                teamCode = punConnectionManager.MyTeamCode;
            }
            return teamCode;
        }
    }

    public byte OtherTeamCode
    {
        get
        {
            byte teamCode = 10;
            if (_Connection is IPUNConnection)
            {
                IPUNConnection pun_connection = _Connection as IPUNConnection;
                PUNConnectionManager punConnectionManager = (PUNConnectionManager)pun_connection;
                teamCode = punConnectionManager.OtherTeamCode;
            }
            return teamCode;
        }
    }

    // =========== CONSTRUCTOR ==============//
    public PUNMultiplayerHelper()
    {
        mpConnectionHelper = new MPConnectionHelper();
        mpSceneLoader = new MPSceneLoader();
    }


    // Called while starting the multiplayer connection
    public void InitMultiplayerConnection(string userName,string gameplaySceneName,int gamestartCountDownTimer)
    {
        // Initialize Multiplayer Connection
        mpConnectionHelper.InitialiseConnection();
        _Connection.SetPlayerName(userName);
        
        // Initialize Multiplayer Game Scene Loader
        mpSceneLoader.Init(_Connection, gameplaySceneName);
        mpSceneLoader.OnGameplaySceneLoaded -= ProcessGameplaySceneLoadedEvent;
        mpSceneLoader.OnGameplaySceneLoaded += ProcessGameplaySceneLoadedEvent;

        // Start Connection
        _Connection.Connect();

        // Subscribe to photon network events
        _Connection.OnRoomJoinEvent -= ProcessJoinedRoomEvent;
        _Connection.OnRoomJoinEvent += ProcessJoinedRoomEvent;

        _Connection.OnPlayerLeftRoomEvent -= ProcessPlayerLeftRoomEvent;
        _Connection.OnPlayerLeftRoomEvent += ProcessPlayerLeftRoomEvent;

        // Deal with PUN Specific Connection
        if (_Connection is IPUNConnection)
        {
            PUNCountdownTimer countdownTimer = mpConnectionHelper.currentConnectionGO.GetComponent<PUNCountdownTimer>();
            countdownTimer.Countdown = gamestartCountDownTimer;

            IPUNConnection punConnection = _Connection as IPUNConnection;
            punConnection.OnRoomPropertiesUpdated -= ProcessRoomPropertiesUpdatedEvent;
            punConnection.OnRoomPropertiesUpdated += ProcessRoomPropertiesUpdatedEvent;

            punConnection.OnPlayerPropertiesUpdated -= ProcessPlayerPropertiesUpdatedEvent;
            punConnection.OnPlayerPropertiesUpdated += ProcessPlayerPropertiesUpdatedEvent;

            punConnection.OnEventRaised -= ProcessOnRaiseEventData;
            punConnection.OnEventRaised += ProcessOnRaiseEventData;
        }
    }

    // Disconnect from multiplayer connection
    public void Disconnect()
    {
        if (mpConnectionHelper != null && _Connection != null)
        {
            _Connection.Disconnect();
        }
    }

    // When Gameplay scene is loaded over network
    void ProcessGameplaySceneLoadedEvent()
    {
        OnGameplaySceneLoadedEvent?.Invoke();
    }

    // When a player joins a room
    void ProcessJoinedRoomEvent(object obj)
    {
        OnJoinedRoomEvent?.Invoke();

        if (_Connection is IPUNConnection)
        {
            IPUNConnection punConnection = _Connection as IPUNConnection;
            if (!IsMasterClient)
            {
                ProcessRoomPropertiesUpdatedEvent(punConnection.GetRoomCustomProperties());
            }
        }
    }

    // When a player leaves a room
    void ProcessPlayerLeftRoomEvent(object obj)
    {
        if(_Connection is IPUNConnection)
        {
            Photon.Realtime.Player player = (Photon.Realtime.Player)obj;
            int playerNumber = player.GetPlayerNumber();
            Debug.LogError($"{player.GetPlayerNumber()} has left the room!");
            OnPlayerLeftRoomEvent?.Invoke(playerNumber);
        }
    }

    // ========================== RAISE EVENT =====================//
    public void RaiseEvent(byte eventCode, object[] data)
    {
        if (_Connection is IPUNConnection)
        {
            IPUNConnection punConnection = _Connection as IPUNConnection;
            punConnection.RaiseEvents(eventCode, data);
        }
        else
        {
            Debug.LogError("Not Connected to PUN server");
        }
    }
    void ProcessOnRaiseEventData(byte eventCode, object dataObj)
    {
        OnRaisedEventDataReceived?.Invoke(eventCode, dataObj);
    }
    // ================ ROOM PROPERTIES ===================//
    public void SetRoomProperties(Dictionary<string, object> keyValuePairs)
    {
        if (_Connection is IPUNConnection)
        {
            IPUNConnection punConnection = _Connection as IPUNConnection;

            ExitGames.Client.Photon.Hashtable roomCustomProperties = new ExitGames.Client.Photon.Hashtable();
            foreach (var item in keyValuePairs)
            {
                roomCustomProperties.Add(item.Key, item.Value);
            }
            punConnection.SetRoomCustomProperties(roomCustomProperties);
        }
        else
        {
            Debug.LogError("Not Connected to PUN server");
        }
    }

    public void RemoveRoomProperties(object key)
    {
        if (_Connection is IPUNConnection)
        {
            IPUNConnection punConnection = _Connection as IPUNConnection;
            punConnection.RemoveRoomCustomProperties(key);
        }
    }

    void ProcessRoomPropertiesUpdatedEvent(ExitGames.Client.Photon.Hashtable roomProerties)
    {
        if (roomProerties == null)
            return;

        Dictionary<string, object> roomPropDict = new Dictionary<string, object>();
        foreach (var item in roomProerties)
        {
            roomPropDict.Add(item.Key.ToString(), item.Value);
        }

        OnRoomPropertiesUpdatedEvent?.Invoke(roomPropDict);
    }

    // ================== PLAYER PROPERTIES ========================//
    public void SetPlayerProperties(Dictionary<string, object> keyValuePairs)
    {
        _Connection.SetPlayerCustomProperties(keyValuePairs);
    }
    void ProcessPlayerPropertiesUpdatedEvent(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable playerProperties)
    {
        Dictionary<string, object> playerPropDict = new Dictionary<string, object>();
        foreach (var item in playerProperties)
        {
            playerPropDict.Add(item.Key.ToString(), item.Value);
        }

        if (IsMyPlayer(player.GetPlayerNumber()))
        {
            // My Properties changed
            OnMyPlayerPropertiesChangedEvent?.Invoke(playerPropDict);
        }
        else
        {
            // Other player's properties changed
            OnOtherPlayerPropertiesChangedEvent?.Invoke(playerPropDict);
        }
    }

    // ================= COUNT DOWN TIMER =========================//
    public void StartNewCountDownTimer()
    {
        if (IsMasterClient)
        {
            PUNCountdownTimer.OnCountDowsTimerStart -= OnCountDownTimerStartEvent;
            PUNCountdownTimer.OnCountDowsTimerStart += OnCountDownTimerStartEvent;

            PUNCountdownTimer.OnCountDownTimerTick -= OnCountDownTimerTick;
            PUNCountdownTimer.OnCountDownTimerTick += OnCountDownTimerTick;

            PUNCountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
            PUNCountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

            PUNCountdownTimer.SetStartTime();
        }
    }

    void OnCountDownTimerStartEvent()
    {
        OnCountDownStartEvent?.Invoke();
    }

    void OnCountDownTimerTick(string timeString)
    {
        OnCountDownTimerTickEvent?.Invoke(timeString);
    }

    void OnCountdownTimerIsExpired()
    {
        RemoveRoomProperties(PUNCountdownTimer.CountdownStartTimeKey);
        OnCountDownExpiredEvent?.Invoke();
    }
}