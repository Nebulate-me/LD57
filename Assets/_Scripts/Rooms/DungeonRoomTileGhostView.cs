using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileGhostView : MonoBehaviour
    {
        [SerializeField] private Transform wallSpriteTransform;
        [SerializeField] private SpriteRenderer wallSpriteRenderer;
        
        [SerializeField] private Transform furnitureSpriteTransform;
        [SerializeField] private SpriteRenderer furnitureSpriteRenderer;
        
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;

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