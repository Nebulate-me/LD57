using System.Collections.Generic;
using System.Linq;
using _Scripts.RoomTiles;
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
        
        private RoomTileDto _tileDto;

        public Vector2Int GridPosition => gridPosition;
        public List<RoomDirection> OpenDirections => openDirections;

        public bool IsUsed
        {
            get => isUsed;
            set
            {
                isUsed = value;
                spriteRenderer.sprite = isUsed ? _tileDto.UsedSprite : _tileDto.UnusedSprite;
            }
        }

        public void SetUp(RoomTileDto roomTileDto, Vector2Int initialGridPosition, RoomDirection initialDirection)
        {
            _tileDto = roomTileDto;
            gridPosition = initialGridPosition;
            spriteRenderer.sprite = _tileDto.UnusedSprite;
            direction = initialDirection;
            transform.rotation = direction.ToRotation();

            openDirections = _tileDto.OpenDirections.Select(openDirection => openDirection.Rotate(direction)).ToList();
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