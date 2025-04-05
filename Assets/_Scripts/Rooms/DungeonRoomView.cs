using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Prefabs;

namespace _Scripts.Rooms
{
    public class DungeonRoomView : MonoBehaviour, IPoolableResource
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Do Not Edit")] 
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private RoomDirection direction;
        [SerializeField] private List<RoomDirection> openDirections = new();
        [SerializeField] private bool isUsed = false;
        
        public Vector2Int GridPosition => gridPosition;
        public RoomDirection Direction => direction;
        public List<RoomDirection> OpenDirections => openDirections;
        
        public bool IsUsed
        {
            get => isUsed;
            set => isUsed = value;
        }

        public void SetUp(RoomDto dto, Vector2Int initialGridPosition, RoomDirection initialDirection)
        {
            gridPosition = initialGridPosition;
            spriteRenderer.sprite = dto.Sprite;
            direction = initialDirection;
            transform.rotation = direction.ToRotation();

            openDirections = dto.OpenDirections.Select(openDirection => openDirection.Rotate(direction)).ToList();
        }
        
        public void OnSpawn()
        {
            isUsed = false;
        }

        public void OnDespawn()
        {
            isUsed = false;
        }
    }
}