using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Missions
{
    public class MissionCardView : MonoBehaviour, IPoolableResource, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI missionName;
        [SerializeField] private TextMeshProUGUI rewardCountText;
        [SerializeField] private TextMeshProUGUI rewardScoreText;
        [SerializeField] private Image missionBackgroundImage;
        [SerializeField] private RectTransform missionPatternContainer;
        [SerializeField] private GameObject missionPatternCellPrefab;
        [SerializeField] private float patternCellSize = 0.33f; 

        [Space] [SerializeField] private Sprite uncompletableMissionBackgroundSprite;
        [Space] [SerializeField] private Sprite completableMissionBackgroundSprite;

        [Inject] private IPrefabPool prefabPool;
        [Inject] private IMissionManager missionManager;
        
        private MissionDto dto;
        private readonly List<MissionPatternCellView> patternCellViews = new();
        private bool isCompletable;

        public MissionDto Dto => dto;
        public void SetUp(MissionDto missionDto)
        {
            dto = missionDto;
            missionName.text = missionDto.Name;
            rewardCountText.text = missionDto.RewardCards.Count.ToString();
            rewardScoreText.text = missionDto.RewardScore.ToString();

            missionPatternContainer.DestroyChildren();
            foreach (var patternCell in missionDto.Pattern)
            {
                if (patternCell.Type == MissionCellType.Any) continue;
                var patternCellView = prefabPool.Spawn(missionPatternCellPrefab, missionPatternContainer).GetComponent<MissionPatternCellView>();
                patternCellView.SetUp(patternCell);
                patternCellView.transform.position = missionPatternContainer.transform.position + patternCell.Position.ToVector3() * patternCellSize;
                patternCellViews.Add(patternCellView);
            }
        }

        public bool Completable
        {
            set
            {
                isCompletable = value;
                missionBackgroundImage.sprite =
                    value ? completableMissionBackgroundSprite : uncompletableMissionBackgroundSprite;
            }
        }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            isCompletable = false;
            foreach (var cellView in patternCellViews)
            {
                prefabPool.Despawn(cellView.gameObject);
            }
            patternCellViews.Clear();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isCompletable)
            {
                missionManager.CompleteMission(this);
            }
        }
    }
}