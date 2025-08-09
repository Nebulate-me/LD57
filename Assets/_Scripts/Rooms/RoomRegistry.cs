using UnityEngine;

namespace _Scripts.Rooms
{
    public class RoomRegistry : MonoBehaviour, IRoomRegistry
    {
        [SerializeField] private Sprite defaultRoomTypeIcon;
        [SerializeField] private RoomTypeToSpriteDictionary roomTypeSprites;
        
        public Sprite GetRoomTypeIcon(RoomType roomType)
        {
            return roomTypeSprites.TryGetValue(roomType, out var roomIcon) ? roomIcon : defaultRoomTypeIcon;
        }
    }
}