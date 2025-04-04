using _Scripts.BehaviorTree.Example.Enemy.AI;
using UnityEngine;
using Zenject;

namespace _Scripts.BehaviorTree.Example.Enemy
{
    public class ExampleEnemyDiInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Transform>().FromInstance(transform).AsSingle();

            var aiBehaviourTree = Container.Instantiate<ExampleEnemyBehaviorTree>();
            Container.BindInterfacesAndSelfTo<ExampleEnemyBehaviorTree>().FromInstance(aiBehaviourTree).AsSingle();
            aiBehaviourTree.InitTree();
        }
    }
}