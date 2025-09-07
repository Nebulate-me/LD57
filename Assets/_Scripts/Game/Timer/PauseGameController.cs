using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game.Timer
{
    public class PauseGameController : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Image pauseButtonImage;
        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private Sprite resumeSprite;
        
        [SerializeField] private bool isEnabledOnStart = true;

        [ShowInInspector, ReadOnly] private bool _isEnabled = false;
        [ShowInInspector, ReadOnly] private bool _isPaused = false;
        
        [Inject] private IGameTimerController _gameTimerController;

        private void OnEnable()
        {
            SignalsHub.AddListener<GameTimerPausedSignal>(OnTimerPaused);
            SignalsHub.AddListener<GameTimerStartedSignal>(OnTimerStarted);
            
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<GameTimerPausedSignal>(OnTimerPaused);
            SignalsHub.RemoveListener<GameTimerStartedSignal>(OnTimerStarted);
            
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }

        private void Start()
        {
            SetIsEnabled(isEnabledOnStart);
            SetIsPaused(!_gameTimerController.IsRunning);
        }

        private void OnTimerPaused(GameTimerPausedSignal obj)
        {
            SetIsPaused(true);
        }

        private void OnTimerStarted(GameTimerStartedSignal obj)
        {
            SetIsPaused(false);
        }
        
        private void OnPauseButtonClicked()
        {
            if (!_isEnabled) return;
            if (!_isPaused)
            {
                _gameTimerController.PauseTimer();
            }
            else
            {
                _gameTimerController.StartTimer();
            }
        }

        private void SetIsEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;
            pauseButton.interactable = isEnabled;
        }

        private void SetIsPaused(bool isPaused)
        {
            _isPaused = isPaused;
            pauseButtonImage.sprite = isPaused ? pauseSprite : resumeSprite;
        }
    }
}