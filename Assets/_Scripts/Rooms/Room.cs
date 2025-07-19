using System.Collections.Generic;
using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Rooms
{
    [CreateAssetMenu(menuName = "LD57/Create Room", fileName = "Room", order = 2)]
    public class Room : ScriptableObject
    {
        [SerializeField] private string roomName;
        [SerializeField] private RoomType roomType;
        [SerializeField] private List<RoomTileCell> tiles = new();

        public string RoomName => roomName;
        public RoomType RoomType => roomType;
        public List<RoomTileCell> Tiles => tiles;
    }
}