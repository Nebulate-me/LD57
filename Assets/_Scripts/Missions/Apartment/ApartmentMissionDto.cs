using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Missions.Apartment
{
    [Serializable]
    public class ApartmentMissionDto
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<RoomRequirement> requirements;
        [SerializeField] private List<RoomDto> rewardRooms;
        [SerializeField] private int minCompletedMissions = 0;
        [SerializeField] private int maxCompletedMissions = 0; // 0 = no max value here
        [SerializeField] private int rewardScore;
        
        public string Name => missionName;
        public List<RoomRequirement> Requirements => requirements;
        public List<RoomDto> RewardRooms => rewardRooms;
        public int MinCompletedMissions => minCompletedMissions;
        public int MaxCompletedMissions => maxCompletedMissions;
        public int RewardScore => rewardScore;
        
        public ApartmentMissionDto(ApartmentMission apartmentMission)
        {
            missionName = apartmentMission.MissionName;
            requirements = apartmentMission.Requirements;
            rewardRooms = apartmentMission.Rewards.Select(reward => reward.ToDto()).ToList();
            minCompletedMissions = apartmentMission.MinCompletedMissions;
            maxCompletedMissions = apartmentMission.MaxCompletedMissions;
            rewardScore = apartmentMission.RewardScore;
        }

        public List<RoomRequirement> GetRequirementsCopy()
        {
            return Requirements.Select(requirement => new RoomRequirement(requirement)).ToList();
        }
    }
}