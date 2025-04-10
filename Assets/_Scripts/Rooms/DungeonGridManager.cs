using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Utils;
using ModestTree;
using Signals;
using UnityEngine;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonGridManager : MonoBehaviour, IDungeonGridManager
    {
        [SerializeField] private Transform roomContainer;
        [SerializeField] private GameObject dungeonRoomPrefab;
        [SerializeField] private GameObject dungeonRoomGhostPrefab;
        [SerializeField] private Room startingRoom;
        [SerializeField] private Vector2 mousePositionOffset;
        [SerializeField] private List<RectTransform> unclickableScreenAreas;

        [Inject] private IHandManager handManager;
        [Inject] private IPrefabPool prefabPool;
        [Inject] private IDungeonCameraController dungeonCameraController;
        [Inject] private ISoundManager soundManager;
        [Inject(Id = "uiCamera")] private Camera uiCamera;

        private DungeonRoomGhostView roomGhostInstance;
        private List<DungeonRoomView> rooms = new();
        private RoomDirection currentDirection = RoomDirectionExtensions.Default;
        private List<Bounds> unclickableBounds;

        private void Start()
        {
            currentDirection = RoomDirectionExtensions.Default;
            rooms = new List<DungeonRoomView>();
            unclickableBounds = unclickableScreenAreas.Select(RectTransformUtility.CalculateRelativeRectTransformBounds)
                .ToList();
            roomGhostInstance = prefabPool.Spawn(dungeonRoomGhostPrefab, roomContainer)
                .GetComponent<DungeonRoomGhostView>();

            var startingRoomPosition = new Vector2Int(); // 0, 0
            PlaceRoom(startingRoom.ToDto(), startingRoomPosition);
        }

        private void Update()
        {
            if (!handManager.SelectedRoomCardView.TryGetValue(out var selectedRoomCardView))
            {
                roomGhostInstance.gameObject.SetActive(false);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                roomGhostInstance.gameObject.SetActive(false);
                handManager.DeselectRoomCard();
                return;
            }

            var mouseUI = dungeonCameraController.GetMouseUIPosition();
            if (unclickableScreenAreas.Any(area => RectTransformUtility.RectangleContainsScreenPoint(area, mouseUI, uiCamera)))
            {
                roomGhostInstance.gameObject.SetActive(false);
                return;
            }
            
            roomGhostInstance.gameObject.SetActive(true);

            Vector2 mouseWorld = dungeonCameraController.GetMouseWorldPosition();
            var gridPosition = WorldToGrid(mouseWorld);
            var snappedPosition = GridToWorld(gridPosition);
            // Debug.Log($"Mouse Position {mouseWorld}, gridPosition {gridPosition}, snappedPosition {snappedPosition}");

            roomGhostInstance.transform.position = snappedPosition;

            if (!IsPositionEmptyAndAdjacent(gridPosition) ||
                !TryGetValidDirection(selectedRoomCardView, gridPosition, out var validDirection))
            {
                roomGhostInstance.transform.rotation = currentDirection.ToRotation();
                roomGhostInstance.SetUpInvalid(selectedRoomCardView.Dto);
                return;
            }

            currentDirection = validDirection;
            roomGhostInstance.transform.rotation = validDirection.ToRotation();
            roomGhostInstance.SetUpValid(selectedRoomCardView.Dto);

            if (selectedRoomCardView.Dto.IsRotatable && Input.mouseScrollDelta.y != 0)
                RotateGhostView(selectedRoomCardView, gridPosition, Input.mouseScrollDelta.y > 0);

            if (Input.GetMouseButtonDown(0))
                PlaceRoom(selectedRoomCardView.Dto, gridPosition);
        }

        private bool TryGetValidDirection(RoomCardView selectedRoomCardView, Vector2Int gridPosition,
            out RoomDirection validDirection)
        {
            validDirection = currentDirection;

            if (!GetAdjacentDirections(gridPosition, out var adjacentOpenDirections, out var adjacentClosedDirections))
                return false;

            if (IsValidDirection(currentDirection, selectedRoomCardView.Dto.OpenDirections, adjacentOpenDirections,
                    adjacentClosedDirections))
                return true;

            foreach (var roomDirection in EnumExtensions.GetAllItems<RoomDirection>())
                if (IsValidDirection(roomDirection, selectedRoomCardView.Dto.OpenDirections,
                        adjacentOpenDirections, adjacentClosedDirections))
                {
                    validDirection = roomDirection;
                    return true;
                }

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

        private void RotateGhostView(RoomCardView selectedRoomCardView, Vector2Int gridPosition, bool clockwise)
        {
            if (!GetAdjacentDirections(gridPosition, out var adjacentOpenDirections, out var adjacentClosedDirections))
                return;

            var rotation = clockwise ? RoomDirection.West : RoomDirection.East;
            var rotatedDirection = currentDirection.Rotate(rotation);

            if (IsValidDirection(rotatedDirection, selectedRoomCardView.Dto.OpenDirections, adjacentOpenDirections,
                    adjacentClosedDirections))
            {
                currentDirection = rotatedDirection;
                return;
            }

            var invertedDirection = currentDirection.Invert();
            if (IsValidDirection(invertedDirection, selectedRoomCardView.Dto.OpenDirections, adjacentOpenDirections,
                    adjacentClosedDirections))
            {
                currentDirection = invertedDirection;
                return;
            }

            var invertedRotatedDirection = currentDirection.Rotate(rotation.Invert());
            if (IsValidDirection(invertedRotatedDirection, selectedRoomCardView.Dto.OpenDirections,
                    adjacentOpenDirections, adjacentClosedDirections)) currentDirection = invertedRotatedDirection;
        }

        private void PlaceRoom(RoomDto selectedRoomDto, Vector2Int gridPosition)
        {
            var worldPosition = GridToWorld(gridPosition);
            var dungeonRoom = prefabPool.Spawn(dungeonRoomPrefab, roomContainer)
                .GetComponent<DungeonRoomView>();
            dungeonRoom.transform.position = worldPosition;
            dungeonRoom.SetUp(selectedRoomDto, gridPosition,
                selectedRoomDto.IsRotatable ? currentDirection : RoomDirectionExtensions.Default);
            rooms.Add(dungeonRoom);

            handManager.TryPlaySelectRoomCard();
            handManager.RefillHand();
            roomGhostInstance.gameObject.SetActive(false);

            soundManager.PlaySound(SoundType.PlaceRoom);
            SignalsHub.DispatchAsync(new RoomPlacedSignal(dungeonRoom));
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