using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using System;
using Photon.Pun;

namespace Lucky4u.PhotonPUNMultiplayer
{
    public class PUNCountdownTimer : MonoBehaviourPunCallbacks
    {
        public const string CountdownStartTimeKey = "StartTime";

        [Header("Countdown time in seconds")] 
        [HideInInspector] public float Countdown = 5.0f;

        private bool isTimerRunning;
        private int startTime;

        public static Action OnCountDowsTimerStart;
        public static Action<string> OnCountDownTimerTick;
        public static Action OnCountdownTimerHasExpired;

        public override void OnEnable()
        { 
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }


        public void Update()
        {
            if (!this.isTimerRunning) return;

            float countdown = TimeRemaining();
            string countDownTimeString = countdown.ToString("n0");
            OnCountDownTimerTick?.Invoke(countDownTimeString);

            if (countdown > 0.0f) return;

            OnTimerEnds();
        }


        private void OnTimerRuns()
        {
            this.isTimerRunning = true;
            OnCountDowsTimerStart?.Invoke();
        }

        private void OnTimerEnds()
        {
            this.isTimerRunning = false;

            if (OnCountdownTimerHasExpired != null) OnCountdownTimerHasExpired();
        }


        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
            Initialize();
        }


        private void Initialize()
        {
            int propStartTime;
            if (TryGetStartTime(out propStartTime))
            {
                this.startTime = propStartTime;

                this.isTimerRunning = TimeRemaining() > 0;

                if (this.isTimerRunning)
                    OnTimerRuns();
                else
                    OnTimerEnds();
            }
        }

        private float TimeRemaining()
        {
            int timer = PhotonNetwork.ServerTimestamp - this.startTime;
            return this.Countdown - timer / 1000f;
        }

        public static void SetStartTime()
        {
            int startTimestamp;
            bool startTimeIsSet = TryGetStartTime(out startTimestamp);
            
            if (!startTimeIsSet)
            {
                Hashtable props = new Hashtable
                {
                    {CountdownStartTimeKey, PhotonNetwork.ServerTimestamp}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }

        private static bool TryGetStartTime(out int startTimestamp)
        {
            startTimestamp = PhotonNetwork.ServerTimestamp;

            object startTimeFromProps;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTimeKey, out startTimeFromProps))
            {
                startTimestamp = (int)startTimeFromProps;
                return true;
            }

            return false;
        }
    }
}