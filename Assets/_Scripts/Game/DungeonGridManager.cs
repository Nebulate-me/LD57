using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using ModestTree;
using Signals;
using UnityEngine;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Game
{
    public class DungeonGridManager : MonoBehaviour, IDungeonGridManager
    {
        [SerializeField] private Transform roomContainer;
        [SerializeField] private GameObject dungeonRoomPrefab;
        [SerializeField] private GameObject dungeonRoomGhostPrefab;
        // TODO: Replace with the Level Setup - starting room and position
        [Space]
        [SerializeField] private Room startingRoom;
        [SerializeField] private Vector2Int startingRoomPosition;
        [Space]
        [SerializeField] private Vector2 mousePositionOffset;
        [SerializeField] private List<RectTransform> unclickableScreenAreas;

        [Inject] private IHandManager handManager;
        [Inject] private IPrefabPool prefabPool;
        [Inject] private IDungeonCameraController dungeonCameraController;
        [Inject] private ISoundManager soundManager;
        [Inject(Id = "uiCamera")] private Camera uiCamera;

        private DungeonRoomTileGhostView _roomTileGhostInstance;
        private DungeonRoomGhostView _roomGhostInstance;
        private List<DungeonRoomView> _rooms = new();
        private RoomDirection _currentDirection = RoomDirectionExtensions.Default;
        private List<Bounds> unclickableBounds;

        private void Start()
        {
            _currentDirection = RoomDirectionExtensions.Default;
            _rooms = new List<DungeonRoomView>();
            unclickableBounds = unclickableScreenAreas.Select(RectTransformUtility.CalculateRelativeRectTransformBounds)
                .ToList();
            
            _roomGhostInstance = prefabPool.Spawn(dungeonRoomGhostPrefab, roomContainer)
                .GetComponent<DungeonRoomGhostView>();
            
            PlaceRoom(startingRoom.ToDto(), startingRoomPosition);
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
            
            if (!AreRoomPositionsEmptyAndAdjacent(gridPosition, selectedRoomCardView.RoomDto, _currentDirection))
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

            var adjacentRooms = _rooms.Where(room => room.GridPosition.ManhattanDistance(roomPosition) == 1).ToList();

            if (adjacentRooms.IsEmpty())
                return false;

            foreach (var adjacentRoom in adjacentRooms)
            {
                var adjacentRoomDirection = (adjacentRoom.GridPosition - roomPosition).FromVector2Int();
                var invertedAdjacentRoomDirection = adjacentRoomDirection.Invert();
                if (adjacentRoom.OpenDirections.Contains(invertedAdjacentRoomDirection))
                    adjacentOpenDirections.Add(adjacentRoomDirection);
                else if (adjacentRoom.DoorDirections.Contains(invertedAdjacentRoomDirection))
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
                adjacentConfiguration.OpenDirections.All(openDirection => dtoConfiguration.OpenDirections.Contains(openDirection) || dtoConfiguration.AnyDirections.Contains(openDirection) );
            var allClosedDirectionsClosed =
                adjacentConfiguration.ClosedDirections.All(closedDirection => dtoConfiguration.ClosedDirections.Contains(closedDirection)  || dtoConfiguration.AnyDirections.Contains(closedDirection) );

            return allOpenDirectionsOpen && allClosedDirectionsClosed;
        }

        private bool AreRoomPositionsEmptyAndAdjacent(Vector2Int gridPosition, RoomDto roomDto, RoomDirection roomDirection)
        {
            var rotatedRoomDto = roomDto.Rotate(roomDirection);
            var roomStartingPosition = rotatedRoomDto.StartingPosition;
            var roomPositions = rotatedRoomDto.Tiles
                .Select(tile => tile.Position - roomStartingPosition + gridPosition).ToList();
            
            if (!roomPositions.All(IsPositionEmpty)) return false;
            
            var adjacentPositions = roomPositions.Where(IsPositionAdjacent).ToList();
            if (adjacentPositions.IsEmpty()) return false;

            return AreAllAdjacentPositionsValid(gridPosition, rotatedRoomDto, adjacentPositions);
        }

        private bool IsPositionEmpty(Vector2Int gridPosition)
        {
            return _rooms.All(room => room.GridPosition != gridPosition);
        }

        private bool IsPositionAdjacent(Vector2Int gridPosition)
        {
            return _rooms.Any(room => room.GridPosition.ManhattanDistance(gridPosition) == 1);
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

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPos.x + mousePositionOffset.x),
                Mathf.RoundToInt(worldPos.y + mousePositionOffset.y));
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
            foreach (var roomTileCell in rotatedRoomDto.Tiles)
            {
                var dungeonRoom = prefabPool.Spawn(dungeonRoomPrefab, roomContainer)
                    .GetComponent<DungeonRoomView>();
                var tileGridPosition = gridPosition + roomTileCell.Position - startingTilePosition;
                dungeonRoom.transform.position = GridToWorld(tileGridPosition);
                dungeonRoom.SetUp(roomTileCell.Tile, tileGridPosition, roomTileCell.Direction);
                _rooms.Add(dungeonRoom);
                
                SignalsHub.DispatchAsync(new RoomTilePlacedSignal(dungeonRoom));
            }
            handManager.TryPlaySelectRoomCard();
            handManager.RefillRoomHand();

            soundManager.PlaySound(SoundType.PlaceRoom);
            _roomGhostInstance.gameObject.SetActive(false);
        }

        public IReadOnlyList<DungeonRoomView> Rooms => _rooms;

        public Bounds GetRoomBounds()
        {
            var minX = _rooms.Min(room => room.GridPosition.x);
            var minY = _rooms.Min(room => room.GridPosition.y);
            var maxX = _rooms.Max(room => room.GridPosition.x);
            var maxY = _rooms.Max(room => room.GridPosition.y);

            var center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f);
            var size = new Vector3(maxX - minX, maxY - minY);

            return new Bounds(center, size);
        }
    }
}