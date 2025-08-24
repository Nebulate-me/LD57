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

        [ShowInInspector, ReadOnly] public string RoomName => _roomDto.Name;
        [ShowInInspector, ReadOnly] public List<RoomType> RoomTypes => _roomDto.RoomTypes;
        [ShowInInspector, ReadOnly] public int AdjacentRoomCount => _adjacentRooms.Count;
        [HideInInspector] public List<DungeonRoomModel> AdjacentRooms => _adjacentRooms;
        public int Score => _roomTiles.Count;

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
            return HasType(requirement.RoomType);
        }

        public bool HasType(RoomType roomType)
        {
            return RoomTypes.Contains(roomType);
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