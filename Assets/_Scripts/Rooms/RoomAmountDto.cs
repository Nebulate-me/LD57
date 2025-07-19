using System;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Cards
{
    [Serializable]
    public class RoomAmountDto
    {
        [FormerlySerializedAs("room")] [SerializeField] private RoomTile roomTile;
        [SerializeField] private int amount;

        public RoomTile RoomTile => roomTile;
        public int Amount => amount;
    }
}