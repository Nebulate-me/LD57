using _Scripts.Rooms;
using _Scripts.RoomTiles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomCardView : MonoBehaviour, IPointerClickHandler, IPoolableResource
    {
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private Image roomImage;
        [SerializeField] private Image cardBackgroundImage;

        [Space] 
        [SerializeField] private Sprite deselectedCardBackgroundSprite;
        [SerializeField] private Sprite selectedCardBackgroundSprite;

        [Inject] private IHandManager handManager;
        
        private RoomTileDto _tileDto;
        public RoomTileDto TileDto => _tileDto;


        public void SetUp(RoomTileDto roomTileDto)
        {
            _tileDto = roomTileDto;
            roomName.text = roomTileDto.Name;
            roomImage.sprite = roomTileDto.UnusedSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            handManager.SelectRoomCard(_tileDto);
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