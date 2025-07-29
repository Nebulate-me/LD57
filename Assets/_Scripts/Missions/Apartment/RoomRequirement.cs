using System;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Missions.Apartment
{
    [Serializable]
    public class RoomRequirement
    {
        [SerializeField] private RoomType roomType;
        // TODO: other conditions, like room's size, luxurity, etc.
    }
}