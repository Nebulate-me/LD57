using System;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game.Timer
{
    public class GameTimerController : MonoBehaviour, IGameTimerController
    {
        [Serializable]
        public class IntEvent : UnityEvent<int>
        {
        }

        [Header("Display")] [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private string format = "{0:00}:{1:00}";

        [Header("Timing")] [SerializeField] private float startSeconds = 600f;
        [SerializeField] private bool ignoreTimeScale;

        public UnityEvent OnTimerFinished { get; } = new();

        public float RemainingSeconds { get; private set; }
        public bool IsRunning { get; private set; }
        

        private void Awake()
        {
            ResetTimer();
            UpdateLabel();
        }

        private void Update()
        {
            if (!IsRunning) return;

            var dt = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            RemainingSeconds -= dt;

            if (RemainingSeconds <= 0f)
            {
                RemainingSeconds = 0f;
                IsRunning = false;
                UpdateLabel();
                OnTimerFinished.Invoke();
                return;
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (!timerText) return;

            var minutes = Mathf.FloorToInt(RemainingSeconds / 60f);
            var seconds = Mathf.FloorToInt(RemainingSeconds % 60f);
            timerText.text = string.Format(format, minutes, seconds);
        }

        public void StartTimer()
        {
            if (RemainingSeconds <= 0f) RemainingSeconds = startSeconds;
            IsRunning = true;
            UpdateLabel();
            SignalsHub.DispatchAsync(new GameTimerStartedSignal());
        }

        public void PauseTimer()
        {
            IsRunning = false;
            SignalsHub.DispatchAsync(new GameTimerPausedSignal());
        }

        public void ResumeTimer()
        {
            if (RemainingSeconds > 0f)
            {
                IsRunning = true;
                SignalsHub.DispatchAsync(new GameTimerStartedSignal());
            }
        }

        public void ResetTimer()
        {
            IsRunning = false;
            RemainingSeconds = startSeconds;
            UpdateLabel();
        }

        public void SetSeconds(float seconds)
        {
            RemainingSeconds = Mathf.Max(0f, seconds);
            UpdateLabel();
        }

        public void AddSeconds(float seconds)
        {
            RemainingSeconds = Mathf.Max(0f, RemainingSeconds + seconds);
            UpdateLabel();
        }
    }
}