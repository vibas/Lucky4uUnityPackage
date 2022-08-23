﻿using UnityEngine;
using Photon.Pun.UtilityScripts;
using Photon.Pun;

public class MPConnectionHelper
{
    private byte maxPlayersCount = 2;
    public byte MaxPlayersCount { get => maxPlayersCount; set => maxPlayersCount = value; }

    public IMultiplayerConnection Connection;
    private GameObject currentConnectionGO;

    public void InitialiseConnection()
    {
        // PUN Connection Setup
        currentConnectionGO = new GameObject("PUNConnection");
        currentConnectionGO.AddComponent<PlayerNumbering>();
        Connection = currentConnectionGO.AddComponent<PUNConnectionManager>();
        
        // PUN Player events subscription
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;

        if (Connection != null)
        {
            Connection.MaxPlayersPerRoom = MaxPlayersCount;
            Debug.LogError("MAX PLAYER PER ROOM : " + Connection.MaxPlayersPerRoom);
        }

        Connection.OnDisconnectedEvent -= Disconnect;
        Connection.OnDisconnectedEvent += Disconnect;
    }

    void OnPlayerNumberingChanged()
    {
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        if(PhotonNetwork.LocalPlayer.GetPhotonTeam()!=null)
        {
            switch (playerNumber)
            {
                case 0:
                    PhotonNetwork.LocalPlayer.SwitchTeam("Blue");
                    break;
                case 1:
                    PhotonNetwork.LocalPlayer.SwitchTeam("Red");
                    break;
            }
        }
        else
        {
            switch (playerNumber)
            {
                case 0:
                    PhotonNetwork.LocalPlayer.JoinTeam("Blue");
                    break;
                case 1:
                    PhotonNetwork.LocalPlayer.JoinTeam("Red");
                    break;
            }
        }
    }

    /// <summary>
    /// To Check if Multiplayer feature is enabled or not
    /// </summary>
    /// <returns></returns>
    public static bool IsMultiplayerOn()
    {
        return IsPUNMultiplayer();
    }

    /// <summary>
    /// To check if PUN Multiplayer is active or not 
    /// </summary>
    /// <returns></returns>
    public static bool IsPUNMultiplayer()
    {
#if PUN_MULTIPLAYER
        return true;
#else
        return false;
#endif
    }

    private void Disconnect()
    {
        if (currentConnectionGO != null)
        {
            GameObject.Destroy(currentConnectionGO.gameObject);
            currentConnectionGO = null;
        }    

        if(Connection!=null)
        {
            Connection = null;
        }
    }
}