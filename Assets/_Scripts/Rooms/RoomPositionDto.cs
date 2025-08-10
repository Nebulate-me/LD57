using System;
using UnityEngine;

namespace _Scripts.Rooms
{
    [Serializable]
    public class RoomPositionDto
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private RoomDirection direction;
        [SerializeField] private Room room;

        public Vector2Int Position => position;
        public RoomDirection Direction => direction;
        public Room Room => room;
    }
}