using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Utilities;

namespace _Scripts.Rooms
{
    [Serializable]
    public class RoomDto
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite usedSprite;
        [SerializeField] private Sprite unusedSprite;
        [SerializeField] private List<RoomDirection> openDirections;
        private bool isRotatable;

        public RoomDto(Room room)
        {
            name = room.RoomName;
            usedSprite = room.UsedSprite;
            unusedSprite = room.UnusedSprite;
            openDirections = room.OpenDirections;
            isRotatable = !room.OpenDirections.IsEmpty() &&
                          room.OpenDirections.Count != EnumExtensions.GetAllItems<RoomDirection>().Count();
        }
        
        public string Name => name;
        public Sprite UsedSprite => usedSprite;
        public Sprite UnusedSprite => unusedSprite;
        public IReadOnlyList<RoomDirection> OpenDirections => openDirections;
        public bool IsRotatable => isRotatable;
    }
}