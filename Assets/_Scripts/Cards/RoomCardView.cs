using _Scripts.Rooms;
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
        
        private RoomDto dto;
        public RoomDto Dto => dto;


        public void SetUp(RoomDto roomDto)
        {
            dto = roomDto;
            roomName.text = roomDto.Name;
            roomImage.sprite = roomDto.UnusedSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            handManager.SelectRoomCard(dto);
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