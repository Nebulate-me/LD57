using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IDungeonRoomTileView
    {
        Vector2Int GridPosition { get; }
        List<RoomDirection> DoorDirections { get; }
    }
}