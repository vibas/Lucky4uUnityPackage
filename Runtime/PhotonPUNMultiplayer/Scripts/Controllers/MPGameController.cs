using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Pun;

namespace PhotonPUNMultiplayer
{
    public class MPGameController : MonoBehaviour
    {
        public float loadingTime;
        public List<IPhotonPlayer> GamePlayersList;

        public CountdownTimer countdownTimerComponent;
        bool IsMyPlayerSpawned;

        IMultiplayerConnection MultiplayerConnection;
        int GameStartCountDownTime;
        string PlayerPrefabPath;

        public void Init(IMultiplayerConnection multiplayerConnection, int gamestartCountDownTime, string playerPrefabPath)
        {
            this.MultiplayerConnection = multiplayerConnection;
            this.GameStartCountDownTime = gamestartCountDownTime;
            this.PlayerPrefabPath = playerPrefabPath;

            this.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            countdownTimerComponent.Countdown = GameStartCountDownTime;
            CountdownTimer.OnCountDowsTimerStart -= OnCountDownTimerStartEvent;
            CountdownTimer.OnCountDowsTimerStart += OnCountDownTimerStartEvent;

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

            CountdownTimer.OnCountDownTimerTick -= OnCountDownTimerTick;
            CountdownTimer.OnCountDownTimerTick += OnCountDownTimerTick;
            
            MultiplayerConnection.OnAllPlayersReady -= HandleAllPlayersReadyEvent;
            MultiplayerConnection.OnAllPlayersReady += HandleAllPlayersReadyEvent;

            if(MultiplayerConnection is IPUNConnection)
            {
                IPUNConnection punConnection = MultiplayerConnection as IPUNConnection;
                punConnection.OnEventRaised -= ProcessRaisedEvent;
                punConnection.OnEventRaised += ProcessRaisedEvent;

                punConnection.OnRoomPropertiesUpdated -= ProcessRoomPropertiesUpdate;
                punConnection.OnRoomPropertiesUpdated += ProcessRoomPropertiesUpdate;
            }
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(loadingTime);
            SetGameReadyStatus();
        }

        // ============== COUNT DOWN TIMER ====================//

        void OnCountDownTimerStartEvent()
        {
            
        }

        void OnCountDownTimerTick(string timeString)
        {
            Debug.LogError(timeString);
        }

        void OnCountdownTimerIsExpired()
        {
            
        }

        // ================== PLAYER READY STATUS ====================//
        void SetGameReadyStatus()
        {
            MultiplayerConnection.AcknowledgePlayerReadyStatus();
        }

        void HandleAllPlayersReadyEvent()
        {
            SpawnGamePlayer();

            if (MultiplayerConnection.IsMasterClient)
            {
                CountdownTimer.SetStartTime();
            }
        }

        public void SetLocalPlayerCustomProperties(Dictionary<string,object> keyValuePairs)
        {
            MultiplayerConnection.SetPlayerCustomProperties(keyValuePairs);
        }

        // ====================== SPAWN PLAYER =======================//
        void SpawnGamePlayer()
        {
            if (IsMyPlayerSpawned)
                return;

            GameObject photonPlayerGO = MultiplayerConnection.SpawnPlayer(this.PlayerPrefabPath);
            if (photonPlayerGO != null)
            {
                IPhotonPlayer photonPlayer = photonPlayerGO.GetComponent<IPhotonPlayer>();
                photonPlayer.OnDestroyedEvent -= RemovePlayerFromAllPlayersList;
                photonPlayer.OnDestroyedEvent += RemovePlayerFromAllPlayersList;
                AddPlayerToAllPlayersList(photonPlayerGO.GetComponent<IPhotonPlayer>());
                IsMyPlayerSpawned = true;
            }
        }

        // ================= ADD/REMOVE PLAYERS IN LIST ==============//
        public void AddPlayerToAllPlayersList(IPhotonPlayer player)
        {
            if (player == null)
                return;

            if (GamePlayersList == null)
            {
                GamePlayersList = new List<IPhotonPlayer>();
            }

            if (!GamePlayersList.Contains(player))
            {
                GamePlayersList.Add(player);
            }
        }

        public void RemovePlayerFromAllPlayersList(IPhotonPlayer player)
        {
            if (player == null)
                return;

            if (GamePlayersList.Contains(player))
            {
                GamePlayersList.Remove(player);
            }
        }

        // =================== RPC ============================//
        public void SendRPCTest()
        {
            foreach (var item in GamePlayersList)
            {
                Dictionary<string, object> dataDict = new Dictionary<string, object>();
                dataDict.Add(MultiplayerGameConstants.TEST_RPC_KEY, "TestValue");

                item.SendData(MultiplayerGameConstants.TEST_RPC_METHOD_ID, dataDict);
            }
        }

        public static void RPC_DataReceived(int methodID, Dictionary<string,object> data)
        {
            Debug.LogError($"Data received from {data[MultiplayerGameConstants.RPC_CALLER_ID]}");
            Debug.LogError("Execute Method : " + methodID);
        }


        // ========================== RAISE EVENT =====================//
        public void RaiseEventTest()
        {
            if(MultiplayerConnection is IPUNConnection)
            {
                IPUNConnection punConnection = MultiplayerConnection as IPUNConnection;
                punConnection.RaiseEvents(MultiplayerGameConstants.TestEventCode,
                                      new object[]
                                      {
                                        PhotonNetwork.LocalPlayer.ActorNumber
                                      });
            }
            else
            {
                Debug.LogError("Not Connected to PUN server");
            }
        }

        void ProcessRaisedEvent(byte eventCode, object eventData)
        {
            switch (eventCode)
            {
                case MultiplayerGameConstants.TestEventCode:
                    object[] testEventData = (object[])eventData;
                    int actorNumber = int.Parse(testEventData[0].ToString());
                    Debug.LogError($"{actorNumber} raised event");
                    break;
            }
        }

        // ================ SET ROOM PROPERTIES ===================//
        public void SetRoomCustomProperties()
        {
            if (MultiplayerConnection is IPUNConnection)
            {
                IPUNConnection punConnection = MultiplayerConnection as IPUNConnection;

                ExitGames.Client.Photon.Hashtable roomCustomProperties = new ExitGames.Client.Photon.Hashtable();
                roomCustomProperties.Add("Level", 1);
                roomCustomProperties.Add("Environment", "Forest");
                punConnection.SetRoomCustomProperties(roomCustomProperties);
            }
            else
            {
                Debug.LogError("Not Connected to PUN server");
            }
        }

        public void ProcessRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable roomProerties)
        {
            Debug.LogError("=== OnRoomPropertiesUpdate ===");
            foreach (var item in roomProerties)
            {
                Debug.LogError(item.Key + " : " + item.Value);
            }
            
            if(isResetRequested)
            {
                if (roomProerties.ContainsKey("RESET"))
                {
                    if (MultiplayerConnection.IsMasterClient)
                    {
                        CountdownTimer.SetStartTime();
                        isResetRequested = false;
                    }
                }
            }
            
        }


        // =============== RESET ===================//
        bool isResetRequested = false;
        public void Reset()
        {
            if (MultiplayerConnection is IPUNConnection && MultiplayerConnection.IsMasterClient)
            {
                IPUNConnection punConnection = MultiplayerConnection as IPUNConnection;
                ExitGames.Client.Photon.Hashtable roomCustomProperties = new ExitGames.Client.Photon.Hashtable();
                roomCustomProperties.Add("RESET", 1);
                punConnection.ClearRoomCustomProperties();
                punConnection.SetRoomCustomProperties(roomCustomProperties);
                isResetRequested = true;
            }
        }
    }
}