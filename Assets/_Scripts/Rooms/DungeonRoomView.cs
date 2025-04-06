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
        
        private RoomDto dto;

        public Vector2Int GridPosition => gridPosition;
        public List<RoomDirection> OpenDirections => openDirections;

        public bool IsUsed
        {
            get => isUsed;
            set
            {
                isUsed = value;
                spriteRenderer.sprite = isUsed ? dto.UsedSprite : dto.UnusedSprite;
            }
        }

        public void SetUp(RoomDto roomDto, Vector2Int initialGridPosition, RoomDirection initialDirection)
        {
            dto = roomDto;
            gridPosition = initialGridPosition;
            spriteRenderer.sprite = dto.UnusedSprite;
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