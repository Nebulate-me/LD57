using System.Collections.Generic;
using _Scripts.Rooms;
using _Scripts.Utils;
using Signals;
using UnityEngine;
using Utilities;
using Utilities.Monads;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class HandManager : MonoBehaviour, IHandManager
    {
        [SerializeField] private GameObject roomCardPrefab;
        [SerializeField] private RectTransform roomCardContainer;
        [SerializeField] private int handSize = 5;

        private readonly List<RoomCardView> cardViews = new();

        [Inject] private IDeckManager deckManager;
        [Inject] private IPrefabPool prefabPool;

        private void OnEnable()
        {
            SignalsHub.AddListener<DeckUpdatedSignal>(OnDeckUpdated);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<DeckUpdatedSignal>(OnDeckUpdated);
        }

        private void OnDeckUpdated(DeckUpdatedSignal signal)
        {
            RefillHand();
        }

        private void Start()
        {
            roomCardContainer.DestroyChildren();

            RefillHand();
        }

        public void RefillHand()
        {
            while (cardViews.Count < handSize)
                if (deckManager.TryDraw(out var card))
                {
                    var cardView = prefabPool.Spawn(roomCardPrefab, roomCardContainer).GetComponent<RoomCardView>();
                    cardView.SetUp(card);
                    cardViews.Add(cardView);
                }
                else
                {
                    // Debug.Log("No cards left in the deck, not drawing!");
                    break;
                }
        }

        public int CardAmount => cardViews.Count;
        public IMaybe<RoomCardView> SelectedRoomCardView { get; private set; } = Maybe.Empty<RoomCardView>();

        public bool TryPlaySelectRoomCard()
        {
            if (SelectedRoomCardView.TryGetValue(out var cardView))
            {
                cardViews.Remove(cardView);
                prefabPool.Despawn(cardView.gameObject);
                SelectedRoomCardView = Maybe.Empty<RoomCardView>();

                return true;
            }

            return false;
        }

        public bool SelectRoomCard(RoomDto dto)
        {
            SelectedRoomCardView = Maybe.Empty<RoomCardView>();
            foreach (var cardView in cardViews)
                if (cardView.Dto == dto)
                {
                    cardView.Select();
                    SelectedRoomCardView = Maybe.Of(cardView);
                }
                else
                {
                    cardView.Deselect();
                }
            SignalsHub.DispatchAsync(new RoomCardSelectedSignal());
            
            return true;
        }

        public bool DeselectRoomCard()
        {
            if (SelectedRoomCardView.TryGetValue(out var cardView))
            {
                SelectedRoomCardView = Maybe.Empty<RoomCardView>();
                cardView.Deselect();
                return true;
            }

            return false;
        }
    }
}