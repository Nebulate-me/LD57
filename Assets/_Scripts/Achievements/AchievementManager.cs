using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using Signals;
using UnityEngine;

namespace _Scripts.Achievements
{
    public class AchievementManager : MonoBehaviour, IAchievementManager
    {
        [SerializeField] private List<Achievement> allAchievements;

        private readonly List<Achievement> _scoredAchievements = new();
        private readonly List<Achievement> _unlockedAchievements = new();
        private readonly Dictionary<int, int> _completedApartmentRoomCounts = new();
        private readonly Dictionary<RoomType, int> _placedRoomCounts = new();

        private void OnEnable()
        {
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.AddListener<RoomRemovedSignal>(OnRoomRemoved);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnApartmentMissionCompleted);
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.RemoveListener<RoomRemovedSignal>(OnRoomRemoved);
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

        private void OnRoomPlaced(RoomPlacedSignal signal)
        {
            foreach (var roomType in signal.Room.RoomTypes)
            {
                if (_placedRoomCounts.TryGetValue(roomType, out var roomCount))
                {
                    _placedRoomCounts[roomType] = roomCount + 1;
                }
                else
                {
                    _placedRoomCounts.Add(roomType, 1);
                }

                var completedAchievements = allAchievements
                    .Where(a => a.AchievementType == AchievementType.RoomTypeCount
                                && !IsUnlocked(a)
                                && a.RoomType == roomType
                                && a.RequiredRooms <= _placedRoomCounts[roomType]);
                foreach (var completedAchievement in completedAchievements)
                {
                    _unlockedAchievements.Add(completedAchievement);
                }
            }
        }
        
        private void OnRoomRemoved(RoomRemovedSignal signal)
        {
            foreach (var roomType in signal.Room.RoomTypes)
            {
                if (_placedRoomCounts.TryGetValue(roomType, out var roomCount))
                {
                    _placedRoomCounts[roomType] = roomCount - 1;
                }
                else
                {
                    _placedRoomCounts.Add(roomType, 0); // Should never happen
                }

                var uncompletedAchievements = allAchievements
                    .Where(a => a.AchievementType == AchievementType.RoomTypeCount
                                && !IsUnlocked(a)
                                && a.RoomType == roomType
                                && a.RequiredRooms > _placedRoomCounts[roomType]);
                foreach (var uncompletedAchievement in uncompletedAchievements)
                {
                    _unlockedAchievements.Remove(uncompletedAchievement);
                }
            }
        }
        
        public List<Achievement> GetUnlockedAchievements()
        {
            return _unlockedAchievements;
        }

        public int ScoreUnlockedAchievements()
        {
            var unscoredAchievements = _unlockedAchievements.Where(a => !_scoredAchievements.Contains(a)).ToList();
            var unlockedSum = unscoredAchievements.Sum(a => a.Points);
            _scoredAchievements.AddRange(unscoredAchievements);
            
            return unlockedSum;
        }

        private bool IsUnlocked(Achievement a) => _unlockedAchievements.Any(unlockedAchievement => unlockedAchievement.Id == a.Id);
    }
}
