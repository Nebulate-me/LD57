using _Scripts.Player;
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
        [SerializeField] private Color highlightedColor = Color.yellow;
        [SerializeField] private Color regularColor = Color.white;

        public void SetUp(int index, PlayerProfile entry, bool isHighlighted)
        {
            indexText.text = index.ToString();
            playerNameText.text = entry.Name;
            playerScore.text = entry.Score.ToString();
            
            var color = isHighlighted ? highlightedColor : regularColor;

            indexText.color = color;
            playerNameText.color = color;
            playerScore.color = color;
        }
    }
}