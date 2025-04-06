using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Missions
{
    [Serializable]
    public class MissionDto
    {
        [SerializeField] private string name;
        [SerializeField] private List<MissionCell> pattern;
        [SerializeField] private List<RoomDto> rewardCards;
        [SerializeField] private int requiredCompletedMissions;
        [SerializeField] private int rewardScore = 1;

        public MissionDto(Mission mission)
        {
            name = mission.MissionName;
            pattern = mission.Pattern;
            rewardCards = mission.Rewards.Select(reward => reward.ToDto()).ToList();
            requiredCompletedMissions = mission.RequiredCompletedMissions;
            rewardScore = mission.RewardScore;
        }
        
        public string Name => name;
        public List<MissionCell> Pattern => pattern;
        public List<RoomDto> RewardCards => rewardCards;
        public int RequiredCompletedMissions => requiredCompletedMissions;
        public int RewardScore => rewardScore;
    }
}