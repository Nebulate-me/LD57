using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IRoomRegistry
    {
        Sprite GetRoomTypeIcon(RoomType roomType);
    }
}