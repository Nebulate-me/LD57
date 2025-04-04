using System.Collections.Generic;
using _Scripts.BehaviorTree.Example.Enemy.AI.Actions;
using BehaviorTree;
using BehaviorTree.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Scripts.BehaviorTree.Example.Enemy.AI
{
    public class ExampleEnemyBehaviorTree : BaseBehaviorTree, IExampleEnemyBehaviorTree
    {
        [Inject] private Transform transform;
        [Inject] private DiContainer diContainer;

        private ExampleEnemyAICalculationState state = new();
        [ShowInInspector] public ExampleEnemyAICalculationState State => state;

        public override Node CreateTree()
        {
            state = GetEmptyState();
            var stateArgs = new object[] {state};

            var tree = new Selector(new List<INode>
            {
                new Sequence(new List<INode>
                {
                    diContainer.Instantiate<CanMoveToEnemyCheck>(stateArgs),
                    new Selector(new List<INode>
                    {
                        diContainer.Instantiate<MoveToEnemyAction>(stateArgs),
                    })
                }),
                new Sequence(new List<INode>()
                {
                    diContainer.Instantiate<CanAttackEnemyCheck>(stateArgs),
                    new Selector(new List<INode>
                    {
                        diContainer.Instantiate<AttackEnemyAction>(stateArgs),
                    })
                }),
                diContainer.Instantiate<IdleAction>(stateArgs),
            });

            return tree;
        }

        private ExampleEnemyAICalculationState GetEmptyState()
        {
            return new ExampleEnemyAICalculationState
            {
                CurrentAction = ExampleEnemyAIAction.None,
                Transform = transform,
            };
        }
    }
}