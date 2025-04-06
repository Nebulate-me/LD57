using System.Collections.Generic;
using _Scripts.Cards;
using _Scripts.Missions;
using _Scripts.Rooms;
using DITools;
using UnityEngine;
using Utilities.Prefabs;
using Utilities.Random;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.DI
{
    public class DiInstaller : MonoInstaller
    {
        [SerializeField] private PrefabPool prefabPool;
        
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private HandManager handManager;
        [SerializeField] private DungeonGridManager dungeonGridManager;
        [SerializeField] private MissionManager missionManager;
        [SerializeField] private ScoreManager scoreManager;

        [Header("Do Not Edit")]
        [SerializeField] private Camera uiCamera;

        protected virtual void ConfigureServices()
        {
            Container.Configure(new List<ConfigureType>
            {
                new(typeof(IContainerConstructable), ScopeTypes.Singleton, false),
            });
        }

        public override void InstallBindings()
        {
            ConfigureServices();

            Container.Bind<IPrefabPool>().FromInstance(prefabPool).AsSingle().NonLazy();
            Container.Bind<IRandomService>().To<RandomService>().AsSingle().NonLazy();

            uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Container.Bind<Camera>().WithId("uiCamera").FromInstance(uiCamera).AsSingle();

            Container.BindInterfacesTo<HandManager>().FromInstance(handManager).AsSingle();
            Container.BindInterfacesTo<DeckManager>().FromInstance(deckManager).AsSingle();
            Container.BindInterfacesTo<DungeonGridManager>().FromInstance(dungeonGridManager).AsSingle();
            Container.BindInterfacesTo<MissionManager>().FromInstance(missionManager).AsSingle();
            Container.BindInterfacesTo<ScoreManager>().FromInstance(scoreManager).AsSingle();
        }

        private void OnDisable()
        {
            Destroy(prefabPool);
        }
    }
}