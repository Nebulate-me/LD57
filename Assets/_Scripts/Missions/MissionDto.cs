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
        [SerializeField] private string missionName;
        [SerializeField] private List<MissionCell> pattern;
        [SerializeField] private List<RoomDto> rewardCards;
        [SerializeField] private bool mirrorPattern;

        public MissionDto(Mission mission)
        {
            missionName = mission.MissionName;
            pattern = mission.Pattern;
            rewardCards = mission.Rewards.Select(reward => reward.ToDto()).ToList(); ;
        }
        
        public string MissionName => missionName;
        public List<MissionCell> Pattern => pattern;
        public List<RoomDto> RewardCards => rewardCards;
    }
}