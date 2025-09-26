using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions.Apartment;
using Signals;
using UnityEngine;

namespace _Scripts.Achievements
{
    public class AchievementManager : MonoBehaviour, IAchievementManager
    {
        [SerializeField] private List<Achievement> allAchievements;

        private readonly List<Achievement> _unlockedAchievements = new();
        private readonly Dictionary<int, int> _completedApartmentRoomCounts = new();

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
            var apartmentRoomCountType = signal.Dto.ApartmentRoomCountType;
            if (_completedApartmentRoomCounts.TryGetValue(apartmentRoomCountType, out var roomCount))
            {
                _completedApartmentRoomCounts[apartmentRoomCountType] = roomCount + 1;
            }
            else
            {
                _completedApartmentRoomCounts.Add(apartmentRoomCountType, 1);
            }

            var completedAchievements = allAchievements
                .Where(a => a.AchievementType == AchievementType.ApartmentTypeCount
                            && !IsUnlocked(a)
                            && a.ApartmentRoomCountType == apartmentRoomCountType
                            && a.RequiredCompletedApartments <= _completedApartmentRoomCounts[apartmentRoomCountType]);
            foreach (var completedAchievement in completedAchievements)
            {
                _unlockedAchievements.Add(completedAchievement);
            }
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            return _unlockedAchievements;
        }
        private bool IsUnlocked(Achievement a) => _unlockedAchievements.Any(unlockedAchievement => unlockedAchievement.Id == a.Id);
    }
}
