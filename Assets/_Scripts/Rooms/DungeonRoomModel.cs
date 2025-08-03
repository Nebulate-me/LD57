using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions.Apartment;
using _Scripts.Utils;
using Sirenix.OdinInspector;

namespace _Scripts.Rooms
{
    [Serializable]
    public class DungeonRoomModel
    {
        private readonly RoomDto _roomDto;
        private readonly List<DungeonRoomTileView> _roomTiles;
        private List<DungeonRoomModel> _adjacentRooms;
        private bool _isUsed;

        public DungeonRoomModel(RoomDto roomDto, List<DungeonRoomTileView> roomTiles, List<DungeonRoomModel> adjacentRooms)
        {
            _roomDto = roomDto;
            _roomTiles = roomTiles;
            _adjacentRooms = adjacentRooms;
            _isUsed = false;
        }
        
        public bool IsUsed
        {
            get => _isUsed;
            set
            {
                _isUsed = value;
                foreach (var roomTile in _roomTiles)
                {
                    roomTile.IsUsed = _isUsed;
                }
            }
        }

        [ShowInInspector, ReadOnly] public string RoomName => _roomDto.Name;
        [ShowInInspector, ReadOnly] public RoomType RoomType => _roomDto.RoomType;
        [ShowInInspector, ReadOnly] public int AdjacentRoomCount => _adjacentRooms.Count;
        public List<DungeonRoomModel> AdjacentRooms => _adjacentRooms;

        public bool IsAdjacent(List<DungeonRoomTileView> otherRoomTiles)
        {
            foreach (var otherRoomTile in otherRoomTiles)
            {
                foreach (var roomTile in _roomTiles)
                {
                    if (roomTile.GridPosition.ManhattanDistance(otherRoomTile.GridPosition) != 1) continue;
                    var adjacentDirection = (roomTile.GridPosition - otherRoomTile.GridPosition).FromVector2Int();
                    if (otherRoomTile.OpenDirections.Contains(adjacentDirection) ||
                        otherRoomTile.DoorDirections.Contains(adjacentDirection) ||
                        roomTile.DoorDirections.Contains(adjacentDirection.Invert())) return true;
                }
            }

            return false;
        }

        public void AddAdjacentRoom(DungeonRoomModel room)
        {
            _adjacentRooms.Add(room);
        }

        public bool IsFulfilling(RoomRequirement requirement)
        {
            return requirement.RoomType == RoomType;
        }
    }
}