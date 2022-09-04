using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky4u.PhotonPUNMultiplayer
{
    public class MatchmakingScreen : MonoBehaviour
    {
        public Action OnCancelButtonPressedEvent;

        public Transform roomPlayerCardHOlder;
        public List<RoomPlayerCard> roomPlayersCardsList = new List<RoomPlayerCard>();
        public RoomPlayerCard roomPlayerCardPrefab;

        public void CancelButtonPressed()
        {
            ResetScreenElements();
            OnCancelButtonPressedEvent?.Invoke();
        }

        private void OnEnable()
        {
            AppController.Instance.mpConnectionHelper.Connection.OnRoomJoinEvent += HandleOnPlayerJoinedEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnPlayerEnteredRoomEvent += HandleOnPlayerJoinedEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnRoomPlayerInfoUpdatedEvent += HandleOnPlayerJoinedEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnPlayerLeftRoomEvent += HandleOnPlayerLeftEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnDisconnectedEvent += CancelButtonPressed;
        }

        private void OnDisable()
        {
            AppController.Instance.mpConnectionHelper.Connection.OnRoomJoinEvent -= HandleOnPlayerJoinedEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnPlayerEnteredRoomEvent -= HandleOnPlayerJoinedEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnRoomPlayerInfoUpdatedEvent -= HandleOnPlayerJoinedEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnPlayerLeftRoomEvent -= HandleOnPlayerLeftEvent;
            AppController.Instance.mpConnectionHelper.Connection.OnDisconnectedEvent -= CancelButtonPressed;
        }

        public void HandleOnPlayerJoinedEvent(object playerInfo)
        {
            UpdateRoomPlayerUI();
        }
        public void HandleOnPlayerLeftEvent(object playerInfo)
        {
            UpdateRoomPlayerUI();
        }

        void UpdateRoomPlayerUI()
        {
            ClearPlayerCards();

            foreach (var player in AppController.Instance.mpConnectionHelper.Connection.JoinedPlayersDict)
            {
                int userID;
                string userName;
                AppController.Instance.mpConnectionHelper.Connection.TryGetPlayerInfo(player.Value, out userID, out userName);

                RoomPlayerCard card = Instantiate(roomPlayerCardPrefab, roomPlayerCardHOlder);
                card.UpdateCardInfo(userID, userName);
                roomPlayersCardsList.Add(card);
            }
        }

        void ResetScreenElements()
        {
            ClearPlayerCards();
            roomPlayersCardsList.Clear();
        }

        void ClearPlayerCards()
        {
            for (int i = roomPlayersCardsList.Count - 1; i >= 0; i--)
            {
                if (roomPlayersCardsList[i] != null)
                {
                    Destroy(roomPlayersCardsList[i].gameObject);
                }
            }
        }
    }
}