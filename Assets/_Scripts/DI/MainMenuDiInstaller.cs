using System.Collections.Generic;
using _Scripts.Cards;
using _Scripts.Game;
using _Scripts.Game.Timer;
using _Scripts.Missions;
using _Scripts.Player;
using _Scripts.Rooms;
using _Scripts.Score;
using DITools;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;
using Utilities.Random;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.DI
{
    public class MainMenuDiInstaller : MonoInstaller
    {
        [SerializeField] private PrefabPool prefabPool;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private ScoreSaver scoreSaver;
        [SerializeField] private GameManager gameManager;
        
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

            _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Container.Bind<Camera>().WithId("uiCamera").FromInstance(_uiCamera).AsSingle();
            
            Container.BindInterfacesTo<SoundManager>().FromInstance(soundManager).AsSingle();
            
            Container.BindInterfacesTo<ScoreManager>().FromInstance(scoreManager).AsSingle();
            Container.BindInterfacesTo<ScoreSaver>().FromInstance(scoreSaver).AsSingle();
            Container.BindInterfacesTo<GameManager>().FromInstance(gameManager).AsSingle();
        }

        private void OnDisable()
        {
            Destroy(prefabPool);
        }
    }
}