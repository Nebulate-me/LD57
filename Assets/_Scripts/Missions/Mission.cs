using System.Collections.Generic;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Missions
{
    [CreateAssetMenu(menuName = "LD57/Create Mission", fileName = "Mission", order = 0)]
    public class Mission : ScriptableObject
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<MissionCell> pattern = new();
        [SerializeField] private List<Room> rewards = new();
        [SerializeField] private int requiredCompletedMissions = 0;

        public string MissionName => missionName;
        public List<MissionCell> Pattern => pattern;
        public List<Room> Rewards => rewards;
        public int RequiredCompletedMissions => requiredCompletedMissions;

        public MissionDto ToDto()
        {
            return new MissionDto(this);
        }
    }
}