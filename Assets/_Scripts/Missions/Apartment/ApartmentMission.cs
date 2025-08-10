using System.Collections.Generic;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Missions.Apartment
{
    [CreateAssetMenu(menuName = "LD57/Missions/Create ApartmentMission", fileName = "ApartmentMission", order = 1)]
    public class ApartmentMission : ScriptableObject
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<RoomRequirement> requirements = new();
        [SerializeField] private int requiredWindows = 1;
        [SerializeField] private List<Room> rewards = new();
        [SerializeField] private int minCompletedMissions = 0;
        [SerializeField] private int maxCompletedMissions = 0; // 0 = no max value here
        [SerializeField] private int rewardScore = 1;

        public string MissionName => missionName;
        public List<RoomRequirement> Requirements => requirements;
        public int RequiredWindows => requiredWindows;
        public List<Room> Rewards => rewards;
        public int MinCompletedMissions => minCompletedMissions;
        public int MaxCompletedMissions => maxCompletedMissions;
        public int RewardScore => rewardScore;

        public ApartmentMissionDto ToDto()
        {
            return new ApartmentMissionDto(this);
        }
    }
}