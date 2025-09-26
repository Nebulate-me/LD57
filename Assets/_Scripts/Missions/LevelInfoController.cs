using _Scripts.Game;
using _Scripts.Missions.Apartment;
using Signals;
using TMPro;
using UnityEngine;


namespace _Scripts.Missions
{
    public class LevelInfoController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI missionCountText;
        [SerializeField] private TextMeshProUGUI levelNameText;

        private int _missionsToComplete = 0;
        private int _completedMissions = 0;

        private void OnEnable()
        {
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.AddListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.RemoveListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }
        
        private void OnApartmentMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            _completedMissions++;
            UpdateMissionCounterText();
        }
        
        private void OnLevelSetupCompleted(LevelSetupCompletedSignal signal)
        {
            _completedMissions = 0;
            _missionsToComplete = signal.Level.MissionsToComplete;
            levelNameText.text = signal.Level.LevelName;
            UpdateMissionCounterText();
        }

        private void UpdateMissionCounterText()
        {
            missionCountText.text = $"{_completedMissions} / {_missionsToComplete}";
        }
    }
}