using BehaviorTree;

namespace _Scripts.BehaviorTree.Example.Enemy.AI.Actions
{
    public class IdleAction : Node
    {
        private readonly ExampleEnemyAICalculationState aiState;

        public IdleAction(ExampleEnemyAICalculationState aiState)
        {
            this.aiState = aiState;
        }

        public override NodeState Evaluate()
        {
            aiState.CurrentAction = ExampleEnemyAIAction.Idle;
            return NodeState.Success;
        }
    }
}