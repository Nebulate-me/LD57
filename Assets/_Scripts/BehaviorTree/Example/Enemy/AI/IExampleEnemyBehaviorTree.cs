using BehaviorTree.Common;

namespace _Scripts.BehaviorTree.Example.Enemy.AI
{
    public interface IExampleEnemyBehaviorTree : IBehaviorTree
    {
        public ExampleEnemyAICalculationState State { get; }
    }
}