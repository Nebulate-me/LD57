using UnityEngine;

namespace _Scripts.Missions
{
    [CreateAssetMenu(menuName = "LD57/Create ScoreRankConfiguration", fileName = "ScoreRank", order = 0)]
    public class ScoreRank : ScriptableObject
    {
        [SerializeField] private string rank;
        [SerializeField] private int minScore;

        public string Rank => rank;
        public int MinScore => minScore;
    }
}