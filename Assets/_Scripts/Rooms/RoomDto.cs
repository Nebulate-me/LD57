using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Rooms
{
    [Serializable]
    public class RoomDto
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite sprite;
        [SerializeField] private List<RoomDirection> openDirections;
        public RoomDto(Room room)
        {
            name = room.RoomName;
            sprite = room.Sprite;
            openDirections = room.OpenDirections;
        }
        
        public Sprite Sprite => sprite;
        public IReadOnlyList<RoomDirection> OpenDirections => openDirections;
        public string Name => name;
    }
}