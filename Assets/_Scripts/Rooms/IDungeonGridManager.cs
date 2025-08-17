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
        bool IsTileAdjacentToLevelBounds(Vector2Int gridPosition, RoomDirection key);
        bool IsTileAdjacentToDoorOrEmpty(Vector2Int tilePosition, RoomDirection doorDirection);
        Vector2Int WorldToGrid(Vector2 transformPosition);
    }
}