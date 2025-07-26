using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using UnityEngine;
using Utilities;

namespace _Scripts.Rooms
{
    [Serializable]
    public class RoomDto
    {
        [SerializeField] private string name;
        [SerializeField] private RoomType roomType;
        [SerializeField] private List<RoomTileCellDto> tiles;

        public RoomDto(Room room)
        {
            name = room.RoomName;
            roomType = room.RoomType;
            tiles = room.Tiles.Select(tile => tile.ToDto()).ToList();
        }
        
        public string Name => name;
        public RoomType RoomType => roomType;
        public List<RoomTileCellDto> Tiles => tiles;

        public Vector2Int StartingPosition => tiles.Count <= 0
            ? Vector2Int.zero
            : new Vector2Int(tiles.Min(tile => tile.Position.x), tiles.Min(tile => tile.Position.y));

        public List<RoomTileCellDto> RotateTiles(RoomDirection direction)
        {
            if (direction == RoomDirection.North) return tiles;
            var rotation = direction.ToRotation();
            return tiles.Select(tile =>
                new RoomTileCellDto(
                        (rotation * tile.Position.ToVector3()).ToVector2Int(),
                        tile.Direction.Rotate(direction),
                        tile.Tile.Rotate(direction)
                )).ToList();
        }
        
        private List<MissionCell> RotatePattern(IEnumerable<MissionCell> pattern, RoomDirection direction)
        {
            var rotation = direction.ToRotation();
            return pattern.Select(cell => 
                new MissionCell(
                    cell.Type,
                    (rotation * cell.Position.ToVector3()).ToVector2Int(),
                    cell.OpenDirections.Select(openDirection => openDirection.Rotate(direction)).ToList(),
                    cell.ClosedDirections.Select(closedDirection => closedDirection.Rotate(direction)).ToList()
                )).ToList();
        }
    }
}