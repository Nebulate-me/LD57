using _Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomTypeIconView : MonoBehaviour
    {
        [SerializeField] private Image roomTypeImage;

        [Inject] private IRoomRegistry _roomRegistry;

        public void SetUp(RoomType roomType)
        {
            roomTypeImage.sprite = _roomRegistry.GetRoomTypeIcon(roomType);
        }
    }
}