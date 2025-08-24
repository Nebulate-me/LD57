using System.Collections.Generic;
using _Scripts.Game;
using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IDungeonGridManager
    {
        IReadOnlyList<DungeonRoomTileView> RoomTiles { get; }
        IReadOnlyList<DungeonRoomModel> Rooms { get; }
        int EmptyRoomTilesCount { get; }
        Bounds GetLevelBounds();
        Bounds GetRoomBoundsBasedOnTiles();
        bool IsTileAdjacentToLevelBounds(Vector2Int gridPosition, RoomDirection key);
        bool IsTileAdjacentToDoorOrEmpty(Vector2Int tilePosition, RoomDirection doorDirection);
        Vector2Int WorldToGrid(Vector2 transformPosition);
        void UnloadLevel();
        void LoadLevel(Level level);
    }
}