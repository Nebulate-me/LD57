using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.Rooms;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Missions.Apartment
{
    public class ApartmentMissionCardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI missionNameText;
        [SerializeField] private TextMeshProUGUI rewardCountText;
        [SerializeField] private TextMeshProUGUI rewardScoreText;

        [Header("Room Type Icon")]
        [SerializeField] private GameObject roomTypeIconViewPrefab;
        [SerializeField] private RectTransform roomTypeIconContainer;

        [Header("Mission Background")]
        [SerializeField] private Image missionBackgroundImage;
        [Space] [SerializeField] private Sprite uncompletableMissionBackgroundSprite;
        [Space] [SerializeField] private Sprite completableMissionBackgroundSprite;

        [Inject] private IMissionManager _missionManager;
        [Inject] private IPrefabPool _prefabPool;
        
        private ApartmentMissionDto _dto;
        private bool _isCompletable;
        private List<DungeonRoomModel> _roomsToUse = new();

        public ApartmentMissionDto Dto => _dto;
        
        public bool Completable
        {
            get => _isCompletable;
            set
            {
                _isCompletable = value;
                missionBackgroundImage.sprite =
                    value ? completableMissionBackgroundSprite : uncompletableMissionBackgroundSprite;
            }
        }

        public void SetUp(ApartmentMissionDto missionDto)
        {
            _dto = missionDto;
            missionNameText.text = missionDto.Name;
            rewardCountText.text = $"{missionDto.RewardRooms.Count} карт";
            rewardScoreText.text = $"{missionDto.RewardScore} очков";

            SetUpRequirements(missionDto);
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
        }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            _isCompletable = false;
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
            _missionManager.HighlightMission(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _missionManager.UnhighlightMission(this);
        }
    }
}