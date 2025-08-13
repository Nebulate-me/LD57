using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IRoomRegistry
    {
        Sprite UnusedRoomFloorSprite { get; }
        Sprite SharedRoomFloorSprite { get; }
        
        Sprite GetRoomTypeIcon(RoomType roomType);
        Sprite GetRoomFloorSprite(RoomFloorColor color);
        RoomFloorColor GetUnusedColor();
    }
}