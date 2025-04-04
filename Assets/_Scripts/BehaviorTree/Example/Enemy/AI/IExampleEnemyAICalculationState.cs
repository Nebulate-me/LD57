using UnityEngine;

namespace _Scripts.BehaviorTree.Example.Enemy.AI
{
    public enum ExampleEnemyAIAction
    {
        None,
        Moving,
        Attacking,
        Idle,
    }

    public interface IExampleEnemyAICalculationState
    {
        public Transform Transform { get; set; }
        public Vector2 NextPosition { get; }
        public ExampleEnemyAIAction CurrentAction { get; }
    }
}