using System;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Cards
{
    [Serializable]
    public class RoomAmountDto
    {
        [SerializeField] private Room room;
        [SerializeField] private int amount;

        public Room Room => room;
        public int Amount => amount;
    }
}