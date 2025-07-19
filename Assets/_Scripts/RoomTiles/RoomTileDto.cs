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
        private bool isRotatable;

        public RoomTileDto(RoomTile roomTile)
        {
            name = roomTile.TileName;
            usedSprite = roomTile.UsedSprite;
            unusedSprite = roomTile.UnusedSprite;
            openDirections = roomTile.OpenDirections;
            isRotatable = !roomTile.OpenDirections.IsEmpty() &&
                          roomTile.OpenDirections.Count != EnumExtensions.GetAllItems<RoomDirection>().Count();
        }
        
        public string Name => name;
        public Sprite UsedSprite => usedSprite;
        public Sprite UnusedSprite => unusedSprite;
        public IReadOnlyList<RoomDirection> OpenDirections => openDirections;
        public bool IsRotatable => isRotatable;
    }
}