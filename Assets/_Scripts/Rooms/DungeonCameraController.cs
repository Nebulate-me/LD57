using _Scripts.Cards;
using Signals;
using UnityEngine;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonCameraController : MonoBehaviour, IDungeonCameraController
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float zoomSpeedButtonMultiplier = 0.1f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 100f;
        [SerializeField] private Vector2 positionClampOffset = new(0, 1f);
        [SerializeField] private float panSpeed = 1f;
        [SerializeField] private float panZoomFactor = 5f;
        
        [Inject] private IHandManager handManager;
        [Inject] private IDungeonGridManager dungeonGridManager;

        private Bounds dungeonBounds;
        private Plane worldPlane;
        private Vector3 lastMousePosition;
        private bool isDragging;

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
        }
        
        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
        }

        private void OnRoomPlaced(RoomPlacedSignal signal)
        {
            dungeonBounds = dungeonGridManager.GetRoomBounds();
        }

        private void Start()
        {
            worldPlane = new Plane(Vector3.back, Vector3.zero);
        }

        private void LateUpdate()
        {
            HandleCameraDrag();

            HandleCameraZoom();
        }

        private void HandleCameraDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0) || handManager.SelectedRoomCardView.IsPresent)
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 move = -delta * panSpeed * Time.deltaTime;

                move *= mainCamera.orthographicSize / panZoomFactor;

                mainCamera.transform.position = ClampCameraPosition(mainCamera.transform.position + move);
                
                lastMousePosition = Input.mousePosition;
            }
        }
        
        private void HandleCameraZoom()
        {
            if (handManager.SelectedRoomCardView.IsNotPresent)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    mainCamera.orthographicSize = ClampCameraZoom(mainCamera.orthographicSize - Mathf.Sign(Input.mouseScrollDelta.y) * zoomSpeed);    
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    mainCamera.orthographicSize = ClampCameraZoom(mainCamera.orthographicSize - zoomSpeed * zoomSpeedButtonMultiplier);
                } else if (Input.GetKey(KeyCode.E))
                {
                    mainCamera.orthographicSize = ClampCameraZoom(mainCamera.orthographicSize + zoomSpeed * zoomSpeedButtonMultiplier);
                }
                   
            }
        }

        private float ClampCameraZoom(float cameraZoom)
        {
            return Mathf.Clamp(cameraZoom, minZoom, maxZoom);
        }

        private Vector3 ClampCameraPosition(Vector3 cameraPosition)
        {
            return new Vector3(
                Mathf.Clamp(cameraPosition.x, dungeonBounds.min.x - positionClampOffset.x, dungeonBounds.max.x + positionClampOffset.x),
                Mathf.Clamp(cameraPosition.y, dungeonBounds.min.y - positionClampOffset.y, dungeonBounds.max.y + positionClampOffset.y),
                -10f);
        }

        public Vector3 GetMouseUIPosition()
        {
            return Input.mousePosition;
        }
        
        public Vector3 GetMouseWorldPosition()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (worldPlane.Raycast(ray, out var enter)) {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }
    }
}