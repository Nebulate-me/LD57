using System;
using System.Collections.Generic;
using _Scripts.Utils;
using UnityEngine;
using Utilities;
using Utilities.Prefabs;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Missions
{
    public class MissionManager : MonoBehaviour
    {
        [SerializeField] private RectTransform missionContainer;
        [SerializeField] private GameObject missionPrefab;
        [SerializeField] private List<Mission> availableMissions = new();
        [SerializeField] private int missionHandSize = 1;

        [Inject] private IPrefabPool prefabPool;
        [Inject] private IRandomService randomService;

        private List<MissionCardView> missionCards = new();

        private void Start()
        {
            missionContainer.DestroyChildren();

            for (var i = 0; i < missionHandSize; i++)
            {
                var mission = randomService.Sample(availableMissions);
                var missionCardView = prefabPool.Spawn(missionPrefab, missionContainer).GetComponent<MissionCardView>();
                missionCardView.SetUp(mission);
                missionCards.Add(missionCardView);
            }
        }
    }
}