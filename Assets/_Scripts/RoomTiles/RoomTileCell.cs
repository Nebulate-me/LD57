using System;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.RoomTiles
{
    [Serializable]
    public class RoomTileCell
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private RoomDirection direction;
        [SerializeField] private RoomTile tile;
        [SerializeField] private Sprite furnitureSprite;

        public Vector2Int Position => position;
        public RoomDirection Direction => direction;
        public RoomTile Tile => tile;
        public Sprite FurnitureSprite => furnitureSprite;

        public RoomTileCellDto ToDto()
        {
            return new RoomTileCellDto(this);
        }
    }
}