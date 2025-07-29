using System.Collections.Generic;
using _Scripts.RoomTiles;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Missions.Pattern
{
    [CreateAssetMenu(menuName = "LD57/Missions/Create PatternMission", fileName = "PatternMission", order = 0)]
    public class PatternMission : ScriptableObject
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<MissionCell> pattern = new();
        [SerializeField] private bool mirrorPatternY;
        [SerializeField] private List<RoomTile> rewards = new();
        [FormerlySerializedAs("requiredCompletedMissions")] [SerializeField] private int minCompletedMissions = 0;
        [SerializeField] private int maxCompletedMissions = 0; // 0 - no max value here
        [SerializeField] private int rewardScore = 1;

        public string MissionName => missionName;
        public List<MissionCell> Pattern => pattern;
        public bool MirrorPatternY => mirrorPatternY;
        public List<RoomTile> Rewards => rewards;
        public int MinCompletedMissions => minCompletedMissions;
        public int MaxCompletedMissions => maxCompletedMissions;
        public int RewardScore => rewardScore;

        public PatternMissionDto ToDto()
        {
            return new PatternMissionDto(this);
        }
    }
}