using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Missions.Apartment;
using _Scripts.Missions.Pattern;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using ModestTree;
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
        [SerializeField] private int missionHandSize = 1;
        [SerializeField] private List<int> missionHandSizeIncreases = new() {2, 5, 10};

        [Inject] private IPrefabPool _prefabPool;
        [Inject] private IRandomService _randomService;
        [Inject] private IDungeonGridManager _dungeonGridManager;
        [Inject] private IDeckManager _deckManager;
        [Inject] private ISoundManager _soundManager;
        [Inject] private IRoomRegistry _roomRegistry;

        private readonly List<PatternMissionCardView> _patternMissionCardViews = new();
        private readonly List<ApartmentMissionCardView> _apartmentMissionCardViews = new();
        private int _completedMissionCount;
        private string _lastCompletedMissionName;
        private List<DungeonRoomModel> _highlightedRooms = new();
        private Level _currentLevel;

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.AddListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
            SignalsHub.RemoveListener<LevelSetupCompletedSignal>(OnLevelSetupCompleted);
        }

        private void OnRoomPlaced(RoomPlacedSignal signal)
        {
            UpdateMissions();
        }
        
        private void OnLevelSetupCompleted(LevelSetupCompletedSignal signal)
        {
            _currentLevel = signal.Level;
            missionContainer.DestroyChildren();
            foreach (var apartmentMission in _currentLevel.InitialMissions)
            {
                var missionDto = apartmentMission.ToDto();
                var missionCardView = _prefabPool.Spawn(apartmentMissionCardPrefab, missionContainer)
                    .GetComponent<ApartmentMissionCardView>();
                missionCardView.SetUp(missionDto);
                missionCardView.Completable = false;
                _apartmentMissionCardViews.Add(missionCardView);
            }
        }

        private void Start()
        {
            missionContainer.DestroyChildren();
        }

        public int CompletableMissionsCount => _apartmentMissionCardViews.Count(mission => mission.Completable);

        [Obsolete]
        public void CompleteMission(PatternMissionCardView patternMissionCard)
        {
            if (!IsPatternMissionCompletable(patternMissionCard.Dto, _dungeonGridManager.RoomTiles,
                    out var roomsToUse)) return;

            foreach (var dungeonRoomView in roomsToUse)
                dungeonRoomView.IsUsed = true;

            var shuffledMissionRewards = _randomService.Shuffle(patternMissionCard.Dto.RewardCards).ToList();
            // _deckManager.BuryRoomTile(shuffledMissionRewards);
            // TODO: Give Rooms instead
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
            if (!IsApartmentMissionCompletable(apartmentMissionCard.Dto, out var roomsToUse)) return;

            var apartmentFloorColor = _roomRegistry.TakeUnusedColor();
            foreach (var dungeonRoomModel in roomsToUse)
            {
                dungeonRoomModel.IsUsed = true;
                dungeonRoomModel.SetFloorColor(apartmentFloorColor);
            }
            
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

            StartCoroutine(UpdateMissionCoroutine());
        }

        public void HighlightMission(ApartmentMissionCardView apartmentMissionCard)
        {
            var isCompletable = IsApartmentMissionCompletable(apartmentMissionCard.Dto, out var roomsToUse);
            
            var apartmentFloorColor = _roomRegistry.HighlightColor;
            foreach (var dungeonRoomModel in roomsToUse)
            {
                dungeonRoomModel.SetFloorColor(apartmentFloorColor);
            }

            _highlightedRooms = roomsToUse;
        }

        public void UnhighlightMission(ApartmentMissionCardView apartmentMissionCard)
        {
            var apartmentFloorColor = _roomRegistry.UnusedRoomColor;
            
            foreach (var dungeonRoomModel in _highlightedRooms)
            {
                if (!dungeonRoomModel.IsUsed)
                    dungeonRoomModel.SetFloorColor(apartmentFloorColor);
            }
        }

        private IEnumerator UpdateMissionCoroutine()
        {
            yield return new WaitForEndOfFrame();
            
            UpdateMissions();
        }

        private void UpdateMissions()
        {
            foreach (var missionCardView in _apartmentMissionCardViews)
            {
                var isCompletable = IsApartmentMissionCompletable(missionCardView.Dto, out var usedRooms);
                missionCardView.Completable = isCompletable;
                missionCardView.SetAchievedRequirements(usedRooms);
            }

            RefillMissionHand();
            SignalsHub.DispatchAsync(new MissionsUpdatedSignal());
        }

        private void RefillMissionHand()
        {
            if (!_currentLevel) return;
            
            var unlockedMissions = _currentLevel.AvailableMissions
                .Where(mission => _completedMissionCount >= mission.MinCompletedMissions &&
                                  (mission.MaxCompletedMissions <= 0 ||
                                   _completedMissionCount < mission.MaxCompletedMissions) &&
                                  mission.MissionName != _lastCompletedMissionName)
                .ToList();
            while (_apartmentMissionCardViews.Count < missionHandSize)
            {
                var missionDto = _randomService.Sample(unlockedMissions).ToDto();
                if (_apartmentMissionCardViews.Any(card => card.Dto.Name == missionDto.Name)) continue;
                var missionCardView = _prefabPool.Spawn(apartmentMissionCardPrefab, missionContainer)
                    .GetComponent<ApartmentMissionCardView>();
                missionCardView.SetUp(missionDto);
                missionCardView.Completable = IsApartmentMissionCompletable(missionCardView.Dto, out var usedRooms);
                missionCardView.SetAchievedRequirements(usedRooms);
                _apartmentMissionCardViews.Add(missionCardView);
            }
        }

        private bool IsApartmentMissionCompletable(ApartmentMissionDto missionDto, out List<DungeonRoomModel> roomsToUse)
        {
            roomsToUse = new List<DungeonRoomModel>();

            var sharedRooms = _dungeonGridManager.Rooms.Where(room => room.HasType(RoomType.Shared));
            // We can start forming apartment from any of the required rooms or a hallway
            var startingRooms = sharedRooms.SelectMany(sharedRoom =>
                    sharedRoom.AdjacentRooms.Where(room =>
                        !room.IsUsed &&
                        (missionDto.Requirements.Any(room.IsFulfilling) && room.HasType(RoomType.LivingRoom) || room.HasType(RoomType.Hallway))))
                .Distinct();
            foreach (var startingRoom in startingRooms)
            {
                var fulfilledRequirementIndex = missionDto.Requirements.FindIndex(startingRoom.IsFulfilling);
                var startingRequirements = missionDto.Requirements.Where((t, i) => i != fulfilledRequirementIndex).ToList();
                var inputSearch = new ApartmentMissionSearchDto(startingRequirements, new List<DungeonRoomModel>{ startingRoom }, missionDto.RequiredWindows - startingRoom.WindowCount);
                if (TrySearchApartmentMission(inputSearch, out var alternativeRoomsToUse))
                {
                    roomsToUse = alternativeRoomsToUse;
                    return true;
                }

                if (alternativeRoomsToUse.Count > roomsToUse.Count) // TODO: Check the requirements in a better available way
                {
                    roomsToUse = alternativeRoomsToUse;
                }
            }

            return false;
        }

        private bool TrySearchApartmentMission(ApartmentMissionSearchDto inputSearch, out List<DungeonRoomModel> roomsToUse)
        {
            if (inputSearch.IsCompleted)
            {
                roomsToUse = inputSearch.UsedRooms;
                return true;
            }
            
            roomsToUse = inputSearch.UsedRooms.Select(room => room).ToList();
            
            if (GetApartmentRoomOptions(inputSearch, out var outputSearchOptions))
            {
                foreach (var searchOption in outputSearchOptions)
                {
                    if (TrySearchApartmentMission(searchOption, out var alternativeRoomsToUse))
                    {
                        roomsToUse = alternativeRoomsToUse;
                        return true;
                    }

                    if (alternativeRoomsToUse.Count > roomsToUse.Count)
                    {
                        roomsToUse = alternativeRoomsToUse;
                    }
                }
            }
            
            return false;
        }

        private bool GetApartmentRoomOptions(ApartmentMissionSearchDto inputSearch,
            out List<ApartmentMissionSearchDto> outputSearchOptions)
        {
            outputSearchOptions = inputSearch.AdjacentRoomOptions.Select(roomOption =>
            {
                var fulfilledRequirementIndex = inputSearch.Requirements.FindIndex(roomOption.IsFulfilling);
                var requirements = inputSearch.Requirements.Where((t, i) => i != fulfilledRequirementIndex).ToList();
                var usedRooms = new List<DungeonRoomModel> { roomOption };
                usedRooms.AddRange(inputSearch.UsedRooms);
                
                return new ApartmentMissionSearchDto(requirements, usedRooms, inputSearch.RemainingWindowCount - roomOption.WindowCount);
            }).ToList();
            return !outputSearchOptions.IsEmpty();
        }

        private bool IsPatternMissionCompletable(PatternMissionDto patternMissionDto,
            IReadOnlyList<DungeonRoomTileView> rooms, out List<DungeonRoomTileView> roomsToUse)
        {
            roomsToUse = new List<DungeonRoomTileView>();
            var unusedRooms = rooms.Where(room => !room.IsUsed).ToList();
            var allDirections = EnumExtensions.GetAllItems<RoomDirection>().ToList();
            var normalizedPattern = NormalizePattern(patternMissionDto.Pattern);
            if (IsAnyPatternDirectionMatching(ref roomsToUse, allDirections, normalizedPattern, unusedRooms))
                return true;

            if (patternMissionDto.FlipPatternY)
            {
                var normalizedFlippedYPattern = NormalizePattern(FlipYPattern(patternMissionDto.Pattern));
                if (IsAnyPatternDirectionMatching(ref roomsToUse, allDirections, normalizedFlippedYPattern,
                        unusedRooms))
                    return true;
            }

            return false;
        }

        private bool IsAnyPatternDirectionMatching(ref List<DungeonRoomTileView> roomsToUse,
            List<RoomDirection> allDirections, IReadOnlyCollection<MissionCell> normalizedFlippedYPattern,
            List<DungeonRoomTileView> unusedRooms)
        {
            foreach (var direction in allDirections)
            {
                var rotatedPattern = RotatePattern(normalizedFlippedYPattern, direction);
                foreach (var dungeonRoomView in unusedRooms)
                    if (IsPatternMatching(rotatedPattern, dungeonRoomView, unusedRooms, out roomsToUse))
                        return true;
            }

            return false;
        }

        private bool IsPatternMatching(List<MissionCell> rotatedPattern, DungeonRoomTileView startingRoom,
            List<DungeonRoomTileView> rooms, out List<DungeonRoomTileView> roomsToUse)
        {
            roomsToUse = new List<DungeonRoomTileView>();
            var startingPosition = startingRoom.GridPosition;
            foreach (var missionCell in rotatedPattern)
            {
                var maybeMatchingRoom =
                    rooms.FirstOrEmpty(room => room.GridPosition == startingPosition + missionCell.Position);
                var matchingRoomExists = maybeMatchingRoom.TryGetValue(out var matchingRoom);
                switch (missionCell.Type)
                {
                    case MissionCellType.Any:
                        break;
                    case MissionCellType.Room:
                        if (!matchingRoomExists) return false;
                        if (!missionCell.OpenDirections.All(openDirection =>
                                matchingRoom.OpenDirections.Contains(openDirection))) return false;
                        if (missionCell.ClosedDirections.Any(closedDirection =>
                                matchingRoom.OpenDirections.Contains(closedDirection))) return false;
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

    internal class ApartmentMissionSearchDto
    {
        public readonly List<RoomRequirement> Requirements;
        public readonly int RequiredWindowCount;
        
        public readonly List<DungeonRoomModel> UsedRooms;
        public readonly int RemainingWindowCount;

        public ApartmentMissionSearchDto(List<RoomRequirement> requirements, List<DungeonRoomModel> usedRooms,
            int remainingWindowCount)
        {
            Requirements = requirements;
            UsedRooms = usedRooms;
            RemainingWindowCount = remainingWindowCount;
        }

        /// <summary>
        /// The idea is to get all adjacent hallways that may connect to the required rooms
        /// </summary>
        public List<DungeonRoomModel> AdjacentRoomOptions => UsedRooms.SelectMany(usedRoom =>
                usedRoom.AdjacentRooms.Where(
                    room => !room.IsUsed &&
                            !UsedRooms.Contains(room) &&
                            (Requirements.Any(room.IsFulfilling) || room.HasType(RoomType.Hallway)) ))
            .ToList();

        public bool IsCompleted => Requirements.IsEmpty() && RemainingWindowCount <= 0;
    }
}