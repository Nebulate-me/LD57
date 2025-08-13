using _Scripts.RoomTiles;
using UnityEngine;
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

        [Inject] private IRoomRegistry _roomRegistry;

        public Color Color
        {
            set => wallSpriteRenderer.color = value;
        }

        public void SetUp(RoomTileCellDto roomTileCell)
        {
            wallSpriteRenderer.sprite = roomTileCell.Tile.UnusedSprite;
            wallSpriteTransform.rotation = roomTileCell.Direction.ToRotation();
            
            furnitureSpriteRenderer.gameObject.SetActive(roomTileCell.FurnitureSprite != null); 
            furnitureSpriteRenderer.sprite = roomTileCell.FurnitureSprite;
            furnitureSpriteTransform.rotation = roomTileCell.Direction.Rotate(roomTileCell.FurnitureDirection).ToRotation();

            floorRenderer.sprite = _roomRegistry.SharedRoomFloorSprite;
            
            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);            
            }
            
            foreach (var doorDirection in roomTileCell.Tile.DoorDirections)
            {
                doorObjects[doorDirection].SetActive(true);
            }
        }
    }
}