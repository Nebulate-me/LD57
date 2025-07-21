using _Scripts.Rooms;
using _Scripts.RoomTiles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomTileCardView : MonoBehaviour, IPointerClickHandler, IPoolableResource
    {
        [FormerlySerializedAs("roomName")] [SerializeField] private TextMeshProUGUI roomTileName;
        [FormerlySerializedAs("roomImage")] [SerializeField] private Image roomTileImage;
        [SerializeField] private Image cardBackgroundImage;

        [Space] 
        [SerializeField] private Sprite deselectedCardBackgroundSprite;
        [SerializeField] private Sprite selectedCardBackgroundSprite;

        [Inject] private IHandManager _handManager;
        
        private RoomTileDto _tileDto;
        public RoomTileDto TileDto => _tileDto;


        public void SetUp(RoomTileDto roomTileDto)
        {
            _tileDto = roomTileDto;
            roomTileName.text = roomTileDto.Name;
            roomTileImage.sprite = roomTileDto.UnusedSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _handManager.SelectRoomTileCard(_tileDto);
        }

        public void Select()
        {
            cardBackgroundImage.sprite = selectedCardBackgroundSprite;
        }
        public void Deselect()
        {
            cardBackgroundImage.sprite = deselectedCardBackgroundSprite;
        }

        public void OnDespawn()
        {
            Deselect();
        }

        public void OnSpawn()
        {
            Deselect();
        }
    }
}