using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Score
{
    [CreateAssetMenu(menuName = "LD57/Create ScoreRankConfiguration", fileName = "ScoreRank", order = 4)]
    public class ScoreRank : ScriptableObject
    {
        [FormerlySerializedAs("rank")] [SerializeField] private string rankName;
        [SerializeField] private string rankDescription;
        [SerializeField] private int minScore;

        public string RankName => rankName;
        public string RankDescription => rankDescription;
        public int MinScore => minScore;
    }
}