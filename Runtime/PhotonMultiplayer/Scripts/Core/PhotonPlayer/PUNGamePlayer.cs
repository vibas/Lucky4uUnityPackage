using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;

namespace Lucky4u.PhotonMultiplayer
{
    public class PUNGamePlayer : MonoBehaviour, IPhotonPlayer
    {
        public int PhotonPlayerID { get => photonView.ViewID; }
        public bool IsMine { get => photonView.IsMine; }
        private Action<IPhotonPlayer> onDestroyedEvent;
        public Action<IPhotonPlayer> OnDestroyedEvent
        {
            get => onDestroyedEvent;
            set => onDestroyedEvent = value;
        }

        private Action<int, Dictionary<string, object>> onDataReceived;
        public Action<int, Dictionary<string, object>> OnDataReceived
        {
            get => onDataReceived;
            set => onDataReceived = value;
        }

        PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        private void OnDestroy()
        {
            OnDestroyedEvent?.Invoke(this);
        }

        public void SendData(int methodID, Dictionary<string, object> _dataDict, RpcTarget rpcTarget = RpcTarget.All)
        {
            ExitGames.Client.Photon.Hashtable data = new ExitGames.Client.Photon.Hashtable();
            data.Add(PUNConstants.RPC_CALLER_ID, photonView.ViewID);
            data.Add("MethodID", methodID);
            foreach (KeyValuePair<string, object> item in _dataDict)
            {
                data.Add(item.Key, item.Value);
            }

            if (photonView.IsMine)
                photonView.RPC("ReceivedData", rpcTarget, data);
        }

        [PunRPC]
        void ReceivedData(ExitGames.Client.Photon.Hashtable data)
        {
            int methodID = -1;
            Dictionary<string, object> _dataDict = new Dictionary<string, object>();
            if (data.ContainsKey("MethodID"))
                methodID = (int)data["MethodID"];

            foreach (var item in data)
            {
                _dataDict.Add(item.Key.ToString(), item.Value);
            }
            OnDataReceived?.Invoke(methodID, _dataDict);
        }
    }
}