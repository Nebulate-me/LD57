using System.Collections.Generic;
using _Scripts.Rooms;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.RoomTiles
{
    [CreateAssetMenu(menuName = "LD57/Create RoomTile", fileName = "RoomTile", order = 3)]
    public class RoomTile : ScriptableObject
    {
        [FormerlySerializedAs("roomName")] [SerializeField] private string tileName;
        [FormerlySerializedAs("sprite")] [SerializeField] private Sprite usedSprite;
        [SerializeField] private Sprite unusedSprite;
        [SerializeField] private List<RoomDirection> openDirections = new();
        [SerializeField] private List<RoomDirection> doorDirections = new();

        public string TileName => tileName;
        public Sprite UsedSprite => usedSprite;
        public Sprite UnusedSprite => unusedSprite;
        public List<RoomDirection> OpenDirections => openDirections;
        public List<RoomDirection> DoorDirections => doorDirections;

        public RoomTileDto ToDto(RoomDirection direction = RoomDirection.North)
        {
            return new RoomTileDto(this, direction);
        }
    }
}