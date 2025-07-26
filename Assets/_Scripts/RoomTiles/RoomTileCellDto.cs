using System;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.RoomTiles
{
    [Serializable]
    public class RoomTileCellDto
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private RoomDirection direction;
        [SerializeField] private RoomTileDto tile;

        public RoomTileCellDto(RoomTileCell tileCell)
        {
            position = tileCell.Position;
            direction = tileCell.Direction;
            tile = tileCell.Tile.ToDto();
        }

        public RoomTileCellDto(Vector2Int newPosition, RoomDirection newDirection, RoomTileDto newTile)
        {
            position = newPosition;
            direction = newDirection;
            tile = newTile;
        }

        public Vector2Int Position => position;
        public RoomDirection Direction => direction;
        public RoomTileDto Tile => tile;
    }
}