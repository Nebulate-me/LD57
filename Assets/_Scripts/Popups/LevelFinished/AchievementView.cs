using _Scripts.Achievements;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Popups.LevelFinished
{
    public class AchievementView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI points;
        
        [SerializeField] private Image background;
        [SerializeField] private Sprite unhoveredBackgroundSprite;
        [SerializeField] private Sprite hoveredBackgroundSprite;
        
        private Achievement _achievement;

        public void SetUp(Achievement achievement)
        {
            _achievement = achievement;
            title.text = achievement.Title;
            points.text = $"{achievement.Points}";
            background.sprite = unhoveredBackgroundSprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            background.sprite = hoveredBackgroundSprite;
            SignalsHub.DispatchAsync(new AchievementViewHoveredSignal(_achievement));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background.sprite = unhoveredBackgroundSprite;
            SignalsHub.DispatchAsync(new AchievementViewUnhoveredSignal(_achievement));
        }
    }
}