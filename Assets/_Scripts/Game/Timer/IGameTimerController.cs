using UnityEngine.Events;

namespace _Scripts.Game.Timer
{
    public interface IGameTimerController
    {
        UnityEvent OnTimerFinished { get; }
        public bool IsRunning { get; }
        void StartTimer();
        void PauseTimer();
    }
}