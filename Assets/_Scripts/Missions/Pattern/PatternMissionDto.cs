using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Missions.Pattern
{
    [Serializable]
    public class PatternMissionDto
    {
        [SerializeField] private string name;
        [SerializeField] private List<MissionCell> pattern;
        [SerializeField] private bool flipPatternY;
        [SerializeField] private List<RoomTileDto> rewardCards;
        [SerializeField] private int minCompletedMissions;
        [SerializeField] private int maxCompletedMissions;
        [SerializeField] private int rewardScore;

        public PatternMissionDto(PatternMission patternMission)
        {
            name = patternMission.MissionName;
            pattern = patternMission.Pattern;
            flipPatternY = patternMission.MirrorPatternY;
            rewardCards = patternMission.Rewards.Select(reward => reward.ToDto()).ToList();
            minCompletedMissions = patternMission.MinCompletedMissions;
            maxCompletedMissions = patternMission.MaxCompletedMissions;
            rewardScore = patternMission.RewardScore * 50;
        }

        public string Name => name;
        public List<MissionCell> Pattern => pattern;
        public bool FlipPatternY => flipPatternY;
        public List<RoomTileDto> RewardCards => rewardCards;
        public int MinCompletedMissions => minCompletedMissions;
        public int MaxCompletedMissions => maxCompletedMissions;
        public int RewardScore => rewardScore;
    }
}