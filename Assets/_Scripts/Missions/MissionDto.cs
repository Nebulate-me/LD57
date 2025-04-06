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
        [SerializeField] private bool flipPatternY;
        [SerializeField] private List<RoomDto> rewardCards;
        [SerializeField] private int requiredCompletedMissions;
        [SerializeField] private int rewardScore;

        public MissionDto(Mission mission)
        {
            name = mission.MissionName;
            pattern = mission.Pattern;
            flipPatternY = mission.MirrorPatternY;
            rewardCards = mission.Rewards.Select(reward => reward.ToDto()).ToList();
            requiredCompletedMissions = mission.RequiredCompletedMissions;
            rewardScore = mission.RewardScore * 50;
        }

        public string Name => name;
        public List<MissionCell> Pattern => pattern;
        public bool FlipPatternY => flipPatternY;
        public List<RoomDto> RewardCards => rewardCards;
        public int RequiredCompletedMissions => requiredCompletedMissions;
        public int RewardScore => rewardScore;
    }
}