using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Rooms
{
    [Serializable]
    public class RoomDto
    {
        [SerializeField] private string name;
        [SerializeField] private RoomType roomType;
        [SerializeField] private List<RoomTileCell> tiles;

        public RoomDto(Room room)
        {
            name = room.RoomName;
            roomType = room.RoomType;
            tiles = room.Tiles;
        }
        
        public string Name => name;
        public bool IsRotatable => true;
        public List<RoomTileCell> Tiles => tiles;

        public Vector2Int StartingPosition => tiles.Count <= 0
            ? Vector2Int.zero
            : new Vector2Int(tiles.Min(tile => tile.Position.x), tiles.Min(tile => tile.Position.y));
    }
}