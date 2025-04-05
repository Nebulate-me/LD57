using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Missions
{
    [CreateAssetMenu(menuName = "LD57/Create Mission", fileName = "Mission", order = 0)]
    public class Mission : ScriptableObject
    {
        [SerializeField] private string missionName;
        [SerializeField] private List<MissionCell> pattern = new();

        public string MissionName => missionName;
        public List<MissionCell> Pattern => pattern;
    }
}