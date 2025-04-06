using System;
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
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 100f;
        [SerializeField] private float cameraMovementSpeed = 0.1f;
        [SerializeField] private float noMovementRadius = 2f;
        [SerializeField] private Vector2 positionClampOffset = new Vector2(0, 1f);
        
        [Inject] private IHandManager handManager;
        [Inject] private IDungeonGridManager dungeonGridManager;
        [Inject(Id = "uiCamera")] private Camera uiCamera;
        
        private Bounds dungeonBounds;
        private Plane worldPlane;
        
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
            var mousePosition = uiCamera.ScreenToWorldPoint(Input.mousePosition);
            var cameraDirection = mousePosition.magnitude > noMovementRadius ? mousePosition.normalized * cameraMovementSpeed : Vector3.zero;
            if (cameraDirection != Vector3.zero)
            {
                var targetCameraPosition = ClampCameraPosition(mainCamera.transform.position + cameraDirection);
                var mainCameraTransform = mainCamera.transform;
                var position = mainCameraTransform.position;
                position = new Vector3(
                    (targetCameraPosition.x + position.x) / 2f,
                    (targetCameraPosition.y + position.y) / 2f,
                    -10f);
                mainCameraTransform.position = position;
                Debug.Log($"Move Camera > {cameraDirection}");
            }

            if (handManager.SelectedRoomCardView.IsNotPresent && Input.mouseScrollDelta.y != 0)
            {
                mainCamera.fieldOfView = ClampCameraZoom(mainCamera.fieldOfView - Mathf.Sign(Input.mouseScrollDelta.y) * zoomSpeed);   
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
        
        public Vector3 GetMousePosition()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (worldPlane.Raycast(ray, out float enter)) {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }

        public Camera Camera => mainCamera;
        public Vector3 Position => mainCamera.transform.position;
    }
}