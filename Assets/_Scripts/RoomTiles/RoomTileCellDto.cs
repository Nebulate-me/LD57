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
        [SerializeField] private Sprite furnitureSprite;

        public RoomTileCellDto(RoomTileCell tileCell)
        {
            position = tileCell.Position;
            direction = tileCell.Direction;
            tile = tileCell.Tile.ToDto(direction);
            furnitureSprite = tileCell.FurnitureSprite;
        }

        public RoomTileCellDto(Vector2Int newPosition, RoomTileCellDto newTileDto, RoomDirection newDirection)
        {
            position = newPosition;
            direction = newTileDto.Direction.Rotate(newDirection);
            tile = newTileDto.Tile.Rotate(newDirection);
            furnitureSprite = newTileDto.FurnitureSprite;
        }

        public Vector2Int Position => position;
        public RoomDirection Direction => direction;
        public RoomTileDto Tile => tile;
        public Sprite FurnitureSprite => furnitureSprite;
    }
}