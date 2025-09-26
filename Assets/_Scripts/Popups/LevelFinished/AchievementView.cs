using _Scripts.Achievements;
using _Scripts.Utils;
using TMPro;
using UnityEngine;

namespace _Scripts.Popups.LevelFinished
{
    public class AchievementView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI points;

        public void SetUp(Achievement achievement)
        {
            title.text = achievement.Title;
            description.text = achievement.Description;
            points.text = $"{achievement.Points} {achievement.Points.DeclinePoints()}";
        }
    }
}