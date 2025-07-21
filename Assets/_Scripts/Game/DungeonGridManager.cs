using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using ModestTree;
using Signals;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Game
{
    public class DungeonGridManager : MonoBehaviour, IDungeonGridManager
    {
        [SerializeField] private Transform roomContainer;
        [SerializeField] private GameObject dungeonRoomPrefab;
        [SerializeField] private GameObject dungeonRoomGhostPrefab;
        [SerializeField] private GameObject dungeonRoomTileGhostPrefab;
        [FormerlySerializedAs("startingRoom")] [SerializeField] private RoomTile startingRoomTile;
        [SerializeField] private Vector2 mousePositionOffset;
        [SerializeField] private List<RectTransform> unclickableScreenAreas;

        [Inject] private IHandManager handManager;
        [Inject] private IPrefabPool prefabPool;
        [Inject] private IDungeonCameraController dungeonCameraController;
        [Inject] private ISoundManager soundManager;
        [Inject(Id = "uiCamera")] private Camera uiCamera;

        private DungeonRoomTileGhostView _roomTileGhostInstance;
        private DungeonRoomGhostView _roomGhostInstance;
        private List<DungeonRoomView> rooms = new();
        private RoomDirection currentDirection = RoomDirectionExtensions.Default;
        private List<Bounds> unclickableBounds;

        private void Start()
        {
            currentDirection = RoomDirectionExtensions.Default;
            rooms = new List<DungeonRoomView>();
            unclickableBounds = unclickableScreenAreas.Select(RectTransformUtility.CalculateRelativeRectTransformBounds)
                .ToList();
            
            _roomTileGhostInstance = prefabPool.Spawn(dungeonRoomTileGhostPrefab, roomContainer)
                .GetComponent<DungeonRoomTileGhostView>();
            _roomGhostInstance = prefabPool.Spawn(dungeonRoomGhostPrefab, roomContainer)
                .GetComponent<DungeonRoomGhostView>();

            // TODO: Replace with the Elevator? Or the Level setup starting room hall/foyer?
            var startingRoomPosition = new Vector2Int(); // 0, 0
            PlaceRoomTile(startingRoomTile.ToDto(), startingRoomPosition);
        }

        private void Update()
        {
            // if (!handManager.SelectedRoomTileCardView.TryGetValue(out var selectedRoomTileCardView))
            if (!handManager.SelectedRoomCardView.TryGetValue(out var selectedRoomCardView))
            {
                // _roomTileGhostInstance.gameObject.SetActive(false);
                _roomGhostInstance.gameObject.SetActive(false);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                // _roomTileGhostInstance.gameObject.SetActive(false);
                _roomGhostInstance.gameObject.SetActive(false);
                // handManager.DeselectRoomTileCard();
                handManager.DeselectRoomCard();
                return;
            }

            var mouseUI = dungeonCameraController.GetMouseUIPosition();
            if (unclickableScreenAreas.Any(area => RectTransformUtility.RectangleContainsScreenPoint(area, mouseUI, uiCamera)))
            {
                // _roomTileGhostInstance.gameObject.SetActive(false);
                _roomGhostInstance.gameObject.SetActive(false);
                return;
            }
            
            // _roomTileGhostInstance.gameObject.SetActive(true);
            _roomGhostInstance.gameObject.SetActive(true);

            Vector2 mouseWorld = dungeonCameraController.GetMouseWorldPosition();
            var gridPosition = WorldToGrid(mouseWorld);
            var snappedPosition = GridToWorld(gridPosition);
            // Debug.Log($"Mouse Position {mouseWorld}, gridPosition {gridPosition}, snappedPosition {snappedPosition}");

            // _roomTileGhostInstance.transform.position = snappedPosition;
            _roomGhostInstance.transform.position = snappedPosition;

            // if (!IsPositionEmptyAndAdjacent(gridPosition) ||
            //     !TryGetValidDirection(selectedRoomCardView, gridPosition, out var validDirection))
            if (!AreRoomPositionsEmptyAndAdjacent(gridPosition, selectedRoomCardView.RoomDto))
            {
                // _roomTileGhostInstance.transform.rotation = currentDirection.ToRotation();
                // _roomTileGhostInstance.SetUpInvalid(selectedRoomCardView.RoomDto);
                _roomGhostInstance.transform.rotation = currentDirection.ToRotation();
                _roomGhostInstance.SetUpInvalid(selectedRoomCardView.RoomDto);
                return;
            }

            // currentDirection = validDirection; // TODO: Determine the direction correctly
            // _roomTileGhostInstance.transform.rotation = validDirection.ToRotation();
            // _roomTileGhostInstance.SetUpValid(selectedRoomCardView.RoomDto);
            _roomGhostInstance.SetUpValid(selectedRoomCardView.RoomDto);

            // if (selectedRoomCardView.RoomDto.IsRotatable && Input.mouseScrollDelta.y != 0)
            //     RotateGhostView(selectedRoomCardView, gridPosition, Input.mouseScrollDelta.y > 0);
            // TODO: Rotate Ghost Room

            if (Input.GetMouseButtonDown(0))
                PlaceRoom(selectedRoomCardView.RoomDto, gridPosition);
        }

        private bool TryGetValidDirection(RoomTileCardView selectedRoomTileCardView, Vector2Int gridPosition,
            out RoomDirection validDirection)
        {
            validDirection = currentDirection;

            if (!GetAdjacentDirections(gridPosition, out var adjacentOpenDirections, out var adjacentClosedDirections))
                return false;

            if (IsValidDirection(currentDirection, selectedRoomTileCardView.TileDto.OpenDirections, adjacentOpenDirections,
                    adjacentClosedDirections))
                return true;

            foreach (var roomDirection in EnumExtensions.GetAllItems<RoomDirection>())
                if (IsValidDirection(roomDirection, selectedRoomTileCardView.TileDto.OpenDirections,
                        adjacentOpenDirections, adjacentClosedDirections))
                {
                    validDirection = roomDirection;
                    return true;
                }

            return false;
        }
        
        private bool TryGetValidDirection(RoomCardView selectedRoomCardView, Vector2Int gridPosition,
            out RoomDirection validDirection)
        {
            validDirection = currentDirection;
            // TODO: Implement directional ratotion for the room

            if (!GetAdjacentDirections(gridPosition, out var adjacentOpenDirections, out var adjacentClosedDirections))
                return false;

            // if (IsValidDirection(currentDirection, selectedRoomCardView.RoomDto.OpenDirections, adjacentOpenDirections,
            //         adjacentClosedDirections))
            //     return true;

            // foreach (var roomDirection in EnumExtensions.GetAllItems<RoomDirection>())
            //     if (IsValidDirection(roomDirection, selectedRoomCardView.RoomDto.OpenDirections,
            //             adjacentOpenDirections, adjacentClosedDirections))
            //     {
            //         validDirection = roomDirection;
            //         return true;
            //     }

            return false;
        }

        private bool GetAdjacentDirections(Vector2Int roomPosition, out List<RoomDirection> adjacentOpenDirections,
            out List<RoomDirection> adjacentClosedDirections)
        {
            adjacentOpenDirections = new List<RoomDirection>();
            adjacentClosedDirections = new List<RoomDirection>();

            var adjacentRooms = rooms.Where(room => room.GridPosition.ManhattanDistance(roomPosition) == 1).ToList();

            if (adjacentRooms.IsEmpty())
                return false;

            foreach (var adjacentRoom in adjacentRooms)
            {
                var adjacentRoomDirection = (adjacentRoom.GridPosition - roomPosition).FromVector2Int();
                if (adjacentRoom.OpenDirections.Contains(adjacentRoomDirection.Invert()))
                    adjacentOpenDirections.Add(adjacentRoomDirection);
                else
                    adjacentClosedDirections.Add(adjacentRoomDirection);
            }

            return true;
        }

        private bool IsValidDirection(RoomDirection directionToCheck,
            IEnumerable<RoomDirection> dtoOpenDirections,
            IEnumerable<RoomDirection> adjacentOpenDirections,
            IEnumerable<RoomDirection> adjacentClosedDirections)
        {
            var rotatedDtoOpenDirections =
                dtoOpenDirections.Select(direction => direction.Rotate(directionToCheck)).ToList();
            var rotatedDtoClosedDirections = RoomDirectionExtensions.InvertList(rotatedDtoOpenDirections);
            var allOpenDirectionsOpen =
                adjacentOpenDirections.All(openDirection => rotatedDtoOpenDirections.Contains(openDirection));
            var allClosedDirectionsClosed =
                adjacentClosedDirections.All(closedDirection => rotatedDtoClosedDirections.Contains(closedDirection));

            return allOpenDirectionsOpen && allClosedDirectionsClosed;
        }

        private bool IsPositionEmptyAndAdjacent(Vector2Int gridPosition)
        {
            return rooms.All(room => room.GridPosition != gridPosition) &&
                   rooms.Any(room => room.GridPosition.ManhattanDistance(gridPosition) == 1);
        }

        private bool AreRoomPositionsEmptyAndAdjacent(Vector2Int gridPosition, RoomDto roomDto)
        {
            var roomStartingPosition = roomDto.StartingPosition;
            var roomPositions = roomDto.Tiles
                .Select(tile => tile.Position - roomStartingPosition + gridPosition).ToList();

            return roomPositions.All(IsPositionEmpty) && roomPositions.Any(IsPositionAdjacent);
        }
        
        private bool IsPositionEmpty(Vector2Int gridPosition)
        {
            return rooms.All(room => room.GridPosition != gridPosition);
        }

        private bool IsPositionAdjacent(Vector2Int gridPosition)
        {
            return rooms.Any(room => room.GridPosition.ManhattanDistance(gridPosition) == 1);
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

        private void RotateGhostView(RoomTileCardView selectedRoomTileCardView, Vector2Int gridPosition, bool clockwise)
        {
            if (!GetAdjacentDirections(gridPosition, out var adjacentOpenDirections, out var adjacentClosedDirections))
                return;

            var rotation = clockwise ? RoomDirection.West : RoomDirection.East;
            var rotatedDirection = currentDirection.Rotate(rotation);

            if (IsValidDirection(rotatedDirection, selectedRoomTileCardView.TileDto.OpenDirections, adjacentOpenDirections,
                    adjacentClosedDirections))
            {
                currentDirection = rotatedDirection;
                return;
            }

            var invertedDirection = currentDirection.Invert();
            if (IsValidDirection(invertedDirection, selectedRoomTileCardView.TileDto.OpenDirections, adjacentOpenDirections,
                    adjacentClosedDirections))
            {
                currentDirection = invertedDirection;
                return;
            }

            var invertedRotatedDirection = currentDirection.Rotate(rotation.Invert());
            if (IsValidDirection(invertedRotatedDirection, selectedRoomTileCardView.TileDto.OpenDirections,
                    adjacentOpenDirections, adjacentClosedDirections)) currentDirection = invertedRotatedDirection;
        }

        private void PlaceRoom(RoomDto selectedRoomDto, Vector2Int gridPosition)
        {
            var startingTilePosition = selectedRoomDto.StartingPosition;
            foreach (var roomTileCell in selectedRoomDto.Tiles)
            {
                var dungeonRoom = prefabPool.Spawn(dungeonRoomPrefab, roomContainer)
                    .GetComponent<DungeonRoomView>();
                var tileGridPosition = gridPosition + roomTileCell.Position - startingTilePosition;
                dungeonRoom.transform.position = GridToWorld(tileGridPosition);
                dungeonRoom.SetUp(roomTileCell.Tile.ToDto(), tileGridPosition, roomTileCell.Direction);
                rooms.Add(dungeonRoom);
                
                SignalsHub.DispatchAsync(new RoomTilePlacedSignal(dungeonRoom));
            }
            handManager.TryPlaySelectRoomCard();
            handManager.RefillRoomHand();

            soundManager.PlaySound(SoundType.PlaceRoom);
            _roomGhostInstance.gameObject.SetActive(false);
        }
        
        private void PlaceRoomTile(RoomTileDto selectedRoomTileDto, Vector2Int gridPosition)
        {
            var worldPosition = GridToWorld(gridPosition);
            var dungeonRoom = prefabPool.Spawn(dungeonRoomPrefab, roomContainer)
                .GetComponent<DungeonRoomView>();
            dungeonRoom.transform.position = worldPosition;
            dungeonRoom.SetUp(selectedRoomTileDto, gridPosition,
                selectedRoomTileDto.IsRotatable ? currentDirection : RoomDirectionExtensions.Default);
            rooms.Add(dungeonRoom);

            // handManager.TryPlaySelectRoomTileCard();
            // handManager.RefillRoomTileHand();
            _roomTileGhostInstance.gameObject.SetActive(false);

            soundManager.PlaySound(SoundType.PlaceRoom);
            SignalsHub.DispatchAsync(new RoomTilePlacedSignal(dungeonRoom));
        }

        public IReadOnlyList<DungeonRoomView> Rooms => rooms;

        public Bounds GetRoomBounds()
        {
            var minX = rooms.Min(room => room.GridPosition.x);
            var minY = rooms.Min(room => room.GridPosition.y);
            var maxX = rooms.Max(room => room.GridPosition.x);
            var maxY = rooms.Max(room => room.GridPosition.y);

            var center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f);
            var size = new Vector3(maxX - minX, maxY - minY);

            return new Bounds(center, size);
        }
    }
}