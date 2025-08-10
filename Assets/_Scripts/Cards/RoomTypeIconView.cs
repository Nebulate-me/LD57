using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomTypeIconView : MonoBehaviour
    {
        [SerializeField] private Image iconBackground;
        [SerializeField] private Image roomTypeIcon;

        [Inject] private IRoomRegistry _roomRegistry;

        public void SetUp(RoomType roomType)
        {
            roomTypeIcon.sprite = _roomRegistry.GetRoomTypeIcon(roomType);
        }

        public void SetUp(RoomRequirement roomRequirement)
        {
            roomTypeIcon.sprite = _roomRegistry.GetRoomTypeIcon(roomRequirement.RoomType);
        }
    }
}