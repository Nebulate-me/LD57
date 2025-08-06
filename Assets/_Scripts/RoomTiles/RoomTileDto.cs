using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using ModestTree;
using UnityEngine;
using Utilities;

namespace _Scripts.RoomTiles
{
    [Serializable]
    public class RoomTileDto
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite usedSprite;
        [SerializeField] private Sprite unusedSprite;
        [SerializeField] private List<RoomDirection> openDirections;
        [SerializeField] private List<RoomDirection> doorDirections;
        [SerializeField] private bool isRotatable;

        public RoomTileDto(RoomTile roomTile, RoomDirection roomDirection = RoomDirection.North)
        {
            name = roomTile.TileName;
            usedSprite = roomTile.UsedSprite;
            unusedSprite = roomTile.UnusedSprite;
            openDirections = roomTile.OpenDirections.Rotate(roomDirection);
            doorDirections = roomTile.DoorDirections.Rotate(roomDirection);
            isRotatable = GetIsRotatable();
        }

        private RoomTileDto(
            RoomTileDto roomTileDto,
            List<RoomDirection> dtoOpenDirections, 
            List<RoomDirection> dtoDoorDirections)
        {
            name = roomTileDto.Name;
            usedSprite = roomTileDto.UsedSprite;
            unusedSprite = roomTileDto.UnusedSprite;

            openDirections = dtoOpenDirections;
            doorDirections = dtoDoorDirections;
            
            isRotatable = GetIsRotatable();
        }

        private bool GetIsRotatable()
        {
            return !openDirections.IsEmpty() &&
                   openDirections.Count != EnumExtensions.GetAllItems<RoomDirection>().Count();
        }

        public string Name => name;
        public Sprite UsedSprite => usedSprite;
        public Sprite UnusedSprite => unusedSprite;
        public List<RoomDirection> OpenDirections => openDirections;
        public List<RoomDirection> DoorDirections => doorDirections;
        public bool IsRotatable => isRotatable;

        public RoomTileDto Rotate(RoomDirection direction)
        {
            return new RoomTileDto(
                this,
                openDirections.Rotate(direction),
                doorDirections.Rotate(direction)
            );
        }
    }
}