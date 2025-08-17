using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IRoomRegistry
    {
        Sprite UnusedRoomFloorSprite { get; }
        Sprite SharedRoomFloorSprite { get; }
        RoomFloorColor UnusedRoomColor { get; }
        RoomFloorColor HighlightColor { get; }
        
        Sprite GetRoomTypeIcon(RoomType roomType);
        Sprite GetRoomFloorSprite(RoomFloorColor color);
        RoomFloorColor TakeUnusedColor();
    }
}