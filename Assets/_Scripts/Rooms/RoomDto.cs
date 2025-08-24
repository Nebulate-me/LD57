using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<RoomType> roomTypes;
        [SerializeField] private List<RoomTileCellDto> tiles;

        public RoomDto(Room room)
        {
            name = room.RoomName;
            roomTypes = room.RoomTypes;
            tiles = room.Tiles.Select(tile => tile.ToDto()).ToList();
        }

        private RoomDto(string name, List<RoomType> roomTypes, List<RoomTileCellDto> tiles)
        {
            this.name = name;
            this.roomTypes = roomTypes;
            this.tiles = tiles;
        }

        public string Name => name;
        public List<RoomType> RoomTypes => roomTypes;
        public List<RoomTileCellDto> Tiles => tiles;

        public Vector2Int StartingPosition => tiles.Count <= 0
            ? Vector2Int.zero
            : new Vector2Int(tiles.Min(tile => tile.Position.x), tiles.Min(tile => tile.Position.y));

        public RoomDto Rotate(RoomDirection direction)
        {
            if (direction == RoomDirection.North) return this;
            
            var rotation = direction.ToRotation();
            var rotatedTiles = tiles.Select(tile =>
                new RoomTileCellDto(
                        (rotation * tile.Position.ToVector3()).ToVector2Int(),
                        tile,
                        direction
                )).ToList();
            
            return new RoomDto(name, roomTypes, rotatedTiles);
        }
        
        public RoomDto Shift(Vector2Int positionShift)
        {
            if (positionShift == Vector2.zero) return this;

            var shiftedTiles = tiles.Select(tile =>
                new RoomTileCellDto(tile.Position + positionShift, tile)).ToList();
            
            return new RoomDto(name, roomTypes, shiftedTiles);
        }

        public Vector2 GetCenter()
        {
            return new Vector2(
                tiles.Select(tile => tile.Position.x).ToList().GetCenter(),
                tiles.Select(tile => tile.Position.y).ToList().GetCenter());
        }

        public bool HasRoomType(RoomType roomType)
        {
            return RoomTypes.Contains(roomType);
        }
        
        public bool HasAnyRoomTypes(List<RoomType> requestedRoomTypes)
        {
            return requestedRoomTypes.Any(HasRoomType);
        }

        public bool TryGetTile(Vector2Int position, out RoomTileCellDto tileCellDto)
        {
            return tiles.TryGetFirst(tile => tile.Position == position, out tileCellDto);
        }
    }
}