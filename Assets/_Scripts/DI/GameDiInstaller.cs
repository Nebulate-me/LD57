using System.Collections.Generic;
using _Scripts.Achievements;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Game.Audio;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Player;
using _Scripts.Rooms;
using _Scripts.Score;
using _Scripts.Screens;
using DITools;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;
using Utilities.Random;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.DI
{
    public class GameDiInstaller : MonoInstaller
    {
        [SerializeField] private PrefabPool prefabPool;
        
        [SerializeField] private DungeonCameraController cameraController;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private HandManager handManager;
        [SerializeField] private DungeonGridManager dungeonGridManager;
        [SerializeField] private MissionManager missionManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private AchievementManager achievementManager;
        [SerializeField] private GameTimerController gameTimer;
        [SerializeField] private GameManager gameManager;

        [Header("Prefabs")]
        [SerializeField] private GameObject roomRegistryPrefab;
        
        
        [ShowInInspector, ReadOnly] private Camera _uiCamera;

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
            Container.Bind<IPlayerProfileService>().To<PlayerProfileService>().AsSingle().NonLazy();
            Container.Bind<IScreenManager>().To<ScreenManager>().AsSingle();

            _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Container.Bind<Camera>().WithId("uiCamera").FromInstance(_uiCamera).AsSingle();

            Container.BindInterfacesTo<DungeonCameraController>().FromInstance(cameraController).AsSingle();
            Container.BindInterfacesTo<SoundManager>().FromInstance(soundManager).AsSingle();
            
            Container.BindInterfacesTo<HandManager>().FromInstance(handManager).AsSingle();
            Container.BindInterfacesTo<DeckManager>().FromInstance(deckManager).AsSingle();
            Container.BindInterfacesTo<DungeonGridManager>().FromInstance(dungeonGridManager).AsSingle();
            Container.BindInterfacesTo<MissionManager>().FromInstance(missionManager).AsSingle();
            Container.BindInterfacesTo<ScoreManager>().FromInstance(scoreManager).AsSingle();
            Container.BindInterfacesTo<AchievementManager>().FromInstance(achievementManager).AsSingle();
            Container.BindInterfacesTo<GameTimerController>().FromInstance(gameTimer).AsSingle();
            Container.BindInterfacesTo<GameManager>().FromInstance(gameManager).AsSingle();

            var roomRegistry = Container.InstantiatePrefab(roomRegistryPrefab, transform).GetComponent<RoomRegistry>();
            Container.BindInterfacesTo<RoomRegistry>().FromInstance(roomRegistry).AsSingle();
        }

        private void OnDisable()
        {
            Destroy(prefabPool);
        }
    }
}