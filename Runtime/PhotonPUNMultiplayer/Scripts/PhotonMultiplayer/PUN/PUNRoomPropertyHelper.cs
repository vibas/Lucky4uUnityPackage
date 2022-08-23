using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;

public class PUNRoomPropertyHelper
{
	#region Public Methods
	public bool TryGetRoomProperty(string key, out object value)
	{
		return PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out value);
	}

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
		if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key))
		{
			PhotonNetwork.CurrentRoom.CustomProperties.Remove(key);
			PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
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
	#endregion

	#region Private Methods
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