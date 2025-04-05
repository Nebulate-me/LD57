using System;
using _Scripts.Cards;
using Signals;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonCameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraPositionOffset = new Vector3(0, 1f, 0f);
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 100f;
        
        [Inject] private IHandManager handManager;
        [Inject] private IDungeonGridManager dungeonGridManager;
        [Inject(Id = "uiCamera")] private Camera uiCamera;
        
        private Bounds _dungeonBounds;

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
            _dungeonBounds = dungeonGridManager.GetRoomBounds();
        }

        private void LateUpdate()
        {
            var targetCameraPosition = ClampMousePosition(uiCamera.ScreenToWorldPoint(Input.mousePosition));
            var cameraTransform = mainCamera.transform; 
            cameraTransform.position = new Vector3(
                (targetCameraPosition.x + cameraTransform.position.x) / 2f,
                (targetCameraPosition.y + cameraTransform.position.y) / 2f,
                -10f);
            // mainCamera.transform.position = targetCameraPosition;

            if (handManager.SelectedRoomCardView.IsNotPresent && Input.mouseScrollDelta.y != 0)
            {
                mainCamera.fieldOfView = ClampCameraZoom(mainCamera.fieldOfView - Mathf.Sign(Input.mouseScrollDelta.y) * zoomSpeed);   
            }
        }

        private float ClampCameraZoom(float cameraZoom)
        {
            return Mathf.Clamp(cameraZoom, minZoom, maxZoom);
        }

        private Vector3 ClampMousePosition(Vector3 mouseWorldPosition)
        {
            return new Vector3(
                Mathf.Clamp(mouseWorldPosition.x, _dungeonBounds.min.x - 1, _dungeonBounds.max.x + 1),
                Mathf.Clamp(mouseWorldPosition.y, _dungeonBounds.min.y - 1, _dungeonBounds.max.y + 1),
                -10f);
        }
    }
}