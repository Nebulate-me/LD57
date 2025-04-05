using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Missions
{
    public class MissionCardView : MonoBehaviour, IPoolableResource
    {
        [SerializeField] private TextMeshProUGUI missionName;
        [SerializeField] private Image missionBackgroundImage;
        [SerializeField] private RectTransform missionPatternContainer;
        [SerializeField] private GameObject missionPatternCellPrefab;
        [SerializeField] private float patternCellSize = 0.33f; 

        [Space] [SerializeField] private Sprite uncompletableMissionBackgroundSprite;
        [Space] [SerializeField] private Sprite completableMissionBackgroundSprite;

        [Inject] private IPrefabPool prefabPool;
        
        private Mission mission;
        private readonly List<MissionPatternCellView> patternCellViews = new();

        public Mission Mission => mission;
        public void SetUp(Mission newMission, bool completable = false)
        {
            mission = newMission;
            missionName.text = newMission.MissionName;
            Completable = completable;

            missionPatternContainer.DestroyChildren();
            foreach (var patternCell in newMission.Pattern)
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
            set => missionBackgroundImage.sprite =
                    value ? completableMissionBackgroundSprite : uncompletableMissionBackgroundSprite;
        }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            foreach (var cellView in patternCellViews)
            {
                prefabPool.Despawn(cellView.gameObject);
            }
            patternCellViews.Clear();
        }
    }
}