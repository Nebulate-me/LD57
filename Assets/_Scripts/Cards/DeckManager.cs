using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Cards
{
    public class DeckManager : MonoBehaviour, IDeckManager
    {
        [SerializeField] private TextMeshProUGUI remainingCardsText;
        [SerializeField] private List<RoomCardAmount> initialRooms = new();

        [Header("Do Not Edit")]
        [SerializeField] private List<RoomCard> cards = new();

        [Inject] private IRandomService randomService;
        private int RemainingCardsAmount => cards.Count;

        private void Start()
        {
            cards = new List<RoomCard>();
            foreach (var initialRoom in initialRooms)
            {
                for (var i = 0; i < initialRoom.Amount; i++)
                {
                    cards.Add(initialRoom.Room.ToCard());   
                }
            }
            randomService.ShuffleInPlace(cards);

            UpdateRemainingCardsText();
        }

        private void UpdateRemainingCardsText()
        {
            remainingCardsText.text = RemainingCardsAmount.ToString();
        }

        public bool TryDraw(out RoomCard card)
        {
            card = null;
            if (RemainingCardsAmount <= 0) return false;

            card = cards.First();
            cards.RemoveAt(0);
            UpdateRemainingCardsText();
            return true;
        }

        public void Bury(List<RoomCard> cardsToBury)
        {
            cards.AddRange(cardsToBury);
        }
    }
}