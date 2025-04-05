using System;
using System.Collections.Generic;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Cards
{
    [Serializable]
    public class RoomCard
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite sprite;
        [SerializeField] private List<RoomDirection> openDirections;
        public RoomCard(Room room)
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