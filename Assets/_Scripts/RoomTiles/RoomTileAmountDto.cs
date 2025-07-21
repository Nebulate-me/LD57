using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.RoomTiles
{
    [Serializable]
    public class RoomTileAmountDto
    {
        [FormerlySerializedAs("room")] [SerializeField] private RoomTile roomTile;
        [SerializeField] private int amount;

        public RoomTile RoomTile => roomTile;
        public int Amount => amount;
    }
}