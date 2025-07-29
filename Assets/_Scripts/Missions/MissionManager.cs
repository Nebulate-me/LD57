using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Missions.Apartment;
using _Scripts.Missions.Pattern;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using Signals;
using UnityEngine;
using Utilities;
using Utilities.Monads;
using Utilities.Prefabs;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Missions
{
    public class MissionManager : MonoBehaviour, IMissionManager
    {
        [SerializeField] private RectTransform missionContainer;
        [SerializeField] private GameObject apartmentMissionCardPrefab;
        [SerializeField] private List<ApartmentMission> availableMissions = new();
        [SerializeField] private ApartmentMission initialMission;
        [SerializeField] private int missionHandSize = 1;
        [SerializeField] private List<int> missionHandSizeIncreases = new() {2, 5, 10};

        [Inject] private IPrefabPool _prefabPool;
        [Inject] private IRandomService _randomService;
        [Inject] private IDungeonGridManager _dungeonGridManager;
        [Inject] private IDeckManager _deckManager;
        [Inject] private ISoundManager _soundManager;

        private readonly List<PatternMissionCardView> _patternMissionCardViews = new();
        private readonly List<ApartmentMissionCardView> _apartmentMissionCardViews = new();
        private int _completedMissionCount;
        private string _lastCompletedMissionName;

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomTilePlacedSignal>(OnRoomPlaced);
        }
        
        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomTilePlacedSignal>(OnRoomPlaced);
        }

        private void OnRoomPlaced(RoomTilePlacedSignal roomTilePlacedSignal)
        {
            UpdateMissions();
        }

        private void Start()
        {
            missionContainer.DestroyChildren();
            
            var missionDto = initialMission.ToDto();
            var missionCardView = _prefabPool.Spawn(apartmentMissionCardPrefab, missionContainer).GetComponent<ApartmentMissionCardView>();
            missionCardView.SetUp(missionDto);
            missionCardView.Completable = false;
            _apartmentMissionCardViews.Add(missionCardView);
        }

        public int CompletableMissionsCount => _patternMissionCardViews.Count(mission => mission.Completable);

        public void CompleteMission(PatternMissionCardView patternMissionCard)
        {
            if (!IsPatternMissionCompletable(patternMissionCard.Dto, _dungeonGridManager.Rooms, out var roomsToUse)) return;

            foreach (var dungeonRoomView in roomsToUse) 
                dungeonRoomView.IsUsed = true;
            
            var shuffledMissionRewards = _randomService.Shuffle(patternMissionCard.Dto.RewardCards).ToList(); 
            _deckManager.BuryRoomTile(shuffledMissionRewards);
            _prefabPool.Despawn(patternMissionCard.gameObject);
            _patternMissionCardViews.Remove(patternMissionCard);
            
            _completedMissionCount++;
            if (missionHandSizeIncreases.Contains(_completedMissionCount)) 
                missionHandSize++;
            
            _soundManager.PlaySound(SoundType.CompleteMission);
            SignalsHub.DispatchAsync(new PatternMissionCompletedSignal(patternMissionCard.Dto));
            _lastCompletedMissionName = patternMissionCard.Dto.Name;
            
            UpdateMissions();
        }

        public void CompleteMission(ApartmentMissionCardView apartmentMissionCard)
        {
            if (!IsApartmentMissionCompletable(apartmentMissionCard.Dto, _dungeonGridManager.Rooms, out var roomsToUse)) return;

            foreach (var dungeonRoomView in roomsToUse) 
                dungeonRoomView.IsUsed = true;
            
            var shuffledMissionRewards = _randomService.Shuffle(apartmentMissionCard.Dto.RewardRooms).ToList(); 
            _deckManager.BuryRoom(shuffledMissionRewards);
            _prefabPool.Despawn(apartmentMissionCard.gameObject);
            _apartmentMissionCardViews.Remove(apartmentMissionCard);
            
            _completedMissionCount++;
            if (missionHandSizeIncreases.Contains(_completedMissionCount)) 
                missionHandSize++;
            
            _soundManager.PlaySound(SoundType.CompleteMission);
            SignalsHub.DispatchAsync(new ApartmentMissionCompletedSignal(apartmentMissionCard.Dto));
            _lastCompletedMissionName = apartmentMissionCard.Dto.Name;
            
            UpdateMissions();
        }

        private void UpdateMissions()
        {
            foreach (var missionCardView in _patternMissionCardViews)
            {
                missionCardView.Completable = IsPatternMissionCompletable(missionCardView.Dto, _dungeonGridManager.Rooms, out var _);
            }

            RefillMissionHand();
            SignalsHub.DispatchAsync(new MissionsUpdatedSignal());
        }
        
        private void RefillMissionHand()
        {
            var unlockedMissions = availableMissions
                .Where(mission => _completedMissionCount >= mission.MinCompletedMissions &&
                                  (mission.MaxCompletedMissions <= 0 || _completedMissionCount < mission.MaxCompletedMissions) && 
                                  mission.MissionName != _lastCompletedMissionName)
                .ToList();
            while (_apartmentMissionCardViews.Count < missionHandSize)
            {
                var missionDto = _randomService.Sample(unlockedMissions).ToDto();
                if (_apartmentMissionCardViews.Any(card => card.Dto.Name == missionDto.Name)) continue;
                var missionCardView = _prefabPool.Spawn(apartmentMissionCardPrefab, missionContainer).GetComponent<ApartmentMissionCardView>();
                missionCardView.SetUp(missionDto);
                missionCardView.Completable = IsApartmentMissionCompletable(missionCardView.Dto, _dungeonGridManager.Rooms, out var _);
                _apartmentMissionCardViews.Add(missionCardView);
            }
        }

        private bool IsApartmentMissionCompletable(object dto, IReadOnlyList<DungeonRoomView> rooms, out List<DungeonRoomView> roomsToUse)
        {
            roomsToUse = null;
            return true; // TODO: implement
        }

        private bool IsPatternMissionCompletable(PatternMissionDto patternMissionDto, IReadOnlyList<DungeonRoomView> rooms, out List<DungeonRoomView> roomsToUse)
        {
            roomsToUse = new List<DungeonRoomView>();
            var unusedRooms = rooms.Where(room => !room.IsUsed).ToList();
            var allDirections = EnumExtensions.GetAllItems<RoomDirection>().ToList();
            var normalizedPattern = NormalizePattern(patternMissionDto.Pattern);
            if (IsAnyPatternDirectionMatching(ref roomsToUse, allDirections, normalizedPattern, unusedRooms))
                return true;

            if (patternMissionDto.FlipPatternY)
            {
                var normalizedFlippedYPattern = NormalizePattern(FlipYPattern(patternMissionDto.Pattern));
                if (IsAnyPatternDirectionMatching(ref roomsToUse, allDirections, normalizedFlippedYPattern, unusedRooms)) 
                    return true;
            }
            
            return false;
        }

        private bool IsAnyPatternDirectionMatching(ref List<DungeonRoomView> roomsToUse, List<RoomDirection> allDirections, IReadOnlyCollection<MissionCell> normalizedFlippedYPattern,
            List<DungeonRoomView> unusedRooms)
        {
            foreach (var direction in allDirections)
            {
                var rotatedPattern = RotatePattern(normalizedFlippedYPattern, direction);
                foreach (var dungeonRoomView in unusedRooms)
                {
                    if (IsPatternMatching(rotatedPattern, dungeonRoomView, unusedRooms, out roomsToUse))
                        return true;
                }
            }

            return false;
        }

        private bool IsPatternMatching(List<MissionCell> rotatedPattern, DungeonRoomView startingRoom,
            List<DungeonRoomView> rooms, out List<DungeonRoomView> roomsToUse)
        {
            roomsToUse = new List<DungeonRoomView>();
            var startingPosition = startingRoom.GridPosition;
            foreach (var missionCell in rotatedPattern)
            {
                var maybeMatchingRoom = rooms.FirstOrEmpty(room => room.GridPosition == startingPosition + missionCell.Position);
                var matchingRoomExists = maybeMatchingRoom.TryGetValue(out var matchingRoom);
                switch (missionCell.Type)
                {
                    case MissionCellType.Any:
                        break;
                    case MissionCellType.Room:
                        if (!matchingRoomExists) return false;
                        if (!missionCell.OpenDirections.All(openDirection => matchingRoom.OpenDirections.Contains(openDirection))) return false;
                        if (missionCell.ClosedDirections.Any(closedDirection => matchingRoom.OpenDirections.Contains(closedDirection))) return false;
                        roomsToUse.Add(matchingRoom);
                        break;
                    case MissionCellType.Empty:
                        if (matchingRoomExists) return false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        private List<MissionCell> NormalizePattern(IReadOnlyCollection<MissionCell> missionDtoPattern)
        {
            var firstPatternCell = missionDtoPattern.First();
            var normalizeShift = new Vector2Int(firstPatternCell.Position.x, firstPatternCell.Position.y);
            return missionDtoPattern
                .Select(cell => new MissionCell(
                    cell.Type,
                    cell.Position - normalizeShift,
                    cell.OpenDirections,
                    cell.ClosedDirections))
                .ToList();
        }
        
        private List<MissionCell> RotatePattern(IEnumerable<MissionCell> pattern, RoomDirection direction)
        {
            var rotation = direction.ToRotation();
            return pattern.Select(cell => 
                new MissionCell(
                    cell.Type,
                    (rotation * cell.Position.ToVector3()).ToVector2Int(),
                    cell.OpenDirections.Select(openDirection => openDirection.Rotate(direction)).ToList(),
                    cell.ClosedDirections.Select(closedDirection => closedDirection.Rotate(direction)).ToList()
                )).ToList();
        }

        private List<MissionCell> FlipYPattern(IEnumerable<MissionCell> pattern)
        {
            return pattern.Select(cell =>
                new MissionCell(
                    cell.Type,
                    new Vector2Int(cell.Position.x, -cell.Position.y),
                    cell.OpenDirections.Select(openDirection => openDirection.FlipY()).ToList(),
                    cell.ClosedDirections.Select(closedDirection => closedDirection.FlipY()).ToList()
                )
            ).ToList();
        }
    }
}