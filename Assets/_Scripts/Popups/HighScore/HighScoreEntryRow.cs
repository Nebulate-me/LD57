using _Scripts.Score;
using TMPro;
using UnityEngine;

namespace _Scripts.Popups.HighScore
{
    public class HighScoreEntryRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI indexText;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerScore;

        public void SetUp(int index, HighscoreEntry entry)
        {
            indexText.text = index.ToString();
            playerNameText.text = entry.playerName;
            playerScore.text = entry.score.ToString();
        }
    }
}