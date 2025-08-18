using UnityEngine.Events;

namespace _Scripts.Game.Timer
{
    public interface IGameTimerController
    {
        UnityEvent OnTimerFinished { get; }
        void StartTimer();
    }
}