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
        [SerializeField] private int rewardScore;
        [SerializeField] private int roomCount = 1;

        public string Name => missionName;
        public List<RoomRequirement> Requirements => requirements;
        public int RequiredWindows => requiredWindows;
        public int RewardScore => rewardScore;
        public int RoomCount => roomCount;
        
        public ApartmentMissionDto(ApartmentMission apartmentMission)
        {
            missionName = apartmentMission.MissionName;
            requirements = apartmentMission.Requirements;
            requiredWindows = apartmentMission.RequiredWindows;
            rewardScore = apartmentMission.RewardScore;
            roomCount = apartmentMission.RoomCount;
        }
    }
}