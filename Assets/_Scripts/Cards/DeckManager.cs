using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using Signals;
using TMPro;
using UnityEngine;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Cards
{
    public class DeckManager : MonoBehaviour, IDeckManager
    {
        [SerializeField] private TextMeshProUGUI remainingCardsText;
        [SerializeField] private List<RoomAmountDto> initialRooms = new();

        [Header("Do Not Edit")]
        [SerializeField] private List<RoomDto> cards = new();

        [Inject] private IRandomService randomService;
        public int CardAmount => cards.Count;

        private void Start()
        {
            cards = new List<RoomDto>();
            foreach (var initialRoom in initialRooms)
            {
                for (var i = 0; i < initialRoom.Amount; i++)
                {
                    cards.Add(initialRoom.Room.ToDto());   
                }
            }
            randomService.ShuffleInPlace(cards);
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());

            UpdateRemainingCardsText();
        }

        private void UpdateRemainingCardsText()
        {
            remainingCardsText.text = CardAmount.ToString();
        }
        

        public bool TryDraw(out RoomDto dto)
        {
            dto = null;
            if (CardAmount <= 0) return false;

            dto = cards.First();
            cards.RemoveAt(0);
            UpdateRemainingCardsText();
            return true;
        }

        public void Bury(List<RoomDto> cardsToBury)
        {
            cards.AddRange(cardsToBury);
            UpdateRemainingCardsText();
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());
        }
    }
}