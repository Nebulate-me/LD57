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
            string dtoName, 
            Sprite dtoUsedSprite,
            Sprite dtoUnusedSprite,
            List<RoomDirection> dtoOpenDirections, 
            List<RoomDirection> dtoDoorDirections)
        {
            name = dtoName;
            usedSprite = dtoUsedSprite;
            unusedSprite = dtoUnusedSprite;
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
        public IReadOnlyList<RoomDirection> OpenDirections => openDirections;
        public IReadOnlyList<RoomDirection> DoorDirections => doorDirections;
        public bool IsRotatable => isRotatable;

        public RoomTileDto Rotate(RoomDirection direction)
        {
            return new RoomTileDto(
                name,
                usedSprite,
                unusedSprite,
                openDirections.Rotate(direction),
                doorDirections.Rotate(direction)
            );
        }
    }
}