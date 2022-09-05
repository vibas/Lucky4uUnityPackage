using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;

namespace Lucky4u.PhotonMultiplayer
{
	public class PUNRoomPropertyHelper
	{
		#region Public Methods

		public void UpdateRoomCustomProperty(Hashtable roomPropertis)
		{
			foreach (var item in roomPropertis)
			{
				UpdateRoomCustomValues(item.Key, item.Value);
			}
			UpdateRoomProperties();
		}

		public void RemoveRoomCustomProperty(object key)
		{
			Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

			if (roomProperties.ContainsKey(key))
			{
				roomProperties.Remove(key);
				PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
			}
			else
			{
				Debug.LogError("Key not found in room property for removing");
			}
		}

		public void ClearRoomCustomProperties()
		{
			PhotonNetwork.CurrentRoom.CustomProperties.Clear();
			PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
		}

		public Hashtable GetRoomProperties()
		{
			return PhotonNetwork.CurrentRoom.CustomProperties;
		}
		#endregion

		#region Private Methods

		bool TryGetRoomProperty(string key, out object value)
		{
			return PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out value);
		}

		bool RoomPropertyHasKey(string key)
		{
			return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
		}

		void UpdateRoomCustomValues(object key, object value)
		{
			if (RoomPropertyHasKey(key.ToString()))
			{
				PhotonNetwork.CurrentRoom.CustomProperties[key] = value;
			}
			else
			{
				PhotonNetwork.CurrentRoom.CustomProperties.Add(key, value);
			}
		}
		void UpdateRoomProperties()
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
		}
		#endregion
	}
}