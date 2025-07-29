using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Missions.Apartment
{
    public class ApartmentMissionCardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI missionNameText;
        [SerializeField] private TextMeshProUGUI rewardCountText;
        [SerializeField] private TextMeshProUGUI rewardScoreText;
        
        [SerializeField] private Image missionBackgroundImage;
        [Space] [SerializeField] private Sprite uncompletableMissionBackgroundSprite;
        [Space] [SerializeField] private Sprite completableMissionBackgroundSprite;

        [Inject] private IMissionManager _missionManager;
        
        private ApartmentMissionDto _dto;
        private bool _isCompletable;

        public ApartmentMissionDto Dto => _dto;

        public void SetUp(ApartmentMissionDto missionDto)
        {
            _dto = missionDto;
            missionNameText.text = missionDto.Name;
            rewardCountText.text = missionDto.RewardRooms.Count.ToString();
            rewardScoreText.text = missionDto.RewardScore.ToString();
        }

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