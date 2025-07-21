using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Cards
{
    public class DeckManager : MonoBehaviour, IDeckManager
    {
        [SerializeField] private TextMeshProUGUI remainingCardsText;
        [FormerlySerializedAs("initialRooms")] [SerializeField] private List<RoomTileAmountDto> initialRoomTiles = new();
        [SerializeField] private List<RoomAmountDto> initialRooms = new();
        
        [ShowInInspector, ReadOnly] private List<RoomTileDto> _roomTileCards = new();
        [ShowInInspector, ReadOnly] private List<RoomDto> _roomCards = new();
        

        [Inject] private IRandomService _randomService;

        private void Start()
        {
            _roomTileCards = new List<RoomTileDto>();
            foreach (var initialRoomTile in initialRoomTiles)
            {
                for (var i = 0; i < initialRoomTile.Amount; i++)
                {
                    _roomTileCards.Add(initialRoomTile.RoomTile.ToDto());   
                }
            }
            _randomService.ShuffleInPlace(_roomTileCards);
            
            _roomCards = new List<RoomDto>();
            foreach (var initialRoom in initialRooms)
            {
                for (var i = 0; i < initialRoom.Amount; i++)
                {
                    _roomCards.Add(initialRoom.Room.ToDto());   
                }
            }
            _randomService.ShuffleInPlace(_roomCards);
            
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());

            UpdateRemainingCardsText();
        }

        private void UpdateRemainingCardsText()
        {
            remainingCardsText.text = RoomCardAmount.ToString();
        }

        #region RoomTiles
        public int RoomTileCardAmount => _roomTileCards.Count;

        public bool TryDrawRoomTile(out RoomTileDto tileDto)
        {
            tileDto = null;
            if (RoomTileCardAmount <= 0) return false;

            tileDto = _roomTileCards.First();
            _roomTileCards.RemoveAt(0);
            UpdateRemainingCardsText();
            return true;
        }

        public void BuryRoomTile(List<RoomTileDto> cardsToBury)
        {
            _roomTileCards.AddRange(cardsToBury);
            UpdateRemainingCardsText();
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());
        }

        #endregion


        #region Rooms
        public int RoomCardAmount => _roomCards.Count;

        public bool TryDrawRoom(out RoomDto roomDto)
        {
            roomDto = null;
            if (RoomCardAmount <= 0) return false;

            roomDto = _roomCards.First();
            _roomCards.RemoveAt(0);
            UpdateRemainingCardsText();
            return true;
        }

        public void BuryRoom(List<RoomDto> cardsToBury)
        {
            _roomCards.AddRange(cardsToBury);
            UpdateRemainingCardsText();
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());
        }

        #endregion
        
    }
}