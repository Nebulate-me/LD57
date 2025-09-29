using System;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Rooms;
using _Scripts.Screens;
using Plugins.Sirenix.Odin_Inspector.Modules;
using Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.Popups.GamePaused
{
    public class GamePausedPopupController : BasePopupController
    {
        protected override PopupType Type => PopupType.GamePaused;
        
        [Header("Button Tooltip")]
        [SerializeField] private TextMeshProUGUI buttonTooltip;
        [SerializeField] private GamePausedPopupButtonTypeToStringDictionary buttonTooltipTexts;
        
        [Inject] private IGameTimerController _gameTimerController;
        [Inject] private IScreenManager _screenManager;
        [Inject] private IScoreManager _scoreManager;
        [Inject] private IDungeonGridManager _dungeonGridManager;

        private void Start()
        {
            OnStart();
            
            buttonTooltip.text = string.Empty;
        }

        protected override void SetupSubscriptions()
        {
            base.SetupSubscriptions();
            
            SignalsHub.AddListener<KeyPressedSignal>(OnKeyPressed);
            SignalsHub.AddListener<GamePausedButtonEvent>(OnGamePausedButtonEvent);
        }

        protected override void DisposeSubscriptions()
        {
            base.DisposeSubscriptions();
            
            SignalsHub.RemoveListener<KeyPressedSignal>(OnKeyPressed);
            SignalsHub.RemoveListener<GamePausedButtonEvent>(OnGamePausedButtonEvent);
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
        
        private void OnGamePausedButtonEvent(GamePausedButtonEvent evt)
        {
            switch (evt.EventType)
            {
                case ButtonEventType.None:
                    break;
                case ButtonEventType.PointerClick:
                    switch (evt.ButtonType)
                    {
                        case GamePausedPopupButtonType.None:
                            break;
                        case GamePausedPopupButtonType.Continue:
                            OnReturnToGame();
                            break;
                        case GamePausedPopupButtonType.MainMenu:
                            OnExitToMainMenu();
                            break;
                        case GamePausedPopupButtonType.NextLevel:
                            OnFinishLevel();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ButtonEventType.PointerEnter:
                    buttonTooltip.text = buttonTooltipTexts.TryGetValue(evt.ButtonType, out var buttonText) ? buttonText : string.Empty;
                    break;
                case ButtonEventType.PointerExit:
                    buttonTooltip.text = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnReturnToGame()
        {
            HidePopup();
            _gameTimerController.ResumeTimer();
        }
        
        private void OnExitToMainMenu()
        {
            _scoreManager.FinishLevel(_dungeonGridManager.EmptyRoomTilesCount);
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

    [Serializable]
    internal class GamePausedPopupButtonTypeToStringDictionary : UnitySerializedDictionary<GamePausedPopupButtonType, string>
    {
    }
}