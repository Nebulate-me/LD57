using System.Collections.Generic;
using _Scripts.Cards;
using UnityEngine;

namespace _Scripts.Rooms
{
    [CreateAssetMenu(menuName = "LD57/Create Room", fileName = "Room", order = 0)]
    public class Room : ScriptableObject
    {
        [SerializeField] private string roomName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private List<RoomDirection> openDirections = new();
        [SerializeField] private List<RoomDirection> rotations = new(); 

        public string RoomName => roomName;
        public Sprite Sprite => sprite;
        public List<RoomDirection> OpenDirections => openDirections;

        public RoomCard ToCard()
        {
            return new RoomCard(this);
        }
    }
}