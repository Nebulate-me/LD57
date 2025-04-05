using _Scripts.Cards;
using UnityEngine;

namespace _Scripts.Rooms
{
    public class DungeonRoomGhostView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color validPlacementColor;
        [SerializeField] private Color invalidPlacementColor;
        
        public void SetUpValid(RoomDto room)
        {
            spriteRenderer.sprite = room.Sprite;
            spriteRenderer.color = validPlacementColor;
        }

        public void SetUpInvalid(RoomDto room)
        {
            spriteRenderer.sprite = room.Sprite;
            spriteRenderer.color = invalidPlacementColor;
        }
    }
}