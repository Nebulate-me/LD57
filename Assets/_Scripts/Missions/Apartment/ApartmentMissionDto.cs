using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using _Scripts.Utils;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Missions.Apartment
{
    [Serializable]
    public class ApartmentMissionDto
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<RoomRequirement> requirements;
        [SerializeField] private int requiredWindows = 1;
        [SerializeField] private int rewardScore;
        [FormerlySerializedAs("roomCount")] [SerializeField] private int apartmentRoomCountType = 1;

        public string Name => missionName;
        public List<RoomRequirement> Requirements => requirements;
        public int RequiredWindows => requiredWindows;
        public int RewardScore => rewardScore;
        public int ApartmentRoomCountType => apartmentRoomCountType;

        private const int NO_HALLWAY_BONUS = 5;
        private const int FREE_HALLWAY_TILES_COUNT = 2;
        
        public ApartmentMissionDto(ApartmentMission apartmentMission)
        {
            missionName = apartmentMission.MissionName;
            requirements = apartmentMission.Requirements;
            requiredWindows = apartmentMission.RequiredWindows;
            rewardScore = apartmentMission.RewardScore;
            apartmentRoomCountType = apartmentMission.RoomCount;
        }

        public int CalculateScore(List<DungeonRoomModel> roomsToUse)
        {
            var baseScore = RewardScore;
            var roomScore = roomsToUse.Sum(room => room.Score);
            var usedHallwayTiles = roomsToUse
                .Where(room => room.HasType(RoomTypeExtensions.ConnectingRoomType))
                .Select(room => room.TileCount).ToList();
            var hallwayScore = usedHallwayTiles.IsEmpty()
                ? NO_HALLWAY_BONUS
                : -Mathf.Max(usedHallwayTiles.Count - FREE_HALLWAY_TILES_COUNT, 0);
            var windowScore = roomsToUse.Sum(room => room.WindowCount);
            
            return baseScore + roomScore + hallwayScore + windowScore;
        }
    }
}