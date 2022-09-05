using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Lucky4u.PhotonMultiplayer
{
    public class PUNRoomEventsHelper
    {
        public void SendEvent(byte eventCode, object[] eventParams, ReceiverGroup receiverGroup = ReceiverGroup.All)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = receiverGroup };
            PhotonNetwork.RaiseEvent(eventCode, eventParams, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
}