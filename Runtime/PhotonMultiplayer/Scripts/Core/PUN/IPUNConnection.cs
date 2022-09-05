using System;
using ExitGames.Client.Photon;

namespace Lucky4u.PhotonMultiplayer
{
    public interface IPUNConnection
    {
        // ========== EVENTS ============//
        Action<byte, object> OnEventRaised { get; set; }            // When a player raises an event 
        Action<Hashtable> OnRoomPropertiesUpdated { get; set; }     // When room's custom properties updated
        Action<Photon.Realtime.Player, Hashtable> OnPlayerPropertiesUpdated { get; set; }   // When any player's prperties updated
                                                                                            // ========== METHODS ===========//
        void SetRoomCustomProperties(Hashtable customProperties);
        Hashtable GetRoomCustomProperties();
        void RemoveRoomCustomProperties(object key);
        void ClearRoomCustomProperties();
        void RaiseEvents(byte eventCode, object[] data);
    }
}