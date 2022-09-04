using UnityEngine;
using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System.Linq;

namespace Lucky4u.PhotonPUNMultiplayer
{
	public class PUNConnectionManager : MonoBehaviourPunCallbacks, IOnEventCallback, IMultiplayerConnection, IPUNConnection
	{
		#region Events 
		private Action<object> OnRoomJoinEvent;
		private Action<object> OnPlayerEnteredRoomEvent;
		private Action<object> OnRoomPlayerInfoUpdatedEvent;
		private Action<object> OnPlayerLeftRoomEvent;
		private Action<string> OnConnectionStatusChangeEvent;
		private Action OnMaxPlayersJoinedRoomEvent;
		private Action OnAllPlayersReady;
		private Action OnDisconnectedEvent;
		private Action<byte, object> OnEventRaised;
		private Action<ExitGames.Client.Photon.Hashtable> OnRoomPropertiesUpdatedEvent;
		private Action<Player, ExitGames.Client.Photon.Hashtable> OnPlayerPropertiesUpdatedEvent;
		#endregion

		#region Private Fields
		bool isConnecting;
		string gameVersion = "1";

		private byte maxPlayers;

		private PUNRoomPropertyHelper roomPropertiesHelper;
		private PUNRoomEventsHelper roomEventsHelper;
		private PhotonTeamsManager photonTeamsManager;
		#endregion

		#region Properties
		public byte MyTeamCode { get => PhotonNetwork.LocalPlayer.GetPhotonTeam().Code; }
		public byte OtherTeamCode
		{
			get
			{
				byte teamCode = 10;
				if (photonTeamsManager != null)
				{
					PhotonTeam[] photonTeams = photonTeamsManager.GetAvailableTeams();
					PhotonTeam otherTeam = photonTeams.Where(pt => pt.Code != MyTeamCode)
								 .Select(pt => pt)
								 .FirstOrDefault();

					if (otherTeam != null)
						teamCode = otherTeam.Code;
				}
				return teamCode;
			}
		}
		public bool IsMasterClient { get => PhotonNetwork.IsMasterClient; set { } }
		public byte MaxPlayersPerRoom { get => maxPlayers; set => maxPlayers = value; }
		public Dictionary<int, object> JoinedPlayersDict
		{
			get
			{
				Dictionary<int, object> joinedPlayers = new Dictionary<int, object>();
				foreach (var item in PhotonNetwork.CurrentRoom.Players)
				{
					joinedPlayers.Add(item.Key, item.Value);
				}
				return joinedPlayers;
			}
		}
		public bool HaveMaxPlayersJoined => PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom;
		public bool AreAllPlayersReady
		{
			get
			{
				int readyPlayersCount = 0;
				foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
				{
					ExitGames.Client.Photon.Hashtable customProperty = player.Value.CustomProperties;
					if (customProperty != null)
					{
						if (customProperty.ContainsKey("READY"))
						{
							int readyStatus = int.Parse(customProperty["READY"].ToString());
							if (readyStatus == 1)
							{
								readyPlayersCount++;
							}
						}
					}
				}
				return readyPlayersCount >= MaxPlayersPerRoom;
			}
		}
		public int MyPhotonActorNumber { get => PhotonNetwork.LocalPlayer.ActorNumber; set { } }

		Action<object> IMultiplayerConnection.OnRoomJoinEvent { get => OnRoomJoinEvent; set => OnRoomJoinEvent = value; }
		Action<object> IMultiplayerConnection.OnPlayerEnteredRoomEvent { get => OnPlayerEnteredRoomEvent; set => OnPlayerEnteredRoomEvent = value; }
		Action<object> IMultiplayerConnection.OnRoomPlayerInfoUpdatedEvent { get => OnRoomPlayerInfoUpdatedEvent; set => OnRoomPlayerInfoUpdatedEvent = value; }
		Action<object> IMultiplayerConnection.OnPlayerLeftRoomEvent { get => OnPlayerLeftRoomEvent; set => OnPlayerLeftRoomEvent = value; }
		Action<string> IMultiplayerConnection.OnConnectionStatusChangeEvent { get => OnConnectionStatusChangeEvent; set => OnConnectionStatusChangeEvent = value; }
		Action IMultiplayerConnection.OnMaxPlayersJoinedRoomEvent { get => OnMaxPlayersJoinedRoomEvent; set => OnMaxPlayersJoinedRoomEvent = value; }
		Action IMultiplayerConnection.OnAllPlayersReady { get => OnAllPlayersReady; set => OnAllPlayersReady = value; }
		Action IMultiplayerConnection.OnDisconnectedEvent { get => OnDisconnectedEvent; set => OnDisconnectedEvent = value; }
		Action<byte, object> IPUNConnection.OnEventRaised { get => OnEventRaised; set => OnEventRaised = value; }
		Action<ExitGames.Client.Photon.Hashtable> IPUNConnection.OnRoomPropertiesUpdated { get => OnRoomPropertiesUpdatedEvent; set => OnRoomPropertiesUpdatedEvent = value; }
		Action<Player, ExitGames.Client.Photon.Hashtable> IPUNConnection.OnPlayerPropertiesUpdated { get => OnPlayerPropertiesUpdatedEvent; set => OnPlayerPropertiesUpdatedEvent = value; }
		#endregion

		#region MonoBehaviour CallBacks
		void Awake()
		{
			DontDestroyOnLoad(this);
			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.AutomaticallySyncScene = true;
			roomEventsHelper = new PUNRoomEventsHelper();
			roomPropertiesHelper = new PUNRoomPropertyHelper();

			photonTeamsManager = GetComponent<PhotonTeamsManager>();
		}
		public override void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
			OnConnectionStatusChangeEvent += DisplayConnectionStatusLog;
		}
		public override void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			OnConnectionStatusChangeEvent -= DisplayConnectionStatusLog;
		}
		#endregion

		#region Public Methods
		public void SetPlayerName(string playerName)
		{
			PhotonNetwork.NickName = playerName;
		}
		public void Connect()
		{
			// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = true;

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.IsConnected)
			{
				OnConnectionStatusChangeEvent?.Invoke("Joining Room...");
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom();
			}
			else
			{
				OnConnectionStatusChangeEvent?.Invoke("Connecting...");
				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.GameVersion = this.gameVersion;
			}
		}
		public void Disconnect()
		{
			PhotonNetwork.Disconnect();
		}
		public void AcknowledgePlayerReadyStatus()
		{
			RaiseEvents(PhotonPUNMultiplayerConstants.PlayerReadyStatusCode,
						new object[]
						{
						PhotonNetwork.LocalPlayer.ActorNumber
						});
		}
		public void LoadSceneOverNetwork(string sceneName)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.LoadLevel(sceneName);
			}
		}
		public void TryGetPlayerInfo(object playerObj, out int actorNumber, out string playerName)
		{
			Player photonPlayer = (Player)playerObj;
			actorNumber = photonPlayer.ActorNumber;
			playerName = photonPlayer.NickName;
		}

		public void SetPlayerCustomProperties(Dictionary<string, object> customProps)
		{
			ExitGames.Client.Photon.Hashtable customPropertiesPacket = new ExitGames.Client.Photon.Hashtable();
			foreach (var item in customProps)
			{
				customPropertiesPacket.Add(item.Key, item.Value);
			}

			PhotonNetwork.SetPlayerCustomProperties(customPropertiesPacket);
		}

		public GameObject SpawnPlayer(string prefabPath)
		{
			return PhotonNetwork.Instantiate(prefabPath, Vector3.zero, Quaternion.identity);
		}

		public GameObject SpawnPlayerCharacter(GameObject prefab, Transform parentTransform)
		{
			return null;
		}

		public void SetRoomCustomProperties(ExitGames.Client.Photon.Hashtable customProperties)
		{
			roomPropertiesHelper.UpdateRoomCustomProperty(customProperties);
		}

		public void RemoveRoomCustomProperties(object key)
		{
			roomPropertiesHelper.RemoveRoomCustomProperty(key);
		}

		public ExitGames.Client.Photon.Hashtable GetRoomCustomProperties()
		{
			return roomPropertiesHelper.GetRoomProperties();
		}

		public void ClearRoomCustomProperties()
		{
			roomPropertiesHelper.ClearRoomCustomProperties();
		}

		public void RaiseEvents(byte eventCode, object[] data)
		{
			roomEventsHelper.SendEvent(eventCode, data);
		}

		#endregion

		#region Private Methods

		private void DisplayConnectionStatusLog(string logText)
		{
			Debug.LogError($"PUN_LOG: { logText }");
		}

		private void CheckIfMaxPlayersHaveJoinedTheRoom()
		{
			if (HaveMaxPlayersJoined)
			{
				OnMaxPlayersJoinedRoomEvent?.Invoke();
			}
		}

		private void CheckIfAllPlayersAreReady()
		{
			if (HaveMaxPlayersJoined && AreAllPlayersReady)
			{
				OnAllPlayersReady?.Invoke();
			}
		}
		#endregion

		#region PUN CallBacks
		public override void OnConnectedToMaster()
		{
			// we don't want to do anything if we are not attempting to join a room. 
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				OnConnectionStatusChangeEvent?.Invoke("OnConnectedToMaster");

				// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
				PhotonNetwork.JoinRandomRoom();
			}
		}
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			OnConnectionStatusChangeEvent?.Invoke("OnJoinRandomFailed. Next -> Create a new Room");

			// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
			PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.MaxPlayersPerRoom });
		}
		public override void OnDisconnected(DisconnectCause cause)
		{
			OnConnectionStatusChangeEvent?.Invoke("OnDisconnected " + cause);
			isConnecting = false;
			OnDisconnectedEvent?.Invoke();
		}
		public override void OnJoinedRoom()
		{
			OnConnectionStatusChangeEvent?.Invoke("Joined Room " + PhotonNetwork.LocalPlayer.NickName);
			OnRoomJoinEvent?.Invoke(PhotonNetwork.LocalPlayer);
			CheckIfMaxPlayersHaveJoinedTheRoom();
		}
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			OnConnectionStatusChangeEvent?.Invoke("Player Entered Room " + PhotonNetwork.LocalPlayer.NickName);
			OnPlayerEnteredRoomEvent?.Invoke(newPlayer);
			CheckIfMaxPlayersHaveJoinedTheRoom();
		}
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			OnConnectionStatusChangeEvent?.Invoke("OnPlayerLeftRoom " + otherPlayer.NickName);
			OnPlayerLeftRoomEvent?.Invoke(otherPlayer);
		}
		public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
		{
			OnRoomPropertiesUpdatedEvent?.Invoke(propertiesThatChanged);
		}
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
		{
			OnPlayerPropertiesUpdatedEvent?.Invoke(targetPlayer, changedProps);
		}
		public override void OnLeftRoom()
		{
			OnDisconnectedEvent?.Invoke();
		}
		public void OnEvent(EventData photonEvent)
		{
			byte eventCode = photonEvent.Code;
			object eventData = photonEvent.CustomData;
			OnEventRaised?.Invoke(eventCode, eventData);

			switch (eventCode)
			{
				case PhotonPUNMultiplayerConstants.PlayerReadyStatusCode:
					object[] playerReadyData = (object[])photonEvent.CustomData;
					int playerActorNumber = int.Parse(playerReadyData[0].ToString());
					Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerActorNumber);
					if (player != null)
					{
						ExitGames.Client.Photon.Hashtable customProperty = player.CustomProperties;
						if (customProperty.ContainsKey("READY"))
						{
							customProperty["READY"] = 1;
						}
						else
						{
							customProperty.Add("READY", 1);
						}
						player.SetCustomProperties(customProperty);
					}
					CheckIfAllPlayersAreReady();
					break;
			}
		}
		#endregion

	}
}