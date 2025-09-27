using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using _Scripts.Utils;
using Signals;
using UnityEngine;
using Utilities;

namespace _Scripts.Achievements
{
    public class AchievementManager : MonoBehaviour, IAchievementManager
    {
        [SerializeField] private List<Achievement> allAchievements;

        private readonly List<Achievement> _scoredAchievements = new();
        private readonly List<Achievement> _unlockedAchievements = new();
        private readonly Dictionary<int, int> _completedApartmentRoomCounts = new();
        private int _completedHallwaylessApartmentCount = 0;
        private readonly Dictionary<RoomType, int> _placedRoomCounts = new();
        private readonly Dictionary<RoomType, int> _placedRoomsWithWindowsCounts = new();
        private readonly Dictionary<RoomType, int> _placedRoomsWithoutWindowsCounts = new();

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

        private void Start()
        {
            foreach (var roomType in EnumExtensions.GetAllItems<RoomType>())
            {
                _placedRoomCounts.Add(roomType, 0);
                _placedRoomsWithWindowsCounts.Add(roomType, 0);
                _placedRoomsWithoutWindowsCounts.Add(roomType, 0);
            }
        }

        private void OnApartmentMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            var apartmentRoomCountType = signal.Dto.ApartmentRoomCountType;
            IncrementTypeCount(_completedApartmentRoomCounts, apartmentRoomCountType);

            var completedAchievements = allAchievements
                .Where(a => a.AchievementType == AchievementType.ApartmentTypeCount
                            && !IsUnlocked(a)
                            && a.ApartmentRoomCountType == apartmentRoomCountType
                            && a.RequiredApartments <= _completedApartmentRoomCounts[apartmentRoomCountType]);
            foreach (var completedAchievement in completedAchievements)
            {
                _unlockedAchievements.Add(completedAchievement);
            }

            if (signal.RoomsToUse.None(room => room.HasType(RoomType.Hallway))) 
                _completedHallwaylessApartmentCount++;
            
            var completedHallwaylessAchievements = allAchievements
                .Where(a => a.AchievementType == AchievementType.HallwaylessApartmentCount
                            && !IsUnlocked(a)
                            && a.RequiredApartments <= _completedHallwaylessApartmentCount);
            foreach (var completedAchievement in completedHallwaylessAchievements)
            {
                _unlockedAchievements.Add(completedAchievement);
            }
        }

        private void OnRoomPlaced(RoomPlacedSignal signal)
        {
            foreach (var roomType in signal.Room.RoomTypes)
            {
                IncrementTypeCount(_placedRoomCounts, roomType);

                var completedRoomTypeAchievements = allAchievements
                    .Where(a => a.AchievementType == AchievementType.RoomTypeCount
                                && !IsUnlocked(a)
                                && a.RoomType == roomType
                                && a.RequiredRooms <= _placedRoomCounts[roomType]);
                foreach (var completedAchievement in completedRoomTypeAchievements)
                {
                    _unlockedAchievements.Add(completedAchievement);
                }
                
                IncrementTypeCount(
                    signal.Room.WindowCount == 0 ? _placedRoomsWithoutWindowsCounts : _placedRoomsWithWindowsCounts,
                    roomType);
                
                var completedRoomWindowCountAchievements = allAchievements
                    .Where(a => a.AchievementType == AchievementType.RoomTypeWindowCount
                                && !IsUnlocked(a)
                                && a.RoomType == roomType
                                && (a.WithWindows && _placedRoomsWithWindowsCounts[roomType] > 0 || 
                                    !a.WithWindows && _placedRoomsWithoutWindowsCounts[roomType] > 0));
                foreach (var completedAchievement in completedRoomWindowCountAchievements)
                {
                    _unlockedAchievements.Add(completedAchievement);
                }
            }
        }

        private void OnRoomRemoved(RoomRemovedSignal signal)
        {
            foreach (var roomType in signal.Room.RoomTypes)
            {
                DecrementTypeCount(_placedRoomCounts, roomType);

                var uncompletedRoomTypeAchievements = _unlockedAchievements
                    .Where(a => a.AchievementType == AchievementType.RoomTypeCount
                                && !IsUnlocked(a)
                                && a.RoomType == roomType
                                && a.RequiredRooms > _placedRoomCounts[roomType]);
                foreach (var uncompletedAchievement in uncompletedRoomTypeAchievements)
                {
                    _unlockedAchievements.Remove(uncompletedAchievement);
                }
                
                DecrementTypeCount(
                    signal.Room.WindowCount == 0 ? _placedRoomsWithoutWindowsCounts : _placedRoomsWithWindowsCounts,
                    roomType);
                var uncompletedRoomTypeWindowAchievements = _unlockedAchievements
                    .Where(a => a.AchievementType == AchievementType.RoomTypeWindowCount
                                && !IsUnlocked(a)
                                && a.RoomType == roomType
                                && (a.WithWindows && _placedRoomsWithWindowsCounts[roomType] <= 0 || 
                                    !a.WithWindows && _placedRoomsWithoutWindowsCounts[roomType] <= 0));
                foreach (var uncompletedAchievement in uncompletedRoomTypeWindowAchievements)
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
        
        private static void IncrementTypeCount<T>(Dictionary<T, int> roomTypeCountDictionary, T type)
        {
            if (roomTypeCountDictionary.TryGetValue(type, out var roomTypeCount))
            {
                roomTypeCountDictionary[type] = roomTypeCount + 1;
            }
            else
            {
                roomTypeCountDictionary.Add(type, 1);
            }
        }
        
        private static void DecrementTypeCount<T>(Dictionary<T, int> roomTypeCountDictionary, T type)
        {
            if (roomTypeCountDictionary.TryGetValue(type, out var roomTypeCount))
            {
                roomTypeCountDictionary[type] = roomTypeCount - 1;
            }
            else
            {
                roomTypeCountDictionary.Add(type, 0);
            }
        }

        private static void SetTypeCount<T>(Dictionary<T, int> roomTypeCountDictionary, T type, int value)
        {
            if (roomTypeCountDictionary.TryGetValue(type, out _))
            {
                roomTypeCountDictionary[type] = value;
            }
            else
            {
                roomTypeCountDictionary.Add(type, value);
            }
        }
    }
}
