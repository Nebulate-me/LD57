using _Scripts.Achievements;
using _Scripts.Utils;
using Signals;
using TMPro;
using UnityEngine;

namespace _Scripts.Popups.LevelFinished
{
    public class AchievementDetailedView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private TextMeshProUGUI points;
        
        private Achievement _achievement;

        private void OnEnable()
        {
            SignalsHub.AddListener<AchievementViewHoveredSignal>(OnAchievementViewHovered);
            SignalsHub.AddListener<AchievementViewUnhoveredSignal>(OnAchievementViewUnhovered);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<AchievementViewHoveredSignal>(OnAchievementViewHovered);
            SignalsHub.RemoveListener<AchievementViewUnhoveredSignal>(OnAchievementViewUnhovered);
        }
        
        private void OnAchievementViewHovered(AchievementViewHoveredSignal signal)
        {
            Show(signal.Achievement);
        }
        
        private void OnAchievementViewUnhovered(AchievementViewUnhoveredSignal obj)
        {
            if (obj.Achievement == _achievement) Hide();
        }

        private void Show(Achievement achievement)
        {
            _achievement = achievement;
            canvasGroup.alpha = 1;
            title.text = achievement.Title;
            description.text = achievement.Description;
            conditionText.text = achievement.ConditionText;
            points.text = $"{achievement.Points} {achievement.Points.DeclinePoints()}";
        }
        
        private void Hide()
        {
            canvasGroup.alpha = 0;
        }

        private void Start()
        {
            Hide();
        }
    }
}