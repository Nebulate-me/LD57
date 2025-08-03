using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IDungeonGridManager
    {
        IReadOnlyList<DungeonRoomTileView> RoomTiles { get; }
        IReadOnlyList<DungeonRoomModel> Rooms { get; }
        Bounds GetRoomBounds();
        Bounds GetRoomBoundsBasedOnTiles();
    }
}