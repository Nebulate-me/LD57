using UnityEngine;

namespace _Scripts.BehaviorTree.Example.Enemy.AI
{
    public class ExampleEnemyAICalculationState : IExampleEnemyAICalculationState
    {
        public Transform Transform { get; set; }
        public Vector2 NextPosition { get; set; }
        public ExampleEnemyAIAction CurrentAction { get; set; } = ExampleEnemyAIAction.None;
    }
}