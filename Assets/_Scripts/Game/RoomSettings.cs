using System;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Game
{
    [Serializable]
    public class RoomSettings
    {
        public RoomDto RoomDto { get; set; }
        public Vector2Int GridPosition { get; set; }
        public RoomDirection RoomDirection { get; set; }
        
        public RoomSettings(RoomDto roomDto, Vector2Int gridPosition, RoomDirection direction)
        {
            RoomDto = roomDto;
            GridPosition = gridPosition;
            RoomDirection = direction;
        }
    }
}