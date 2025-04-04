using BehaviorTree;
using UnityEngine;

namespace _Scripts.BehaviorTree.Example.Enemy.AI.Actions
{
    public class CanAttackEnemyCheck : Node
    {
        private readonly ExampleEnemyAICalculationState aiState;

        public CanAttackEnemyCheck(ExampleEnemyAICalculationState aiState)
        {
            this.aiState = aiState;
        }

        public override NodeState Evaluate()
        {
            var enemy = GameObject.Find("Enemy");

            if (enemy == null || !enemy.activeSelf)
                return NodeState.Failure;

            var testEnemyPosition = enemy.transform.position;

            // Check whether enemy close enough and other conditions
            if (Vector3.Distance(aiState.Transform.position, testEnemyPosition) <= 0.5f)
            {
                aiState.CurrentAction = ExampleEnemyAIAction.Attacking;
                return NodeState.Success;
            }

            return NodeState.Failure;
        }
    }
}