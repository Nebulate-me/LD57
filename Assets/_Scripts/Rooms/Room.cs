using System.Collections.Generic;
using _Scripts.Cards;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Rooms
{
    [CreateAssetMenu(menuName = "LD57/Create Room", fileName = "Room", order = 0)]
    public class Room : ScriptableObject
    {
        [SerializeField] private string roomName;
        [FormerlySerializedAs("sprite")] [SerializeField] private Sprite usedSprite;
        [SerializeField] private Sprite unusedSprite;
        [SerializeField] private List<RoomDirection> openDirections = new();

        public string RoomName => roomName;
        public Sprite UsedSprite => usedSprite;
        public Sprite UnusedSprite => unusedSprite;
        public List<RoomDirection> OpenDirections => openDirections;

        public RoomDto ToDto()
        {
            return new RoomDto(this);
        }
    }
}