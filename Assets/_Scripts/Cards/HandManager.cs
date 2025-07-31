using System.Collections.Generic;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using Signals;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Monads;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class HandManager : MonoBehaviour, IHandManager
    {
        [FormerlySerializedAs("roomCardPrefab")] [SerializeField] private GameObject roomTileCardPrefab;
        [SerializeField] private GameObject roomCardPrefab;
        [SerializeField] private RectTransform roomCardContainer;
        [SerializeField] private int handSize = 5;

        private readonly List<RoomTileCardView> _roomTileCardViews = new();
        private readonly List<RoomCardView> _roomCardViews = new();

        [Inject] private IDeckManager _deckManager;
        [Inject] private IPrefabPool _prefabPool;

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
            // RefillRoomTileHand();
            RefillRoomHand();
        }

        private void Start()
        {
            roomCardContainer.DestroyChildren();

            RefillRoomHand();
        }

        public int CardAmount => _roomTileCardViews.Count;

        #region Room Tiles

        public IMaybe<RoomTileCardView> SelectedRoomTileCardView { get; private set; } = Maybe.Empty<RoomTileCardView>();

        public bool TryPlaySelectRoomTileCard()
        {
            if (SelectedRoomTileCardView.TryGetValue(out var cardView))
            {
                _roomTileCardViews.Remove(cardView);
                _prefabPool.Despawn(cardView.gameObject);
                SelectedRoomTileCardView = Maybe.Empty<RoomTileCardView>();

                return true;
            }

            return false;
        }

        public bool SelectRoomTileCard(RoomTileDto tileDto)
        {
            SelectedRoomTileCardView = Maybe.Empty<RoomTileCardView>();
            foreach (var cardView in _roomTileCardViews)
                if (cardView.TileDto == tileDto)
                {
                    cardView.Select();
                    SelectedRoomTileCardView = Maybe.Of(cardView);
                }
                else
                {
                    cardView.Deselect();
                }
            SignalsHub.DispatchAsync(new RoomCardSelectedSignal());
            
            return true;
        }

        public bool DeselectRoomTileCard()
        {
            if (SelectedRoomTileCardView.TryGetValue(out var cardView))
            {
                SelectedRoomTileCardView = Maybe.Empty<RoomTileCardView>();
                cardView.Deselect();
                return true;
            }

            return false;
        }

        #endregion

        #region Rooms

        public IMaybe<RoomCardView> SelectedRoomCardView { get; private set; } = Maybe.Empty<RoomCardView>();

        public void SelectRoomCard(RoomDto roomDto)
        {
            SelectedRoomCardView = Maybe.Empty<RoomCardView>();
            foreach (var cardView in _roomCardViews)
                if (cardView.RoomDto == roomDto)
                {
                    cardView.Select();
                    SelectedRoomCardView = Maybe.Of(cardView);
                }
                else
                {
                    cardView.Deselect();
                }
            SignalsHub.DispatchAsync(new RoomCardSelectedSignal());
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
        
        public bool TryPlaySelectRoomCard()
        {
            if (SelectedRoomCardView.TryGetValue(out var cardView))
            {
                _roomCardViews.Remove(cardView);
                _prefabPool.Despawn(cardView.gameObject);
                SelectedRoomCardView = Maybe.Empty<RoomCardView>();

                return true;
            }

            return false;
        }
        
        public void RefillRoomHand()
        {
            while (_roomCardViews.Count < handSize)
            {
                if (_deckManager.TryDrawRoom(out var card))
                {
                    var cardView = _prefabPool.Spawn(roomCardPrefab, roomCardContainer).GetComponent<RoomCardView>();
                    cardView.SetUp(card);
                    _roomCardViews.Add(cardView);
                }
                else
                {
                    // Debug.Log("No cards left in the deck, not drawing!");
                    break;
                }
            }
        }

        #endregion
    }
}