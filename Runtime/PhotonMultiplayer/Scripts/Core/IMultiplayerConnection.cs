using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMultiplayerConnection
{
    #region Field Properties
    byte MaxPlayersPerRoom { get; set; }                        // Maximum number of players in a game
    int MyPhotonActorNumber { get; set; }                       // Current Player's unique ID over network
    bool IsMasterClient { get; set; }                           // For PUN master client & for Fusion Host player
    Dictionary<int, object> JoinedPlayersDict { get; }          // Dictionary that contains player reference along with ID
    #endregion

    #region Events Properties
    Action<object> OnRoomJoinEvent { get; set; }                // When a player joins room     
    Action<object> OnPlayerEnteredRoomEvent { get; set; }       // Callback at client A when client B enters into a game
    Action<object> OnPlayerLeftRoomEvent { get; set; }          // When any player except host player leaves the game       
    Action<string> OnConnectionStatusChangeEvent { get; set; }  // Connection status changed
    Action OnMaxPlayersJoinedRoomEvent { get; set; }            // When max players join the game 
    Action OnDisconnectedEvent { get; set; }                    // When player gets disconnected from server
    Action<object> OnRoomPlayerInfoUpdatedEvent { get; set; }   // Wehn player info updated
    Action OnAllPlayersReady { get; set; }                      // When all players are ready (Game play scene is loaded)
    #endregion

    #region Methods
    void Connect();                                             // Start Connection to Server
    void Disconnect();                                          // Stop Connection from Server
    void LoadSceneOverNetwork(string sceneName);                // Load a scene for all the players at a same time

    void TryGetPlayerInfo(object playerObj,
                            out int actorNumber,
                            out string playerName);             // Get actor number and name for matchmaking screen
    void SetPlayerName(string playerName);                      // Set Player Name on network

    void SetPlayerCustomProperties(Dictionary<string, object> customProps); // Set Players custom properties related to game
    void AcknowledgePlayerReadyStatus();                        // Acknowledge Player's Ready status
    GameObject SpawnPlayer(string prefabPath);                  // Spawn Player Game Object in game play scene 
    #endregion
}
