using System.Collections.Generic;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using UnityEngine;
using Utilities;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileGhostView : MonoBehaviour, IDungeonRoomTileView
    {
        [SerializeField] private Transform wallSpriteTransform;
        [SerializeField] private SpriteRenderer wallSpriteRenderer;
        [Space]
        [SerializeField] private Transform furnitureSpriteTransform;
        [SerializeField] private SpriteRenderer furnitureSpriteRenderer;
        [Space]
        [SerializeField] private SpriteRenderer floorRenderer;
        [Space]
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;
        [Space]
        [SerializeField] private RoomDirectionToGameObjectDictionary windowObjects;

        [Inject] private IRoomRegistry _roomRegistry;
        [Inject] private IDungeonGridManager _dungeonGridManager;
        
        private RoomTileCellDto _roomTile;

        public Color Color
        {
            set => wallSpriteRenderer.color = value;
        }
        
        public bool IsConnected
        {
            set => wallSpriteRenderer.sprite = value ? _roomTile.Tile.ConnectedSprite : _roomTile.Tile.UnusedSprite;
        }
        
        public Vector2Int GridPosition { get; private set; }
        public List<RoomDirection> DoorDirections { get; private set; }

        public void SetUp(RoomTileCellDto roomTileCell, RoomDto roomDto)
        {
            _roomTile = roomTileCell;
            
            wallSpriteRenderer.sprite = roomTileCell.Tile.UnusedSprite;
            wallSpriteTransform.rotation = roomTileCell.Direction.ToRotation();
            
            furnitureSpriteRenderer.gameObject.SetActive(roomTileCell.FurnitureSprite != null); 
            furnitureSpriteRenderer.sprite = roomTileCell.FurnitureSprite;
            furnitureSpriteTransform.rotation = roomTileCell.Direction.Rotate(roomTileCell.FurnitureDirection).ToRotation();

            floorRenderer.sprite = _roomRegistry.SharedRoomFloorSprite;
            
            GridPosition = _dungeonGridManager.WorldToGrid(transform.position.ToVector2());
            foreach (var (windowDirection, windowObject) in windowObjects)
            {
                var isWindowActive = _dungeonGridManager.IsTileAdjacentToLevelBounds(GridPosition, windowDirection);
                windowObject.SetActive(isWindowActive);
            }

            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);            
            }

            DoorDirections = roomTileCell.Tile.DoorDirections;
            foreach (var doorDirection in roomTileCell.Tile.DoorDirections)
            {
                var door = doorObjects[doorDirection];
                
                var isActiveWindow = windowObjects[doorDirection].activeSelf;
                if (isActiveWindow)
                {
                    door.SetActive(false);
                    continue;
                }
                
                var isAdjacentDoorOrEmpty =
                    _dungeonGridManager.IsTileAdjacentToDoorOrEmpty(GridPosition, doorDirection, out var adjacentTile);
                if (!isAdjacentDoorOrEmpty)
                {
                    door.SetActive(false);
                    continue;
                }

                if (adjacentTile != null && 
                    adjacentTile.RoomDto.HasRoomType(RoomType.Shared) &&
                    !roomDto.HasAnyRoomTypes(RoomTypeExtensions.ApartmentStartingRoomTypes))
                {
                    door.SetActive(false);
                    continue;
                }
                
                doorObjects[doorDirection].SetActive(true);
            }
        }
    }
}