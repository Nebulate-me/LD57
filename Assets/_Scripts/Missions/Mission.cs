using UnityEngine;

namespace _Scripts.Missions
{
    [CreateAssetMenu(menuName = "LD57/Create Mission", fileName = "Mission", order = 0)]
    public class Mission : ScriptableObject
    {
        [SerializeField] private string missionName;
        [SerializeField] private MissionCell[][] pattern = {};
    }
}