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
        
        private RoomCard card;
        public RoomCard Card => card;


        public void SetUp(RoomCard roomCard)
        {
            card = roomCard;
            roomName.text = roomCard.Name;
            roomImage.sprite = roomCard.Sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            handManager.SelectRoomCard(card);
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