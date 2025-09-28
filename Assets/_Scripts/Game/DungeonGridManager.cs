using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using ModestTree;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Monads;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Game
{
    public class DungeonGridManager : MonoBehaviour, IDungeonGridManager, IInitializable
    {
        [SerializeField] private Transform roomContainer;
        [FormerlySerializedAs("dungeonRoomPrefab")] [SerializeField] private GameObject dungeonRoomTilePrefab;
        [SerializeField] private GameObject dungeonRoomGhostPrefab;
        [Space]
        [SerializeField] private Vector2 mousePositionOffset;
        [SerializeField] private List<RectTransform> unclickableScreenAreas;
        [SerializeField] private SpriteRenderer levelBuildingBackground;

        [Inject] private IRoomRegistry roomRegistry;
        [Inject] private IHandManager handManager;
        [Inject] private IPrefabPool prefabPool;
        [Inject] private IDungeonCameraController dungeonCameraController;
        [Inject] private ISoundManager soundManager;
        [Inject(Id = "uiCamera")] private Camera uiCamera;

        private DungeonRoomTileGhostView _roomTileGhostInstance;
        private DungeonRoomGhostView _roomGhostInstance;
        private List<DungeonRoomTileView> _roomTiles = new();
        [ShowInInspector, ReadOnly] private List<DungeonRoomModel> _rooms = new();
        [ShowInInspector, ReadOnly] private Stack<DungeonRoomModel> _lastPlacedRooms = new();
        [ShowInInspector, ReadOnly] private RoomDirection _currentDirection = RoomDirectionExtensions.Default;

        private IMaybe<Level> _maybeCurrentLevel = Maybe.Empty<Level>();

        public void Initialize()
        {
            _currentDirection = RoomDirectionExtensions.Default;
            _roomTiles = new List<DungeonRoomTileView>();
            unclickableScreenAreas.Select(RectTransformUtility.CalculateRelativeRectTransformBounds)
                .ToList();
            
            _roomGhostInstance = prefabPool.Spawn(dungeonRoomGhostPrefab, roomContainer)
                .GetComponent<DungeonRoomGhostView>();
        }

        private void Update()
        {
            if (!handManager.SelectedRoomCardView.TryGetValue(out var selectedRoomCardView))
            {
                _roomGhostInstance.gameObject.SetActive(false);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                _roomGhostInstance.gameObject.SetActive(false);
                SignalsHub.DispatchAsync(new DungeonRoomGhostViewMovedSignal(null, isValid: false));
                handManager.DeselectRoomCard();
                return;
            }

            var mouseUI = dungeonCameraController.GetMouseUIPosition();
            if (unclickableScreenAreas.Any(area => RectTransformUtility.RectangleContainsScreenPoint(area, mouseUI, uiCamera)))
            {
                _roomGhostInstance.gameObject.SetActive(false);
                return;
            }
            
            _roomGhostInstance.gameObject.SetActive(true);
            
            if (Input.mouseScrollDelta.y != 0)
                RotateGhostView(Input.mouseScrollDelta.y > 0);

            Vector2 mouseWorld = dungeonCameraController.GetMouseWorldPosition();
            var gridPosition = WorldToGrid(mouseWorld);
            var snappedPosition = GridToWorld(gridPosition);
            // Debug.Log($"Mouse Position {mouseWorld}, gridPosition {gridPosition}, snappedPosition {snappedPosition}");

            _roomGhostInstance.transform.position = snappedPosition;

            if (!IsRoomPositionValid(gridPosition, selectedRoomCardView.RoomDto, _currentDirection))
            {
                _roomGhostInstance.SetUpInvalid(selectedRoomCardView.RoomDto, _currentDirection);
                return;
            }
            
            _roomGhostInstance.SetUpValid(selectedRoomCardView.RoomDto, _currentDirection);

            if (Input.GetMouseButtonDown(0))
                PlaceRoom(selectedRoomCardView.RoomDto, gridPosition);
        }

        /// <summary>
        /// Finds the lists of open/door/closed directions of rooms adjacent to the provided room position
        /// Returns true if there are any adjacent rooms at all
        /// </summary>
        private bool GetAdjacentDirections(Vector2Int roomPosition, 
            out List<RoomDirection> adjacentOpenDirections, 
            out List<RoomDirection> adjacentAnyDirections, 
            out List<RoomDirection> adjacentClosedDirections)
        {
            adjacentOpenDirections = new List<RoomDirection>();
            adjacentAnyDirections = new List<RoomDirection>();
            adjacentClosedDirections = new List<RoomDirection>();

            var adjacentRoomsTiles = _roomTiles.Where(room => room.GridPosition.ManhattanDistance(roomPosition) == 1).ToList();

            if (adjacentRoomsTiles.IsEmpty())
                return false;

            foreach (var adjacentRoomTile in adjacentRoomsTiles)
            {
                var adjacentRoomDirection = (adjacentRoomTile.GridPosition - roomPosition).FromVector2Int();
                var invertedAdjacentRoomDirection = adjacentRoomDirection.Invert();
                if (adjacentRoomTile.OpenDirections.Contains(invertedAdjacentRoomDirection))
                    adjacentOpenDirections.Add(adjacentRoomDirection);
                else if (adjacentRoomTile.DoorDirections.Contains(invertedAdjacentRoomDirection))
                    adjacentAnyDirections.Add(adjacentRoomDirection);
                else
                    adjacentClosedDirections.Add(adjacentRoomDirection);
            }

            return true;
        }

        private bool IsValidAdjacency(TileDirectionConfiguration dtoConfiguration,
            TileDirectionConfiguration adjacentConfiguration)
        {
            var allOpenDirectionsOpen =
                adjacentConfiguration.OpenDirections.All(openDirection => dtoConfiguration.OpenDirections.Contains(openDirection) || dtoConfiguration.AnyDirections.Contains(openDirection));
            var allClosedDirectionsClosed =
                adjacentConfiguration.ClosedDirections.All(closedDirection => dtoConfiguration.ClosedDirections.Contains(closedDirection) || dtoConfiguration.AnyDirections.Contains(closedDirection) );

            return allOpenDirectionsOpen && allClosedDirectionsClosed;
        }

        private bool IsRoomPositionValid(Vector2Int gridPosition, RoomDto roomDto, RoomDirection roomDirection)
        {
            var rotatedRoomDto = roomDto.Rotate(roomDirection);
            var roomStartingPosition = rotatedRoomDto.StartingPosition;
            var roomPositions = rotatedRoomDto.Tiles
                .Select(tile => tile.Position - roomStartingPosition + gridPosition).ToList();
            
            if (!roomPositions.All(IsPositionEmpty) || !roomPositions.All(IsPositionInsideLevelBounds)) return false;
            
            var adjacentPositions = roomPositions.Where(IsPositionAdjacent).ToList();
            // if (adjacentPositions.IsEmpty()) return false;

            return AreAllAdjacentPositionsValid(gridPosition, rotatedRoomDto, adjacentPositions);
        }

        private bool IsPositionEmpty(Vector2Int gridPosition)
        {
            return _roomTiles.All(room => room.GridPosition != gridPosition);
        }

        private bool IsPositionInsideLevelBounds(Vector2Int gridPosition)
        {
            return _maybeCurrentLevel.TryGetValue(out var currentLevel) && currentLevel.Contains(gridPosition);
        }

        private bool IsPositionAdjacent(Vector2Int gridPosition)
        {
            return _roomTiles.Any(room => room.GridPosition.IsAdjacent(gridPosition));
        }
        
        private bool AreAllAdjacentPositionsValid(Vector2Int gridPosition, RoomDto roomDto, List<Vector2Int> adjacentPositions)
        {
            foreach (var adjacentPosition in adjacentPositions)
            {
                if (!GetAdjacentDirections(adjacentPosition, out var adjacentOpenDirections, out var adjacentAnyDirections, out var adjacentClosedDirections))
                    continue;

                var roomPosition = adjacentPosition - gridPosition + roomDto.StartingPosition;
                var roomTileCell = roomDto.Tiles.First(tile => tile.Position == roomPosition);

                var roomDtoConfig = new TileDirectionConfiguration(roomTileCell);
                var adjacentDirectionsConfig = new TileDirectionConfiguration(adjacentOpenDirections,
                    adjacentAnyDirections, adjacentClosedDirections);
                
                if (!IsValidAdjacency(roomDtoConfig, adjacentDirectionsConfig))
                    return false;
            }

            return true;
        }

        public Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPos.x + mousePositionOffset.x),
                Mathf.RoundToInt(worldPos.y + mousePositionOffset.y));
        }

        public void UnloadLevel()
        {
            _lastPlacedRooms.Clear();
            foreach (var roomModel in _rooms)
            {
                roomModel.ClearTiles(prefabPool);
            }
            _rooms.Clear();
            _roomTiles.Clear();

            _maybeCurrentLevel = Maybe.Empty<Level>();
        }

        public void LoadLevel(Level level)
        {
            _maybeCurrentLevel = Maybe.Of(level);
            _currentDirection = RoomDirectionExtensions.Default;

            foreach (var roomDto in level.StartingPlacedRooms)
            {
                PlaceRoom(roomDto.Room.ToDto().Rotate(roomDto.Direction), roomDto.Position);
            }
            
            levelBuildingBackground.size = level.LevelSize;
            SignalsHub.DispatchAsync(new LevelSetupCompletedSignal(level));
        }

        private Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, gridPos.y, 0);
        }

        private void RotateGhostView(bool clockwise)
        {
            var rotation = clockwise ? RoomDirection.West : RoomDirection.East;
            _currentDirection =  _currentDirection.Rotate(rotation);
        }

        private void PlaceRoom(RoomDto selectedRoomDto, Vector2Int gridPosition)
        {
            var rotatedRoomDto = selectedRoomDto.Rotate(_currentDirection);
            var startingTilePosition = rotatedRoomDto.StartingPosition;
            var selectedRoomTiles = new List<DungeonRoomTileView>();
            var roomFloorSprite =
                selectedRoomDto.RoomTypes.Contains(RoomType.Shared)
                    ? roomRegistry.SharedRoomFloorSprite
                    : roomRegistry.UnusedRoomFloorSprite;
            
            foreach (var roomTileCell in rotatedRoomDto.Tiles)
            {
                var dungeonRoomTile = prefabPool.Spawn(dungeonRoomTilePrefab, roomContainer)
                    .GetComponent<DungeonRoomTileView>();
                var tileGridPosition = gridPosition + roomTileCell.Position - startingTilePosition;
                dungeonRoomTile.transform.position = GridToWorld(tileGridPosition);
                dungeonRoomTile.SetUp(roomTileCell, tileGridPosition, roomFloorSprite, selectedRoomDto);
                dungeonRoomTile.IsConnected = _roomGhostInstance && _roomGhostInstance.IsConnected;
                _roomTiles.Add(dungeonRoomTile);
                selectedRoomTiles.Add(dungeonRoomTile);
                SignalsHub.DispatchAsync(new RoomTilePlacedSignal(dungeonRoomTile));
            }

            var adjacentRooms = GetAdjacentRooms(selectedRoomTiles);
            var roomModel = new DungeonRoomModel(selectedRoomDto, selectedRoomTiles, adjacentRooms);
            foreach (var adjacentRoom in adjacentRooms)
            {
                adjacentRoom.AddAdjacentRoom(roomModel);
            }
            _rooms.Add(roomModel);
            if (!roomModel.HasType(RoomType.Shared))
                _lastPlacedRooms.Push(roomModel);
            
            SignalsHub.DispatchAsync(new RoomPlacedSignal(roomModel));
            handManager.TryPlaySelectRoomCard();
            handManager.RefillRoomHand();

            soundManager.PlaySound(SoundType.PlaceRoom);
            _roomGhostInstance.gameObject.SetActive(false);
        }

        public List<DungeonRoomModel> GetAdjacentRooms(IEnumerable<IDungeonRoomTileView> selectedRoomTiles)
        {
            return _rooms.Where(room => room.IsAdjacent(selectedRoomTiles)).ToList();
        }

        public IReadOnlyList<DungeonRoomTileView> RoomTiles => _roomTiles;
        public IReadOnlyList<DungeonRoomModel> Rooms => _rooms;
        public int EmptyRoomTilesCount => _maybeCurrentLevel.TryGetValue(out var currentLevel) ? currentLevel.TileCount - _roomTiles.Count(tile => tile.IsUsed) : 0;
        public bool CanUndoRoomPlacement => _lastPlacedRooms.TryPeek(out var  lastPlacedRoom) && !lastPlacedRoom.IsUsed;

        public Bounds GetRoomBoundsBasedOnTiles()
        {
            var minX = _roomTiles.Min(room => room.GridPosition.x);
            var minY = _roomTiles.Min(room => room.GridPosition.y);
            var maxX = _roomTiles.Max(room => room.GridPosition.x);
            var maxY = _roomTiles.Max(room => room.GridPosition.y);
            
            var center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f);
            var size = new Vector3(maxX - minX, maxY - minY);

            return new Bounds(center, size);
        }

        public bool IsTileAdjacentToLevelBounds(Vector2Int gridPosition, RoomDirection direction)
        {
            var adjacentPosition = gridPosition + direction.ToVector2Int();
            return IsPositionInsideLevelBounds(gridPosition) && !IsPositionInsideLevelBounds(adjacentPosition);
        }

        public bool IsTileAdjacentDoor(Vector2Int gridPosition, RoomDirection direction,
            out DungeonRoomTileView adjacentTile)
        {
            var adjacentPosition = gridPosition + direction.ToVector2Int();
            return _roomTiles.TryGetFirst(tile => tile.GridPosition == adjacentPosition, out adjacentTile) && 
                   adjacentTile.DoorDirections.Contains(direction.Invert());
        }
        
        public bool IsTileAdjacentToDoorOrEmpty(Vector2Int gridPosition, RoomDirection direction, out DungeonRoomTileView adjacentTile)
        {
            var adjacentPosition = gridPosition + direction.ToVector2Int();
            if (!_roomTiles.TryGetFirst(tile => tile.GridPosition == adjacentPosition, out adjacentTile))
                return true;

            return adjacentTile.DoorDirections.Contains(direction.Invert());
        }

        public bool UndoLastRoomPlacement()
        {
            if (CanUndoRoomPlacement && _lastPlacedRooms.TryPop(out var lastPlacedRoom))
            {
                foreach (var adjacentRoom in lastPlacedRoom.AdjacentRooms)
                {
                    adjacentRoom.RemoveAdjacentRoom(lastPlacedRoom);
                }
                _rooms.Remove(lastPlacedRoom);
                foreach (var tileView in lastPlacedRoom.Tiles)
                {
                    _roomTiles.Remove(tileView); 
                    SignalsHub.DispatchAsync(new RoomTileRemovedSignal(tileView));
                }
                SignalsHub.DispatchAsync(new RoomRemovedSignal(lastPlacedRoom));
                lastPlacedRoom.ClearTiles(prefabPool);
                handManager.TryUnplayLastCard();
                
                return true;
            }

            return false;
        }

        public Bounds GetLevelBounds()
        {
            if (!_maybeCurrentLevel.TryGetValue(out var currentLevel)) return new Bounds();
            
            var minX = -currentLevel.HalfLevelSize.x;
            var maxX = currentLevel.HalfLevelSize.x;
            var minY = -currentLevel.HalfLevelSize.y;
            var maxY = currentLevel.HalfLevelSize.y;
            
            var center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f);
            var size = new Vector3(maxX - minX, maxY - minY);
            
            return new Bounds(center, size);
        }
    }
}