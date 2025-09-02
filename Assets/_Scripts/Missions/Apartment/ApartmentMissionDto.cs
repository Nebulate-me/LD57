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
        [SerializeField] private int requiredWindows = 1;
        [SerializeField] private int minCompletedMissions = 0;
        [SerializeField] private int maxCompletedMissions = 0; // 0 => no max value here
        [SerializeField] private int rewardScore;
        
        public string Name => missionName;
        public List<RoomRequirement> Requirements => requirements;
        public int RequiredWindows => requiredWindows;
        public int MinCompletedMissions => minCompletedMissions;
        public int MaxCompletedMissions => maxCompletedMissions;
        public int RewardScore => rewardScore;
        
        public ApartmentMissionDto(ApartmentMission apartmentMission)
        {
            missionName = apartmentMission.MissionName;
            requirements = apartmentMission.Requirements;
            requiredWindows = apartmentMission.RequiredWindows;
            minCompletedMissions = apartmentMission.MinCompletedMissions;
            maxCompletedMissions = apartmentMission.MaxCompletedMissions;
            rewardScore = apartmentMission.RewardScore;
        }
    }
}