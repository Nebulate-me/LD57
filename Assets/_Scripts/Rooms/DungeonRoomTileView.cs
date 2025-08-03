using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Missions.Apartment;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using Plugins.Sirenix.Odin_Inspector.Modules;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileView : MonoBehaviour, IPoolableResource
    {
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects = new();

        [ShowInInspector, ReadOnly] private Vector2Int gridPosition;
        [ShowInInspector, ReadOnly] private RoomDirection direction;
        [ShowInInspector, ReadOnly] private List<RoomDirection> openDirections = new();
        [ShowInInspector, ReadOnly] private List<RoomDirection> doorDirections = new();
        [ShowInInspector, ReadOnly] private bool isUsed = false;

        [Inject] private IDungeonGridManager _dungeonGridManager;

        private RoomTileDto _tileDto;
        private readonly RoomDirectionToDungeonRoomTileViewDictionary _adjacentTiles = new();

        public Vector2Int GridPosition => gridPosition;
        public List<RoomDirection> OpenDirections => openDirections;
        public List<RoomDirection> DoorDirections => doorDirections;

        public bool IsUsed
        {
            get => isUsed;
            set
            {
                isUsed = value;
                spriteRenderer.sprite = isUsed ? _tileDto.UsedSprite : _tileDto.UnusedSprite;
                UpdateDoors();
            }
        }

        public void SetUp(RoomTileDto roomTileDto, Vector2Int initialGridPosition, RoomDirection initialDirection)
        {
            _tileDto = roomTileDto;
            gridPosition = initialGridPosition;
            direction = initialDirection;
            spriteTransform.rotation = direction.ToRotation();
            spriteRenderer.sprite = _tileDto.UnusedSprite;

            openDirections = _tileDto.OpenDirections;
            doorDirections = _tileDto.DoorDirections;

            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);
            }

            var adjacentTiles =
                _dungeonGridManager.RoomTiles.Where(roomTile => roomTile.gridPosition.IsAdjacent(gridPosition));
            foreach (var adjacentTile in adjacentTiles)
            {
                AddAdjacentTile(adjacentTile);
            }

            UpdateDoors();
        }

        private void UpdateDoors()
        {
            foreach (var doorDirection in doorDirections)
            {
                if (_adjacentTiles.TryGetValue(doorDirection, out var adjacentTile))
                {
                    doorObjects[doorDirection]
                        .SetActive(IsUsed && adjacentTile.IsUsed || !IsUsed && !adjacentTile.IsUsed);
                }
                else
                {
                    doorObjects[doorDirection].SetActive(true); // TODO: Check if we're at the Level border
                }
            }
        }

        public void OnSpawn()
        {
            isUsed = false;

            SignalsHub.AddListener<RoomTilePlacedSignal>(OnRoomTilePlaced);
        }

        public void OnDespawn()
        {
            isUsed = false;

            SignalsHub.RemoveListener<RoomTilePlacedSignal>(OnRoomTilePlaced);
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnMissionCompleted);
        }

        private void OnMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            UpdateDoors();
        }

        private void OnRoomTilePlaced(RoomTilePlacedSignal signal)
        {
            if (!signal.Room.GridPosition.IsAdjacent(GridPosition)) return;

            AddAdjacentTile(signal.Room);
            UpdateDoors();
        }

        private void AddAdjacentTile(DungeonRoomTileView dungeonRoomTileView)
        {
            var roomDirection = (dungeonRoomTileView.GridPosition - GridPosition).FromVector2Int();
            _adjacentTiles[roomDirection] = dungeonRoomTileView;
        }
    }

    [Serializable]
    public class RoomDirectionToDungeonRoomTileViewDictionary : UnitySerializedDictionary<RoomDirection, DungeonRoomTileView>
    {
    }
}