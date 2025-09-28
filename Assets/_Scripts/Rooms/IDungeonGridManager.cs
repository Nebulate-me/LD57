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
        public List<DungeonRoomModel> GetAdjacentRooms(IEnumerable<IDungeonRoomTileView> selectedRoomTiles);
        bool IsTileAdjacentToLevelBounds(Vector2Int gridPosition, RoomDirection key);

        bool IsTileAdjacentDoor(Vector2Int gridPosition, RoomDirection direction,
            out DungeonRoomTileView adjacentTile);
        bool IsTileAdjacentToDoorOrEmpty(Vector2Int gridPosition, RoomDirection direction, out DungeonRoomTileView adjacentTile);
        Vector2Int WorldToGrid(Vector2 transformPosition);
        void UnloadLevel();
        void LoadLevel(Level level);

        void ShowTutorialGhostRoom(Vector2Int gridPosition, RoomDto roomDto, RoomDirection direction);
        void HideTutorialGhostRoom();
    }
}