using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Utils;
using ModestTree;
using UnityEngine;
using Utilities.Monads;
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
        
        [Inject] private IHandManager handManager;
        [Inject] private IPrefabPool prefabPool;
        [Inject(Id = "uiCamera")] private Camera uiCamera;

        private DungeonRoomGhostView roomGhostInstance;
        private List<DungeonRoomView> rooms = new();
        private RoomDirection currentRoomDirection = RoomDirection.North;

        private void Start()
        {
            currentRoomDirection = RoomDirection.North;
            rooms = new List<DungeonRoomView>();
            roomGhostInstance = prefabPool.Spawn(dungeonRoomGhostPrefab, roomContainer).GetComponent<DungeonRoomGhostView>();
            
            var startingRoomView = prefabPool.Spawn(dungeonRoomPrefab, roomContainer).GetComponent<DungeonRoomView>();
            var startingRoomPosition = new Vector2Int(); // 0, 0
            startingRoomView.SetUp(startingRoom.ToCard(), startingRoomPosition, RoomDirection.North); // Direction should be irrelevant
            startingRoomView.transform.position = GridToWorld(startingRoomPosition);
            rooms.Add(startingRoomView);
        }

        private void Update()
        {
            if (handManager.SelectedRoomCardView.TryGetValue(out var selectedRoomCardView))
            {
                roomGhostInstance.gameObject.SetActive(true);
            }
            else
            {
                roomGhostInstance.gameObject.SetActive(false);
                return;
            }
            
            Vector2 mouseWorld = uiCamera.ScreenToWorldPoint(Input.mousePosition);
            var gridPosition = WorldToGrid(mouseWorld);
            var snappedPosition = GridToWorld(gridPosition);
            // Debug.Log($"Mouse Position {mouseWorld}, gridPosition {gridPosition}, snappedPosition {snappedPosition}");
            
            roomGhostInstance.transform.position = snappedPosition;

            if (!IsPositionEmptyAndAdjacent(gridPosition) || 
                !TryGetValidDirection(selectedRoomCardView, gridPosition, currentRoomDirection, out var validDirection))
            {
                roomGhostInstance.transform.rotation = currentRoomDirection.ToRotation();
                roomGhostInstance.SetUpInvalid(selectedRoomCardView.Dto);
                return;
            }

            currentRoomDirection = validDirection;
            roomGhostInstance.transform.rotation = validDirection.ToRotation();
            roomGhostInstance.SetUpValid(selectedRoomCardView.Dto);
            
            // if (Input.GetMouseButtonDown(0)) PlaceRoom(gridPosition);
            //
            // if (Input.mouseScrollDelta.y != 0) RotatePreview(Input.mouseScrollDelta.y);
        }

        private bool TryGetValidDirection(RoomCardView selectedRoomCardView, Vector2Int gridPosition,
            RoomDirection currentDirection, out RoomDirection validDirection)
        {
            validDirection = currentDirection;
            var adjacentRooms = rooms.Where(room => room.GridPosition.ManhattanDistance(gridPosition) == 1).ToList();

            if (adjacentRooms.IsEmpty())
                return false;

            var adjacentOpenDirections = new List<RoomDirection>();
            var adjacentClosedDirections = new List<RoomDirection>();
            foreach (var adjacentRoom in adjacentRooms)
            {
                var adjacentRoomDirection = (adjacentRoom.GridPosition - gridPosition).FromVector2Int();
                if (adjacentRoom.OpenDirections.Contains(adjacentRoomDirection))
                    adjacentOpenDirections.Add(adjacentRoomDirection);
                else
                    adjacentClosedDirections.Add(adjacentRoomDirection);
            }
            
            if (CheckValidDirection(currentDirection, selectedRoomCardView.Dto.OpenDirections, adjacentOpenDirections, adjacentClosedDirections))
                return true;

            var firstValidDirection = EnumExtensions.GetAllValues<RoomDirection>()
                .FirstOrEmpty(roomDirection => CheckValidDirection(roomDirection, selectedRoomCardView.Dto.OpenDirections, adjacentOpenDirections, adjacentClosedDirections));
            
            return firstValidDirection.TryGetValue(out validDirection);
        }

        private bool CheckValidDirection(RoomDirection currentDirection, 
            IEnumerable<RoomDirection> dtoOpenDirections, 
            IEnumerable<RoomDirection> adjacentOpenDirections,
            IEnumerable<RoomDirection> adjacentClosedDirections)
        {
            var rotatedDtoOpenDirections = dtoOpenDirections.Select(direction => direction.Rotate(currentDirection));
            return adjacentOpenDirections.All(openDirection => rotatedDtoOpenDirections.Contains(openDirection)) &&
                   adjacentClosedDirections.All(closedDirection => !rotatedDtoOpenDirections.Contains(closedDirection));
        }

        private bool IsPositionEmptyAndAdjacent(Vector2Int gridPosition)
        {
            return rooms.All(room => room.GridPosition != gridPosition) && rooms.Any(room => room.GridPosition.ManhattanDistance(gridPosition) == 1);
        }

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x + mousePositionOffset.x), Mathf.RoundToInt(worldPos.y + mousePositionOffset.y));
        }

        private Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, gridPos.y, 0);
        }

        // private bool IsPlacementValid(Vector2Int gridPos)
        // {
        //     if (placedPositions.Contains(gridPos)) return false;
        //
        //     // Check for at least one adjacent room
        //     Vector2Int[] directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        //     foreach (var dir in directions)
        //         if (placedPositions.Contains(gridPos + dir))
        //             return true;
        //
        //     return placedPositions.Count == 0; // allow first placement
        // }
        //
        // private void PlaceRoom(Vector2Int gridPos)
        // {
        //     var worldPos = GridToWorld(gridPos);
        //     prefabPool.Spawn(dungeonRoomPrefab, worldPos, currentRotation);
        //     placedPositions.Add(gridPos);
        //
        //     handManager.TryPlaySelectRoomCard();
        //     selectedCard = null;
        //
        //     if (previewGhostInstance != null)
        //     {
        //         Destroy(previewGhostInstance);
        //         previewGhostInstance = null;
        //     }
        // }
        //
        // private void RotatePreview(float direction)
        // {
        //     currentRotation *= Quaternion.Euler(0, 0, -90f * Mathf.Sign(direction));
        // }
    }
}