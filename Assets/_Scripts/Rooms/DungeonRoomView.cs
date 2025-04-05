using _Scripts.Cards;
using UnityEngine;

namespace _Scripts.Rooms
{
    public class DungeonRoomView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetUp(RoomCard card, RoomDirection direction)
        {
            spriteRenderer.sprite = card.Sprite;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, direction.ToRotation()));
        }
    }
}