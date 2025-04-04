using BehaviorTree;
using UnityEngine;

namespace _Scripts.BehaviorTree.Example.Enemy.AI.Actions
{
    public class CanMoveToEnemyCheck : Node
    {
        private readonly ExampleEnemyAICalculationState aiState;

        public CanMoveToEnemyCheck(ExampleEnemyAICalculationState aiState)
        {
            this.aiState = aiState;
        }

        public override NodeState Evaluate()
        {
            var enemy = GameObject.Find("Enemy");

            if (enemy == null || !enemy.activeSelf)
                return NodeState.Failure;

            var testEnemyPosition = enemy.transform.position;

            // if enemy far enough
            var distanceToEnemy = Vector3.Distance(aiState.Transform.position, testEnemyPosition);
            if (distanceToEnemy > 0.5f && distanceToEnemy < 5f)
            {
                aiState.CurrentAction = ExampleEnemyAIAction.Moving;
                aiState.NextPosition = testEnemyPosition;
                return NodeState.Success;
            }

            return NodeState.Failure;
        }
    }
}