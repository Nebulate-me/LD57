using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Cards
{
    public class RoomCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private Image roomImage;
        
        private RoomCard _card;

        public void SetUp(RoomCard card)
        {
            _card = card;
            roomName.text = card.Name;
            roomImage.sprite = card.Sprite;
        }
    }
}