using System;
using UnityEngine;

namespace _Scripts.Rooms
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