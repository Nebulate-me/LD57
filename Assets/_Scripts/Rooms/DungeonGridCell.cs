using System;
using _Scripts.Cards;
using UnityEngine;

namespace _Scripts.Rooms
{
    [Serializable]
    public class DungeonGridCell
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private RoomDto roomDto;

        public Vector2Int Position => position;
        public RoomDto RoomDto => roomDto;
    }
}