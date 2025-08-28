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
        [SerializeField] private RoomDirection furnitureDirection;
        [SerializeField] private RoomDto room;

        public RoomTileCellDto(RoomTileCell tileCell)
        {
            position = tileCell.Position;
            direction = tileCell.Direction;
            tile = tileCell.Tile.ToDto(direction);
            furnitureSprite = tileCell.FurnitureSprite;
            furnitureDirection = tileCell.FurnitureDirection;
        }
        
        public RoomTileCellDto(Vector2Int newPosition, RoomTileCellDto newTileDto, RoomDto roomDto)
        {
            position = newPosition;
            direction = newTileDto.Direction;
            tile = newTileDto.Tile;
            furnitureSprite = newTileDto.FurnitureSprite;
            furnitureDirection = newTileDto.FurnitureDirection;
            room = roomDto;
        }

        public RoomTileCellDto(Vector2Int newPosition, RoomTileCellDto newTileDto, RoomDirection newDirection,
            RoomDto roomDto)
        {
            position = newPosition;
            direction = newTileDto.Direction.Rotate(newDirection);
            tile = newTileDto.Tile.Rotate(newDirection);
            furnitureSprite = newTileDto.FurnitureSprite;
            furnitureDirection = newTileDto.FurnitureDirection;
            room = roomDto;
        }

        public Vector2Int Position => position;
        public RoomDirection Direction => direction;
        public RoomTileDto Tile => tile;
        public RoomDto Room => room;
        public Sprite FurnitureSprite => furnitureSprite;
        public RoomDirection FurnitureDirection => furnitureDirection;
    }
}