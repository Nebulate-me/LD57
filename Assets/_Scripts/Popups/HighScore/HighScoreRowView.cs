using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Popups.HighScore
{
    public class HighScoreRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image background;

        public void SetUp(int rank, string playerName, int score, Color nameColor, Color backgroundColor)
        {
            if (rankText) rankText.text = rank.ToString();
            if (nameText)
            {
                nameText.text = playerName;
                nameText.color = nameColor;
            }

            if (scoreText) scoreText.text = score.ToString();

            if (background) background.color = backgroundColor;
        }
    }
}