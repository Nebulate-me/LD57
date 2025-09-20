using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Game;
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
    public class HandManager : MonoBehaviour, IHandManager, IInitializable
    {
        [FormerlySerializedAs("roomCardPrefab")] [SerializeField] private GameObject roomTileCardPrefab;
        [SerializeField] private GameObject roomCardPrefab;
        [SerializeField] private RectTransform roomCardContainer;
        [SerializeField] private int handSize = 5;

        private readonly List<RoomTileCardView> _roomTileCardViews = new();
        private readonly List<RoomCardView> _roomCardViews = new();
        private readonly Stack<RoomDto> _playedRoomCards = new();
        private Level _currentLevel;
        
        [Inject] private IDeckManager _deckManager;
        [Inject] private IPrefabPool _prefabPool;

        private void OnEnable()
        {
            SignalsHub.AddListener<DeckUpdatedSignal>(OnDeckUpdated);
            SignalsHub.AddListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<DeckUpdatedSignal>(OnDeckUpdated);
            SignalsHub.RemoveListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }
        
        public void Initialize()
        {
            roomCardContainer.DestroyChildren();
        }

        private void OnDeckUpdated(DeckUpdatedSignal signal)
        {
            StartCoroutine(RefillRoomHandCoroutine());
        }

        private IEnumerator RefillRoomHandCoroutine()
        {
            // waiting until unfulfilled requirements will get fulfilled
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            RefillRoomHandImmediate();
        }

        private void OnLevelSetupCompleted(LevelSetupCompletedSignal signal)
        {
            _currentLevel = signal.Level;

            var viewsToDespawn = new List<Transform>();
            foreach (Transform roomCardView in roomCardContainer.transform)
            {
                viewsToDespawn.Add(roomCardView);
            }
            _roomCardViews.Clear();
            
            foreach (var room in _currentLevel.InitialRooms)
            {
                var cardView = _prefabPool.Spawn(roomCardPrefab, roomCardContainer).GetComponent<RoomCardView>();
                cardView.SetUp(room.ToDto());
                _roomCardViews.Add(cardView);
            }

            foreach (var view in viewsToDespawn)
            {
                _prefabPool.Despawn(view.gameObject);
            }
            _playedRoomCards.Clear();
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
                _playedRoomCards.Push(cardView.RoomDto);

                return true;
            }

            return false;
        }
        
        public void RefillRoomHand()
        {
            StartCoroutine(RefillRoomHandCoroutine());
        }

        private void RefillRoomHandImmediate()
        {
            while (_roomCardViews.Count < handSize)
            {
                if (_deckManager.TryDrawRoom(_roomCardViews.Select(view => view.RoomDto), out var card))
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

        public void RedrawRoomHand()
        {
            while (_roomCardViews.TryRemoveFirst(out var roomCard))
            {
                _prefabPool.Despawn(roomCard.gameObject);
                _deckManager.BuryRoom(roomCard.RoomDto);
            }
            RefillRoomHand();
        }

        public bool TryUnplayLastCard()
        {
            if (!_playedRoomCards.TryPop(out var card)) return false;
            
            DeselectRoomCard();
            var lastDrawnCard = _roomCardViews.Last();
            _roomCardViews.Remove(lastDrawnCard);
            _prefabPool.Despawn(lastDrawnCard.gameObject);
            
            var cardView = _prefabPool.Spawn(roomCardPrefab, roomCardContainer).GetComponent<RoomCardView>();
            cardView.SetUp(card);
            _roomCardViews.Insert(0, cardView);
            return true;
        }

        #endregion
    }
}