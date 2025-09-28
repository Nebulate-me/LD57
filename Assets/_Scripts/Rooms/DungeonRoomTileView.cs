using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Mascot;
using _Scripts.Missions.Apartment;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using ModestTree;
using Plugins.Sirenix.Odin_Inspector.Modules;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileView : MonoBehaviour, IDungeonRoomTileView, IPoolableResource
    {
        [SerializeField] private Transform wallSpriteTransform;
        [SerializeField] private SpriteRenderer wallRenderer;
        [Space]
        [SerializeField] private SpriteRenderer floorRenderer;
        [Space]
        [SerializeField] private Transform furnitureSpriteTransform;
        [SerializeField] private SpriteRenderer furnitureRenderer;
        [Space]
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects = new();
        [SerializeField] private RoomDirectionToGameObjectDictionary windowObjects = new();
        [Space]
        [SerializeField] private Color regularWindowColor = Color.white;
        [SerializeField] private Color highlightedWindowColor = Color.red;

        [ShowInInspector, ReadOnly] private Vector2Int gridPosition;
        [ShowInInspector, ReadOnly] private RoomDirection direction;
        [ShowInInspector, ReadOnly] private List<RoomDirection> openDirections = new();
        [ShowInInspector, ReadOnly] private List<RoomDirection> doorDirections = new();
        [ShowInInspector, ReadOnly] private bool _isUsed = false;
        [ShowInInspector, ReadOnly] private bool _isConnected = false;
        [ShowInInspector, ReadOnly] private int _windowCount = 0;

        [Inject] private IDungeonGridManager _dungeonGridManager;
        [Inject] private IRoomRegistry _roomRegistry;

        private RoomTileDto _tileDto;
        private RoomDirectionToDungeonRoomTileViewDictionary _adjacentTiles = new();
        private RoomDirectionToDungeonRoomTileCellDtoDictionary _adjacentGhostTiles = new();
        private RoomDto _roomDto;
        private List<RoomDirection> _potentialGhostTileDirections = new();

        public Vector2Int GridPosition => gridPosition;
        public List<RoomDirection> OpenDirections => openDirections;
        public List<RoomDirection> DoorDirections => doorDirections;
        public int WindowCount => _windowCount;
        public RoomDto RoomDto => _roomDto;

        public bool IsUsed
        {
            get => _isUsed;
            set
            {
                _isUsed = value;
                UpdateWallSprite();
                UpdateDoors();
            }
        }

        public bool IsConnected
        {
            set
            {
                _isConnected = value;
                UpdateWallSprite();
            }
        }

        public RoomFloorColor FloorSprite
        {
            set => floorRenderer.sprite = _roomRegistry.GetRoomFloorSprite(value);
        }

        public Color WindowColor
        {
            set
            {
                foreach (var windowObject in windowObjects.Values)
                {
                    if (windowObject !=null) windowObject.GetComponent<SpriteRenderer>().color = value;
                }
            }
        }

        public void SetUp(RoomTileCellDto roomTileCell, Vector2Int initialGridPosition, Sprite roomFloorSprite,
            RoomDto selectedRoomDto)
        {
            _tileDto = roomTileCell.Tile;
            _roomDto = selectedRoomDto;
            gridPosition = initialGridPosition;
            direction = roomTileCell.Direction;

            floorRenderer.sprite = roomFloorSprite;

            wallSpriteTransform.rotation = direction.ToRotation();
            UpdateWallSprite();
            
            furnitureSpriteTransform.rotation = direction.Rotate(roomTileCell.FurnitureDirection).ToRotation();
            furnitureRenderer.gameObject.SetActive(roomTileCell.FurnitureSprite != null);
            furnitureRenderer.sprite = roomTileCell.FurnitureSprite;
            
            openDirections = _tileDto.OpenDirections;
            doorDirections = _tileDto.DoorDirections;

            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);
            }
            UpdateDoors();

            foreach (var (windowDirection, windowObject) in windowObjects)
            {
                var isWindowActive = _dungeonGridManager.IsTileAdjacentToLevelBounds(gridPosition, windowDirection);
                windowObject.SetActive(isWindowActive);
                if (isWindowActive) _windowCount++;
            }
            UpdateWindows();
            
            SetUpAdjacentTiles();
            UpdatePotentialGhostTileDirections();
        }

        private void SetUpAdjacentTiles()
        {
            var adjacentTiles =
                _dungeonGridManager.RoomTiles.Where(roomTile => roomTile.gridPosition.IsAdjacent(gridPosition));
            foreach (var adjacentTile in adjacentTiles)
            {
                AddAdjacentTile(adjacentTile);
            }
        }

        private void UpdatePotentialGhostTileDirections()
        {
            _potentialGhostTileDirections =
                doorDirections.Where(doorDirection => !_adjacentTiles.ContainsKey(doorDirection)).ToList();
        }
        
        private void UpdateWallSprite()
        {
            if (_isUsed)
            {
                wallRenderer.sprite = _tileDto.UsedSprite;
                return;
            }
            
            wallRenderer.sprite = _isConnected ? _tileDto.ConnectedSprite : _tileDto.UnusedSprite;
        }

        private void UpdateDoors()
        {
            foreach (var doorDirection in doorDirections)
            {
                if (!doorObjects.TryGetValue(doorDirection, out var doorObject) || doorObject == null) continue;
                
                if (_dungeonGridManager.IsTileAdjacentToLevelBounds(gridPosition, doorDirection))
                {
                    doorObject.SetActive(false);
                    continue;
                }

                if (_adjacentTiles.TryGetValue(doorDirection, out var adjacentTile))
                {
                    if (!adjacentTile.doorDirections.Contains(doorDirection.Invert()))
                    {
                        doorObject.SetActive(false);
                        continue;
                    }

                    var isTileShared = HasRoomType(RoomType.Shared);
                    var isAdjacentTileShared = adjacentTile.HasRoomType(RoomType.Shared);
                    var noSharedRoomConnection = !isTileShared && !isAdjacentTileShared;
                    
                    var isConnectingSharedRooms = isTileShared && isAdjacentTileShared;
                    var isApartmentStarting = (isTileShared && adjacentTile.HasAnyRoomType(RoomTypeExtensions.ApartmentStartingRoomTypes)) ||
                                              (isAdjacentTileShared && HasAnyRoomType(RoomTypeExtensions.ApartmentStartingRoomTypes));
                    var isConnectingUsedRooms = noSharedRoomConnection && IsUsed && adjacentTile.IsUsed;
                    var isConnectingUnusedRooms = noSharedRoomConnection && !IsUsed && !adjacentTile.IsUsed;
                    doorObject.SetActive(isConnectingSharedRooms || isApartmentStarting ||  isConnectingUsedRooms || isConnectingUnusedRooms);
                    continue;
                }

                if (!IsUsed && _adjacentGhostTiles.TryGetValue(doorDirection, out var adjacentGhostTileDto))
                {
                    if (!adjacentGhostTileDto.Tile.DoorDirections.Contains(doorDirection.Invert()))
                    {
                        doorObject.SetActive(false);
                        continue;
                    }

                    if (HasRoomType(RoomType.Shared) &&
                        !adjacentGhostTileDto.Room.HasAnyRoomTypes(RoomTypeExtensions.ApartmentStartingRoomTypes))
                    {
                        doorObject.SetActive(false);
                        continue;
                    }
                    
                    doorObject.SetActive(true);
                    continue;
                }

                doorObject.SetActive(!IsUsed);
            }
        }

        private bool HasRoomType(RoomType roomType)
        {
            return _roomDto.HasRoomType(roomType);
        }
        
        private bool HasAnyRoomType(List<RoomType> roomTypes)
        {
            return _roomDto.HasAnyRoomTypes(roomTypes);
        }

        private void UpdateWindows()
        {
            
        }

        public void OnSpawn()
        {
            _isUsed = false;
            _windowCount = 0;
            
            SignalsHub.AddListener<RoomTilePlacedSignal>(OnRoomTilePlaced);
            SignalsHub.AddListener<RoomTileRemovedSignal>(OnRoomTileRemoved);
            SignalsHub.AddListener<DungeonRoomGhostViewMovedSignal>(OnGhostViewMoved);
            SignalsHub.AddListener<ApartmentMissionCompletedSignal>(OnMissionCompleted);
            SignalsHub.AddListener<HighlightWindowsSignal>(OnHighlightWindows);
            SignalsHub.AddListener<UnhighlightWindowsSignal>(OnUnhighlightWindows);
        }

        public void OnDespawn()
        {
            _isUsed = false;
            _tileDto = null;
            _adjacentTiles = new RoomDirectionToDungeonRoomTileViewDictionary();
            _adjacentGhostTiles = new RoomDirectionToDungeonRoomTileCellDtoDictionary();
            WindowColor = regularWindowColor;

            SignalsHub.RemoveListener<RoomTilePlacedSignal>(OnRoomTilePlaced);
            SignalsHub.RemoveListener<RoomTileRemovedSignal>(OnRoomTileRemoved);
            SignalsHub.RemoveListener<DungeonRoomGhostViewMovedSignal>(OnGhostViewMoved);
            SignalsHub.RemoveListener<ApartmentMissionCompletedSignal>(OnMissionCompleted);
            SignalsHub.RemoveListener<HighlightWindowsSignal>(OnHighlightWindows);
            SignalsHub.RemoveListener<UnhighlightWindowsSignal>(OnUnhighlightWindows);
        }

        private void OnMissionCompleted(ApartmentMissionCompletedSignal signal)
        {
            UpdateDoors();
        }

        private void OnRoomTilePlaced(RoomTilePlacedSignal signal)
        {
            if (!signal.Room.GridPosition.IsAdjacent(GridPosition)) return;

            AddAdjacentTile(signal.Room);
            UpdatePotentialGhostTileDirections();
            UpdateDoors();
        }
        
        private void OnRoomTileRemoved(RoomTileRemovedSignal signal)
        {
            if (!signal.RoomTile.GridPosition.IsAdjacent(GridPosition)) return;

            RemoveAdjacentTile(signal.RoomTile);
            UpdatePotentialGhostTileDirections();
            UpdateDoors();
        }
        
        private void OnGhostViewMoved(DungeonRoomGhostViewMovedSignal signal)
        {
            if (_isUsed || (_potentialGhostTileDirections.IsEmpty() && _adjacentGhostTiles.IsEmpty())) return;

            if (!signal.IsValid)
            {
                _adjacentGhostTiles.Clear();
                UpdateDoors();
                return;
            }
            
            foreach (var doorDirection in _potentialGhostTileDirections)
            {
                var adjacentPosition = gridPosition + doorDirection.ToVector2Int();
                if (signal.RoomDto.TryGetTile(adjacentPosition, out var adjacentGhostTile))
                {
                    _adjacentGhostTiles[doorDirection] = adjacentGhostTile;
                }
                else
                {
                    _adjacentGhostTiles.Remove(doorDirection);
                }
            }
            UpdateDoors();
        }
        
        private void OnUnhighlightWindows(UnhighlightWindowsSignal signal)
        {
            WindowColor = regularWindowColor;
        }

        private void OnHighlightWindows(HighlightWindowsSignal signal)
        {
            WindowColor = highlightedWindowColor;
        }

        private void AddAdjacentTile(DungeonRoomTileView dungeonRoomTileView)
        {
            var roomDirection = (dungeonRoomTileView.GridPosition - GridPosition).FromVector2Int();
            _adjacentTiles[roomDirection] = dungeonRoomTileView;
            _adjacentGhostTiles.Remove(roomDirection);
        }

        private void RemoveAdjacentTile(DungeonRoomTileView dungeonRoomTileView)
        {
            var roomDirection = (dungeonRoomTileView.GridPosition - GridPosition).FromVector2Int();
            _adjacentTiles.Remove(roomDirection);
            _adjacentGhostTiles.Remove(roomDirection);
        }
    }

    [Serializable]
    public class RoomDirectionToDungeonRoomTileViewDictionary : UnitySerializedDictionary<RoomDirection, DungeonRoomTileView>
    {
    }
    
    [Serializable]
    public class RoomDirectionToDungeonRoomTileCellDtoDictionary : UnitySerializedDictionary<RoomDirection, RoomTileCellDto>
    {
    }
}