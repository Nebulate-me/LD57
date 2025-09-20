using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions.Apartment;
using _Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;

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

        public int WindowCount => _roomTiles.Sum(roomTile => roomTile.WindowCount);
        public int TileCount => _roomTiles.Count;

        [ShowInInspector, ReadOnly] public string RoomName => _roomDto.Name;
        [ShowInInspector, ReadOnly] public List<RoomType> RoomTypes => _roomDto.RoomTypes;
        [ShowInInspector, ReadOnly] public int AdjacentRoomCount => _adjacentRooms.Count;
        [HideInInspector] public List<DungeonRoomModel> AdjacentRooms => _adjacentRooms;
        public int Score => HasType(RoomTypeExtensions.ConnectingRoomType) 
            ? 0 
            : _roomTiles.Count;

        public List<DungeonRoomTileView> Tiles => _roomTiles;

        public bool IsAdjacent(List<DungeonRoomTileView> otherRoomTiles)
        {
            foreach (var otherRoomTile in otherRoomTiles)
            {
                foreach (var roomTile in _roomTiles)
                {
                    if (roomTile.GridPosition.ManhattanDistance(otherRoomTile.GridPosition) != 1) continue;
                    var adjacentDirection = (roomTile.GridPosition - otherRoomTile.GridPosition).FromVector2Int();
                    // FIXME: Ignoring open directions here because we only ever connect rooms via doors
                    if (otherRoomTile.DoorDirections.Contains(adjacentDirection) &&
                        roomTile.DoorDirections.Contains(adjacentDirection.Invert())) return true;
                }
            }

            return false;
        }

        public void AddAdjacentRoom(DungeonRoomModel room)
        {
            _adjacentRooms.Add(room);
        }

        public void RemoveAdjacentRoom(DungeonRoomModel room)
        {
            _adjacentRooms.Remove(room);    
        }

        public bool IsFulfilling(RoomRequirement requirement)
        {
            return HasType(requirement.RoomType);
        }

        public bool HasType(RoomType expectedRoomType)
        {
            return RoomTypes.Contains(expectedRoomType);
        }
        
        public bool HasAnyType(IEnumerable<RoomType> expectedRoomTypes)
        {
            return expectedRoomTypes.Any(HasType);
        }

        public void SetFloorColor(RoomFloorColor apartmentFloorColor)
        {
            foreach (var roomTile in _roomTiles)
            {
                roomTile.FloorSprite = apartmentFloorColor;
            }
        }

        public void ClearTiles(IPrefabPool prefabPool)
        {
            _adjacentRooms.Clear();
            foreach (var roomTile in _roomTiles)
            {
                prefabPool.Despawn(roomTile.gameObject);
            }
        }
    }
}