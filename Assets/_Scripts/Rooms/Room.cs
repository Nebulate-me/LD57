using System.Collections.Generic;
using System.Linq;
using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Rooms
{
    [CreateAssetMenu(menuName = "LD57/Create Room", fileName = "Room", order = 2)]
    public class Room : ScriptableObject
    {
        [SerializeField] private string roomName;
        [SerializeField] private List<RoomType> roomTypes;
        [SerializeField] private List<RoomTileCell> tiles = new();

        public string RoomName => roomName;
        public List<RoomType> RoomTypes => roomTypes;
        public List<RoomTileCell> Tiles => tiles;

        public RoomDto ToDto()
        {
            return new RoomDto(this);
        }
        
        public bool HasRoomType(RoomType roomType)
        {
            return RoomTypes.Contains(roomType);
        }
        
        public bool HasAnyRoomTypes(List<RoomType> requestedRoomTypes)
        {
            return requestedRoomTypes.Any(HasRoomType);
        }

        public bool IsEqual(RoomDto dto)
        {
            return dto.Name == roomName;
        }

        public bool IsEqual(DungeonRoomModel roomModel)
        {
            return roomModel.RoomName == roomName;
        }
    }
}