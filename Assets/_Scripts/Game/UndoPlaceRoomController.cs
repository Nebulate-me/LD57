using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
{
    public class UndoPlaceRoomController : MonoBehaviour
    {
        [SerializeField] private Button undoButton;
        [SerializeField] private int redrawCostScore = 5;
        
        [Space, SerializeField] private bool disablePermanently = false; // FIXME: Not usable in the Tutorial
        
        [ShowInInspector, ReadOnly] private bool _isEnabled = false;
        [ShowInInspector, ReadOnly] private bool _isPaused = false;
        
        [Inject] private IDungeonGridManager _dungeonGridManager;
        [Inject] private IGameTimerController _gameTimerController;
        [Inject] private IScoreManager _scoreManager;
        [Inject] private ISoundManager _soundManager;

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.AddListener<GameTimerStartedSignal>(OnGameTimerStarted);
            SignalsHub.AddListener<GameTimerPausedSignal>(OnGameTimerPaused);
            
            undoButton.onClick.AddListener(OnUndo);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.RemoveListener<GameTimerStartedSignal>(OnGameTimerStarted);
            SignalsHub.RemoveListener<GameTimerPausedSignal>(OnGameTimerPaused);
            
            undoButton.onClick.RemoveListener(OnUndo);
        }

        private void OnRoomPlaced(RoomPlacedSignal signal)
        {
            SetIsEnabled(_dungeonGridManager.CanUndoRoomPlacement);
        }
        
        private void OnApartmentMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            SetIsEnabled(_dungeonGridManager.CanUndoRoomPlacement);
        }
        
        private void OnGameTimerStarted(GameTimerStartedSignal signal)
        {
            SetIsPaused(false);
        }
        
        private void OnGameTimerPaused(GameTimerPausedSignal obj)
        {
            SetIsPaused(true);
        }
        
        private void OnUndo()
        {
            _soundManager.PlaySound(SoundType.UndoPlaceRoom);
            _dungeonGridManager.UndoLastRoomPlacement();
            _scoreManager.SubtractScore(redrawCostScore);
            SetIsEnabled(_dungeonGridManager.CanUndoRoomPlacement);
        }

        private void Start()
        {
            SetIsEnabled(_dungeonGridManager.CanUndoRoomPlacement);
            SetIsPaused(!_gameTimerController.IsRunning);
        }
        
        private void SetIsEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled && !disablePermanently;
            UpdateButtonInteractable();
        }

        private void SetIsPaused(bool isPaused)
        {
            _isPaused = isPaused;
            UpdateButtonInteractable();
        }

        private void UpdateButtonInteractable()
        {
            undoButton.interactable = _isEnabled && !_isPaused;
        }
    }
}