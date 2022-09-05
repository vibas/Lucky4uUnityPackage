using System;
using System.Collections.Generic;

namespace Lucky4u.PhotonMultiplayer
{
    public class PUNConstants
    {
        // =================== RPC =======================//
        // RPC Caller ID
        public const string RPC_CALLER_ID = "SenderID";

        // RPC Keys
        public const string TEST_RPC_KEY = "TestKey";

        // RPC Method IDs
        public const int TEST_RPC_METHOD_ID = 0;
        // ================================================//


        // =============== RAISE EVENTS ====================//
        // Event Code
        public const byte PlayerReadyStatusCode = 1;
        public const byte TestEventCode = 2;
        // =================================================//

    }

    public class PUNEvents
    {
        public static Action OnRoomJoinEvent;
        public static Action OnPlayerEnteredRoomEvent;
        public static Action OnRoomPlayerInfoUpdatedEvent;
        public static Action OnMaxPlayersJoinedRoomEvent;

        public static Action OnGameplaySceneLoadedEvent;

        public static Action<Dictionary<string, object>> OnMyPlayerPropertiesChangedEvent;
        public static Action<Dictionary<string, object>> OnOtherPlayerPropertiesChangedEvent;
        public static Action<Dictionary<string, object>> OnRoomPropertiesUpdatedEvent;
        public static Action<byte, object> OnRaisedEventDataReceived;

        public static Action OnCountDownStartEvent;
        public static Action<string> OnCountDownTimerTickEvent;
        public static Action OnCountDownExpiredEvent;

        public static Action<int> OnPlayerLeftRoomEvent;

        public static Action OnDisconnectedEvent;
    }
}