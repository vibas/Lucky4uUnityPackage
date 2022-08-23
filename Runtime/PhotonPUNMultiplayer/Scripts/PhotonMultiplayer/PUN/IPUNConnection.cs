using System;
using ExitGames.Client.Photon;
public interface IPUNConnection
{
    // ========== EVENTS ============//
    Action<byte, object> OnEventRaised { get; set; }                // When a player raises an event 
    Action<Hashtable> OnRoomPropertiesUpdated { get; set; }    // When room's custom properties updated

    // ========== METHODS ===========//
    void SetRoomCustomProperties(Hashtable customProperties);
    void ClearRoomCustomProperties();
    void RaiseEvents(byte eventCode, object[] data);
}
