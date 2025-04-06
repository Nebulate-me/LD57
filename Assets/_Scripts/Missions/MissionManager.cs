using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
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
        [SerializeField] private GameObject missionPrefab;
        [SerializeField] private List<Mission> availableMissions = new();
        [SerializeField] private Mission initialMission;
        [SerializeField] private int missionHandSize = 1;
        [SerializeField] private List<int> missionHandSizeIncreases = new() {2, 5, 10};

        [Inject] private IPrefabPool prefabPool;
        [Inject] private IRandomService randomService;
        [Inject] private IDungeonGridManager dungeonGridManager;
        [Inject] private IDeckManager deckManager;

        private readonly List<MissionCardView> missionCards = new();
        private int completedMissionCount = 0;

        private void OnEnable()
        {
            SignalsHub.AddListener<RoomPlacedSignal>(OnRoomPlaced);
        }
        
        private void OnDisable()
        {
            SignalsHub.RemoveListener<RoomPlacedSignal>(OnRoomPlaced);
        }

        private void OnRoomPlaced(RoomPlacedSignal roomPlacedSignal)
        {
            UpdateMissions();
        }

        private void Start()
        {
            missionContainer.DestroyChildren();
            
            var missionDto = initialMission.ToDto();
            var missionCardView = prefabPool.Spawn(missionPrefab, missionContainer).GetComponent<MissionCardView>();
            missionCardView.SetUp(missionDto);
            missionCardView.Completable = false;
            missionCards.Add(missionCardView);
        }

        public void CompleteMission(MissionCardView missionCard)
        {
            if (!IsMissionCompletable(missionCard.Dto, dungeonGridManager.Rooms, out var roomsToUse)) return;

            foreach (var dungeonRoomView in roomsToUse) 
                dungeonRoomView.IsUsed = true;
            
            var shuffledMissionRewards = randomService.Shuffle(missionCard.Dto.RewardCards).ToList(); 
            deckManager.Bury(shuffledMissionRewards);
            prefabPool.Despawn(missionCard.gameObject);
            missionCards.Remove(missionCard);
            
            completedMissionCount++;
            if (missionHandSizeIncreases.Contains(completedMissionCount)) 
                missionHandSize++;
            // TODO: Increase Player Score here
            UpdateMissions();
        }
        
        private void UpdateMissions()
        {
            foreach (var missionCardView in missionCards)
            {
                missionCardView.Completable = IsMissionCompletable(missionCardView.Dto, dungeonGridManager.Rooms, out var _);
            }

            RefillMissionHand();
        }
        
        private void RefillMissionHand()
        {
            while (missionCards.Count < missionHandSize)
            {
                var missionDto = randomService.Sample(availableMissions).ToDto();
                var missionCardView = prefabPool.Spawn(missionPrefab, missionContainer).GetComponent<MissionCardView>();
                missionCardView.SetUp(missionDto);
                missionCardView.Completable = IsMissionCompletable(missionCardView.Dto, dungeonGridManager.Rooms, out var _);
                missionCards.Add(missionCardView);
            }
        }

        private bool IsMissionCompletable(MissionDto missionDto, IReadOnlyList<DungeonRoomView> rooms, out List<DungeonRoomView> roomsToUse)
        {
            roomsToUse = new List<DungeonRoomView>();
            var unusedRooms = rooms.Where(room => !room.IsUsed).ToList();
            var allDirections = EnumExtensions.GetAllItems<RoomDirection>().ToList();
            var normalizedPattern = NormalizePattern(missionDto.Pattern);
            foreach (var direction in allDirections)
            {
                var rotatedPattern = RotatePattern(normalizedPattern, direction);
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

        private List<MissionCell> NormalizePattern(List<MissionCell> missionDtoPattern)
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
        
        private List<MissionCell> RotatePattern(List<MissionCell> pattern, RoomDirection direction)
        {
            var rotation = direction.ToRotation();
            return pattern.Select(cell => 
                new MissionCell(
                    cell.Type,
                    (rotation * cell.Position.ToVector3()).ToVector2Int(),
                    cell.OpenDirections.Select(openDirection => openDirection.Rotate(direction)).ToList(),
                    cell.ClosedDirections.Select(closedDirection => closedDirection.Rotate(direction)).ToList()
                )
                ).ToList();
        }
    }
}