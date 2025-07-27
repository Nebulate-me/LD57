using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Rooms
{
    public class DungeonRoomTileGhostView : MonoBehaviour
    {
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;

        public Color Color
        {
            set => spriteRenderer.color = value;
        }

        public Sprite Sprite
        {
            set => spriteRenderer.sprite = value;
        }

        public void SetUp(RoomTileCellDto roomTileCell)
        {
            Sprite = roomTileCell.Tile.UnusedSprite;
            spriteTransform.rotation = roomTileCell.Direction.ToRotation();
            
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