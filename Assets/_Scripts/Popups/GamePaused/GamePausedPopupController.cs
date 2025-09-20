using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Rooms;
using _Scripts.Screens;
using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Popups.GamePaused
{
    public class GamePausedPopupController : BasePopupController
    {
        protected override PopupType Type => PopupType.GamePaused;
        
        [Header("Buttons")]
        [SerializeField] private Button returnToGameButton;
        [SerializeField] private Button exitToMainMenuButton;
        [SerializeField] private Button finishLevelButton;
        
        [Inject] private IGameTimerController _gameTimerController;
        [Inject] private IScreenManager _screenManager;
        [Inject] private IScoreManager _scoreManager;
        [Inject] private IDungeonGridManager _dungeonGridManager;
        
        private void Start()
        {
            OnStart();
        }

        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();
            
            SignalsHub.AddListener<KeyPressedSignal>(OnKeyPressed);
            
            returnToGameButton.onClick.AddListener(OnReturnToGame);
            exitToMainMenuButton.onClick.AddListener(OnExitToMainMenu);
            finishLevelButton.onClick.AddListener(OnFinishLevel); 
        }

        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();
            
            SignalsHub.RemoveListener<KeyPressedSignal>(OnKeyPressed);
            
            returnToGameButton.onClick.RemoveListener(OnReturnToGame);
            exitToMainMenuButton.onClick.AddListener(OnExitToMainMenu);
            finishLevelButton.onClick.AddListener(OnFinishLevel); 
        }

        protected override void OnShowPopup()
        {
            base.OnShowPopup();
            
            _gameTimerController.PauseTimer();
        }
        
        private void OnKeyPressed(KeyPressedSignal signal)
        {
            if (signal.KeyCode == KeyCode.Escape && IsShown)
            {
                OnReturnToGame();
            }
        }

        private void OnReturnToGame()
        {
            HidePopup();
            _gameTimerController.StartTimer();
        }
        
        private void OnExitToMainMenu()
        {
            _screenManager.GoToMainMenuScreen();
        }
        
        private void OnFinishLevel()
        {
            HidePopup();
            _scoreManager.FinishLevel(_dungeonGridManager.EmptyRoomTilesCount);
            // TODO: Some animation to show how every empty or unused tile contributes to negative score
            SignalsHub.DispatchAsync(new ShowPopupSignal(PopupType.LevelFinished));
        }
    }
}