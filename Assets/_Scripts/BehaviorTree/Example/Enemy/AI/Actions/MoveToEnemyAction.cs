using BehaviorTree;

namespace _Scripts.BehaviorTree.Example.Enemy.AI.Actions
{
    public class MoveToEnemyAction : Node
    {
        private readonly ExampleEnemyAICalculationState aiState;

        public MoveToEnemyAction(ExampleEnemyAICalculationState aiState)
        {
            this.aiState = aiState;
        }

        public override NodeState Evaluate()
        {
            if (aiState.CurrentAction == ExampleEnemyAIAction.Moving)
                return NodeState.Success;

            return base.Evaluate();
        }
    }
}