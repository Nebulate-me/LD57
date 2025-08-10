using System.Collections.Generic;
using _Scripts.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Missions.Apartment
{
    public class ApartmentMissionCardView : MonoBehaviour, IPointerClickHandler
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

            foreach (var requirement in missionDto.Requirements)
            {
                var roomTypeIconView = _prefabPool.Spawn(roomTypeIconViewPrefab, roomTypeIconContainer)
                    .GetComponent<RoomTypeIconView>();
                roomTypeIconView.SetUp(requirement);
                // TODO: Update based on requirement fulfilment
            }

            if (missionDto.RequiredWindows > 0)
            {
                var roomTypeIconView = _prefabPool.Spawn(roomTypeIconViewPrefab, roomTypeIconContainer)
                    .GetComponent<RoomTypeIconView>();
                roomTypeIconView.SetUp(missionDto.RequiredWindows);
            }

            foreach (var objectToDestroy in objectsToDestroy)
            {
                Destroy(objectToDestroy);
            }
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
    }
}