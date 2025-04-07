using System.Collections.Generic;
using _Scripts.Rooms;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Missions
{
    [CreateAssetMenu(menuName = "LD57/Create Mission", fileName = "Mission", order = 0)]
    public class Mission : ScriptableObject
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<MissionCell> pattern = new();
        [SerializeField] private bool mirrorPatternY;
        [SerializeField] private List<Room> rewards = new();
        [FormerlySerializedAs("requiredCompletedMissions")] [SerializeField] private int minCompletedMissions = 0;
        [SerializeField] private int maxCompletedMissions = 0; // 0 - no max value here
        [SerializeField] private int rewardScore = 1;

        public string MissionName => missionName;
        public List<MissionCell> Pattern => pattern;
        public bool MirrorPatternY => mirrorPatternY;
        public List<Room> Rewards => rewards;
        public int MinCompletedMissions => minCompletedMissions;
        public int MaxCompletedMissions => maxCompletedMissions;
        public int RewardScore => rewardScore;

        public MissionDto ToDto()
        {
            return new MissionDto(this);
        }
    }
}