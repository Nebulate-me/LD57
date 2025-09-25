using System.Collections.Generic;
using System.Linq;
using _Scripts.Game;
using _Scripts.Missions;
using _Scripts.Rooms;
using _Scripts.Utils;
using ModestTree;
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
        
        [Inject] private IMissionManager _missionManager;
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
            if (!_currentLevel) return false;
            
            var preferredAvailableRooms = GetPreferredAvailableRooms(handRooms);

            roomDto = preferredAvailableRooms.Count <= 0 
                ? _randomService.Sample(_currentLevel.AvailableRooms).ToDto() 
                : _randomService.Sample(preferredAvailableRooms).ToDto();
            
            UpdateRemainingCardsText();
            return true;
        }

        private List<Room> GetPreferredAvailableRooms(IEnumerable<RoomDto> handRooms)
        {
            var handRoomsList = handRooms.ToList();
            
            const RoomType hallwayRoomType = RoomType.Hallway;
            if (DoesNotContainRoomTypes(handRoomsList, new List<RoomType>{ hallwayRoomType }))
            {
                return _currentLevel.AvailableRooms.Where(room => room.HasRoomType(hallwayRoomType)).ToList();
            }

            var availableNonHandRooms = _currentLevel.AvailableRooms
                .Where(room => room && handRoomsList.All(handRoom => !handRoom.HasAnyRoomTypes(room.RoomTypes)))
                .ToList();

            if (_missionManager.UnfulfilledRoomTypeRequirements.Any())
            {
                var missionFulfillingNonHandCards = availableNonHandRooms
                    .Where(room => room.HasAnyRoomTypes(_missionManager.UnfulfilledRoomTypeRequirements))
                    .ToList();
                
                return missionFulfillingNonHandCards.IsEmpty() ?  availableNonHandRooms : missionFulfillingNonHandCards;
            }
            
            return availableNonHandRooms;
        }

        private bool DoesNotContainRoomTypes(IEnumerable<RoomDto> handRooms, List<RoomType> roomTypes)
        {
            return handRooms.None(handRoom => handRoom.HasAnyRoomTypes(roomTypes));
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