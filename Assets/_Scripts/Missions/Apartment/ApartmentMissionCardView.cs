using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
using _Scripts.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Missions.Apartment
{
    public class ApartmentMissionCardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPoolableResource
    {
        [SerializeField] private TextMeshProUGUI missionNameText;
        [SerializeField] private TextMeshProUGUI rewardScoreText;

        [Header("Room Type Icon")]
        [SerializeField] private GameObject roomTypeIconViewPrefab;
        [SerializeField] private RectTransform roomTypeIconContainer;

        [Header("Mission Background")]
        [SerializeField] private Image missionBackgroundImage;
        [Space] [SerializeField] private Sprite uncompletableMissionBackgroundSprite;
        [SerializeField] private Sprite hoveredMissionBackgroundSprite;
        [SerializeField] private Sprite completableMissionBackgroundSprite;

        [Inject] private IMissionManager _missionManager;
        [Inject] private IPrefabPool _prefabPool;
        
        private ApartmentMissionDto _dto;
        private List<DungeonRoomModel> _roomsToUse = new();
        
        [ShowInInspector, ReadOnly] private bool _isCompletable;
        [ShowInInspector, ReadOnly] private bool _isHovered;
        
        public ApartmentMissionDto Dto => _dto;
        
        public bool Completable
        {
            get => _isCompletable;
            set
            {
                _isCompletable = value;
                UpdateCardBackground();
            }
        }

        private void UpdateCardBackground()
        {
            missionBackgroundImage.sprite = _isCompletable 
                ? completableMissionBackgroundSprite 
                : _isHovered 
                    ? hoveredMissionBackgroundSprite 
                    : uncompletableMissionBackgroundSprite; 
        }

        public void SetUp(ApartmentMissionDto missionDto)
        {
            _dto = missionDto;
            missionNameText.text = missionDto.Name;
            
            SeUpRewardScore(missionDto);
            SetUpRequirements(missionDto);
        }

        private void SeUpRewardScore(ApartmentMissionDto missionDto)
        {
            var currentRewardScore = _roomsToUse.Any() ? (missionDto.RewardScore + _roomsToUse.Sum(room => room.Score)) : missionDto.RewardScore;
            rewardScoreText.text = $"{currentRewardScore} {currentRewardScore.DeclinePoints()}";
        }

        private void SetUpRequirements(ApartmentMissionDto missionDto)
        {
            var objectsToDestroy = new List<GameObject>();
            foreach (Transform childTransform in roomTypeIconContainer)
            {
                var objectToDestroy = childTransform.gameObject;
                objectToDestroy.SetActive(false);
                objectsToDestroy.Add(objectToDestroy);
            }

            var usedRooms = _roomsToUse.Select(room => room).ToList();
            
            foreach (var requirement in missionDto.Requirements)
            {
                var isFulfilled = usedRooms.TryRemoveFirst(room => room.IsFulfilling(requirement), out var _);
                var roomTypeIconView = _prefabPool.Spawn(roomTypeIconViewPrefab, roomTypeIconContainer)
                    .GetComponent<RoomTypeIconView>();
                roomTypeIconView.SetUp(requirement, isFulfilled);
            }

            if (missionDto.RequiredWindows > 0)
            {
                var roomTypeIconView = _prefabPool.Spawn(roomTypeIconViewPrefab, roomTypeIconContainer)
                    .GetComponent<RoomTypeIconView>();
                var usedWindows = _roomsToUse.Sum(room => room.WindowCount);
                roomTypeIconView.SetUp(missionDto.RequiredWindows, usedWindows >= missionDto.RequiredWindows);
            }

            foreach (var objectToDestroy in objectsToDestroy)
            {
                Destroy(objectToDestroy);
            }
        }
        
        public void SetAchievedRequirements(List<DungeonRoomModel> roomsToUse)
        {
            _roomsToUse = roomsToUse;
            SetUpRequirements(_dto);
            SeUpRewardScore(_dto);
        }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            _isCompletable = false;
            _isHovered = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isCompletable)
            {
                _missionManager.CompleteMission(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateCardBackground();
            _missionManager.HighlightMission(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateCardBackground();
            _missionManager.UnhighlightMission(this);
        }
    }
}