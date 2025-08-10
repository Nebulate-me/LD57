using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomTypeIconView : MonoBehaviour
    {
        [Header("Background")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Sprite unfulfilledBackgroundSprite;
        [SerializeField] private Sprite fulfilledBackgroundSprite;
        
        [Header("Foreground")]
        [SerializeField] private Image roomTypeIcon;

        [SerializeField] private TextMeshProUGUI roomCountText;

        [Inject] private IRoomRegistry _roomRegistry;

        public void SetFulfilled(bool fulfilled)
        {
            backgroundImage.sprite = fulfilled ? fulfilledBackgroundSprite : unfulfilledBackgroundSprite;
        }

        public void SetUp(RoomType roomType)
        {
            roomTypeIcon.sprite = _roomRegistry.GetRoomTypeIcon(roomType);
            roomCountText.text = string.Empty;
            SetFulfilled(false);
        }

        public void SetUp(RoomRequirement roomRequirement)
        {
            roomTypeIcon.sprite = _roomRegistry.GetRoomTypeIcon(roomRequirement.RoomType);
            roomCountText.text = string.Empty;
            SetFulfilled(false);
        }

        public void SetUp(int windowCount)
        {
            roomTypeIcon.sprite = _roomRegistry.GetRoomTypeIcon(RoomType.Window);
            roomCountText.text = windowCount.ToString();
            SetFulfilled(false);
        } 
    }
}