using System.Collections.Generic;
using System.Linq;
using _Scripts.Game;
using _Scripts.Rooms;
using Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Cards
{
    public class DeckManager : MonoBehaviour, IDeckManager
    {
        [SerializeField] private TextMeshProUGUI remainingCardsText;
        
        [ShowInInspector, ReadOnly] private int _remainingRoomCards;
        [ShowInInspector, ReadOnly] private Level _currentLevel;
        
        [Inject] private IRandomService _randomService;
        

        private void OnEnable()
        {
            SignalsHub.AddListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnLevelSetupCompleted(LevelSetupCompletedSignal signal)
        {
            _currentLevel = signal.Level;
            _remainingRoomCards = _currentLevel.InitialRemainingRoomCards;
            
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());

            UpdateRemainingCardsText();
        }

        private void UpdateRemainingCardsText()
        {
            remainingCardsText.text = RoomCardAmount.ToString();
        }


        #region Rooms
        public int RoomCardAmount => _remainingRoomCards;

        public bool TryDrawRoom(IEnumerable<RoomDto> handRooms, out RoomDto roomDto)
        {
            roomDto = null;
            if (RoomCardAmount <= 0 || !_currentLevel) return false;

            var nonPresentAvailableRooms = _currentLevel.AvailableRooms
                .Where(room => handRooms.All(handRoom => !handRoom.HasAnyRoomTypes(room.RoomTypes))).ToList();

            roomDto = nonPresentAvailableRooms.Count <= 0 
                ? _randomService.Sample(_currentLevel.AvailableRooms).ToDto() 
                : _randomService.Sample(nonPresentAvailableRooms).ToDto();
            
            _remainingRoomCards--;
            UpdateRemainingCardsText();
            return true;
        }

        public void BuryRoom(List<RoomDto> cardsToBury)
        {
            _remainingRoomCards += cardsToBury.Count;
            UpdateRemainingCardsText();
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());
        }

        public void BuryRoom(RoomDto cardToBury)
        {
            _remainingRoomCards++;
            UpdateRemainingCardsText();
            SignalsHub.DispatchAsync(new DeckUpdatedSignal());
        }

        #endregion
        
    }
}