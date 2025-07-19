using _Scripts.Cards;
using _Scripts.RoomTiles;
using UnityEngine;

namespace _Scripts.Rooms
{
    public class DungeonRoomGhostView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color validPlacementColor;
        [SerializeField] private Color invalidPlacementColor;
        
        public void SetUpValid(RoomTileDto roomTile)
        {
            spriteRenderer.sprite = roomTile.UnusedSprite;
            spriteRenderer.color = validPlacementColor;
        }

        public void SetUpInvalid(RoomTileDto roomTile)
        {
            spriteRenderer.sprite = roomTile.UnusedSprite;
            spriteRenderer.color = invalidPlacementColor;
        }
    }
}