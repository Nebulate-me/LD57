using System.Collections.Generic;
using _Scripts.Game;
using _Scripts.Player;
using DITools;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Prefabs;
using Utilities.Random;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.DI
{
    public class BasicScreenDiInstaller : MonoInstaller
    {
        [SerializeField] private PrefabPool prefabPool;
        [SerializeField] private SoundManager soundManager;
        
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
        }

        private void OnDisable()
        {
            Destroy(prefabPool);
        }
    }
}