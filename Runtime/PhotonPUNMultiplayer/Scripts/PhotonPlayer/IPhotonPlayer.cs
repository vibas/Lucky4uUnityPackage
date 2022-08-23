using Photon.Pun;
using System;
using System.Collections.Generic;

public interface IPhotonPlayer
{
    Action<IPhotonPlayer> OnDestroyedEvent { get; set; }
    Action<int, Dictionary<string, object>> OnDataReceived { get; set; }
    void SendData(int methodID, Dictionary<string, object> _dataDict, RpcTarget rpcTarget = RpcTarget.All);
}
