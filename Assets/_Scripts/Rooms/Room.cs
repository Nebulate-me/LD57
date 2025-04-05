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

        public string RoomName => roomName;
        public Sprite Sprite => sprite;
        public List<RoomDirection> OpenDirections => openDirections;

        public RoomDto ToCard()
        {
            return new RoomDto(this);
        }
    }
}