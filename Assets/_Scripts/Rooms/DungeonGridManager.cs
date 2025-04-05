using _Scripts.Cards;
using UnityEngine;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonGridManager : MonoBehaviour, IDungeonGridManager
    {
        [SerializeField] private Grid grid;
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private GameObject dungeonRoomPrefab;
        [SerializeField] private GameObject dungeonRoomGhostPrefab;
        
        [Inject] private IHandManager handManager;
        [Inject] private IPrefabPool prefabPool;

        private DungeonRoomGhostView _roomGhostInstance;

        private void Start()
        {
            _roomGhostInstance = prefabPool.Spawn(dungeonRoomGhostPrefab).GetComponent<DungeonRoomGhostView>();
            
        }

        private void Update()
        {
            // if (handManager.SelectedRoomCard.IsPresent && _roomGhostInstance != null)
            // {
            //     _roomGhostInstance.
            //     return;
            // }
            //
            // Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // var gridPos = WorldToGrid(mouseWorld);
            // var snappedPos = GridToWorld(gridPos);
            //
            // if (previewGhostInstance == null) previewGhostInstance = Instantiate(previewGhostPrefab);
            //
            // previewGhostInstance.SetActive(true);
            // previewGhostInstance.transform.position = snappedPos;
            // previewGhostInstance.transform.rotation = currentRotation;
            //
            // if (IsPlacementValid(gridPos))
            // {
            //     if (Input.GetMouseButtonDown(0)) PlaceRoom(gridPos);
            //
            //     if (Input.mouseScrollDelta.y != 0) RotatePreview(Input.mouseScrollDelta.y);
            // }
        }

        private Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x / gridSize), Mathf.RoundToInt(worldPos.y / gridSize));
        }

        private Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * gridSize, gridPos.y * gridSize, 0);
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