using System.Collections.Generic;
using System.Linq;
using _Scripts.Utils;
using ModestTree;
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
        [SerializeField] private TextMeshProUGUI missionRequirementsText;
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
            missionRequirementsText.text = GetRequirementsText(missionDto.Requirements);
        }

        private string GetRequirementsText(IEnumerable<RoomRequirement> requirements)
        {
            return requirements.Select(requirement => $"- {requirement.RoomType.Translate()}").Join("\n");
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