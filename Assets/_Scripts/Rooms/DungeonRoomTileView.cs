using System.Collections.Generic;
using System.Linq;
using _Scripts.RoomTiles;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileView : MonoBehaviour, IPoolableResource
    {
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects = new();
        
        [ShowInInspector, ReadOnly] private Vector2Int gridPosition;
        [ShowInInspector, ReadOnly] private RoomDirection direction;
        [ShowInInspector, ReadOnly] private List<RoomDirection> openDirections = new();
        [ShowInInspector, ReadOnly] private List<RoomDirection> doorDirections = new();
        [ShowInInspector, ReadOnly] private bool isUsed = false;
        
        private RoomTileDto _tileDto;

        public Vector2Int GridPosition => gridPosition;
        public List<RoomDirection> OpenDirections => openDirections;
        public List<RoomDirection> DoorDirections => doorDirections;

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
            direction = initialDirection;
            spriteTransform.rotation = direction.ToRotation();
            spriteRenderer.sprite = _tileDto.UnusedSprite;

            openDirections = _tileDto.OpenDirections;
            doorDirections = _tileDto.DoorDirections;
            
            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);            
            }
            
            foreach (var doorDirection in doorDirections)
            {
                doorObjects[doorDirection].SetActive(true);
            }
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