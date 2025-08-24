using System;
using System.Collections.Generic;
using _Scripts.Missions;
using _Scripts.Missions.Apartment;
using _Scripts.Popups.HighscoreTable;
using _Scripts.Rooms;
using ModestTree;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Scripts.Game
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        [SerializeField] private List<Level> levels = new();

        [ShowInInspector, ReadOnly] private int _currentLevelIndex = 0;
        [ShowInInspector, ReadOnly] private int _currentLevelCompletedMissionsCount = 0;
            
        [Inject] private IDungeonGridManager _dungeonGridManager;
        [Inject] private IScoreManager _scoreManager;

        private void OnEnable()
        {
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
        }

        private void OnApartmentMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            if (!TryGetCurrentLevel(out var currentLevel)) return;
            _currentLevelCompletedMissionsCount++;
            if (_currentLevelCompletedMissionsCount < currentLevel.MissionsToComplete) return;
            
            _scoreManager.FinishLevel(_dungeonGridManager.EmptyRoomTilesCount); // TODO: Some animation to show how every empty or unused tile contributes to negative score
            // TODO: Level Completed Popup
            _currentLevelIndex++;
            LoadCurrentLevel();
        }

        public void Start()
        {
            if (levels.IsEmpty()) throw new Exception("Levels array must not be empty!");
            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            if (!TryGetCurrentLevel(out var level))
            {
                SignalsHub.DispatchAsync(new GameFinishedSignal(GameFinishedReason.AllLevelsCompleted));
                return;
            }
            
            _dungeonGridManager.UnloadLevel();
            _dungeonGridManager.LoadLevel(level);
            _currentLevelCompletedMissionsCount = 0;
        }

        private bool TryGetCurrentLevel(out Level level)
        {
            if (_currentLevelIndex < 0 || _currentLevelIndex >= levels.Count)
            {
                level = null;
                return false;
            }

            level = levels[_currentLevelIndex];
            return true;
        }
    }
}