using _Scripts.RoomTiles;
using _Scripts.Utils;
using UnityEngine;
using Utilities;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileGhostView : MonoBehaviour
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

        public Color Color
        {
            set => wallSpriteRenderer.color = value;
        }

        public void SetUp(RoomTileCellDto roomTileCell, RoomDto roomDto)
        {
            wallSpriteRenderer.sprite = roomTileCell.Tile.UnusedSprite;
            wallSpriteTransform.rotation = roomTileCell.Direction.ToRotation();
            
            furnitureSpriteRenderer.gameObject.SetActive(roomTileCell.FurnitureSprite != null); 
            furnitureSpriteRenderer.sprite = roomTileCell.FurnitureSprite;
            furnitureSpriteTransform.rotation = roomTileCell.Direction.Rotate(roomTileCell.FurnitureDirection).ToRotation();

            floorRenderer.sprite = _roomRegistry.SharedRoomFloorSprite;
            
            var tilePosition = _dungeonGridManager.WorldToGrid(transform.position.ToVector2());
            foreach (var (windowDirection, windowObject) in windowObjects)
            {
                var isWindowActive = _dungeonGridManager.IsTileAdjacentToLevelBounds(tilePosition, windowDirection);
                windowObject.SetActive(isWindowActive);
            }

            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);            
            }
            
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
                    _dungeonGridManager.IsTileAdjacentToDoorOrEmpty(tilePosition, doorDirection, out var adjacentTile);
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