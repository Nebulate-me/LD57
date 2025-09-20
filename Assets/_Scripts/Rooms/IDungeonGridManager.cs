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
        bool CanUndoRoomPlacement { get; }
        
        bool UndoLastRoomPlacement();
        Bounds GetLevelBounds();
        Bounds GetRoomBoundsBasedOnTiles();
        bool IsTileAdjacentToLevelBounds(Vector2Int gridPosition, RoomDirection key);
        bool IsTileAdjacentToDoorOrEmpty(Vector2Int gridPosition, RoomDirection direction, out DungeonRoomTileView adjacentTile);
        Vector2Int WorldToGrid(Vector2 transformPosition);
        void UnloadLevel();
        void LoadLevel(Level level);
    }
}