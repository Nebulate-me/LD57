using UnityEngine.Events;

namespace _Scripts.Game.Timer
{
    public interface IGameTimerController
    {
        UnityEvent OnTimerFinished { get; }
        bool IsRunning { get; }
        bool PauseLockedByTutorial { get; set; }
        void StartTimer();
        void PauseTimer(bool isTutorial = false);
        void ResumeTimer(bool isTutorial = false);
    }
}