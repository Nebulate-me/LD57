using _Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.RoomTiles
{
    public class RoomTileCellView : MonoBehaviour
    {
        [SerializeField] private RectTransform tileTransform;
        [SerializeField] private Image tileImage;
        public void SetUp(RoomTileCell roomTileCell)
        {
            tileImage.sprite = roomTileCell.Tile.UsedSprite;
            tileTransform.rotation = roomTileCell.Direction.ToRotation();
        }
    }
}