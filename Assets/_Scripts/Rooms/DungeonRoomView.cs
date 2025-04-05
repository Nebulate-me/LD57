using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using UnityEngine;

namespace _Scripts.Rooms
{
    public class DungeonRoomView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public Vector2Int GridPosition { get; set; }
        public List<RoomDirection> OpenDirections = new();

        public void SetUp(RoomDto dto, Vector2Int gridPosition, RoomDirection direction)
        {
            GridPosition = gridPosition;
            spriteRenderer.sprite = dto.Sprite;
            transform.rotation = direction.ToRotation();

            OpenDirections = dto.OpenDirections.Select(openDirection => openDirection.Rotate(direction)).ToList();
        }
    }
}