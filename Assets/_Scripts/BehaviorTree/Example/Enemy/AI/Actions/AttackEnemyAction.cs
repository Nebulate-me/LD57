using BehaviorTree;

namespace _Scripts.BehaviorTree.Example.Enemy.AI.Actions
{
    public class AttackEnemyAction : Node
    {
        private readonly ExampleEnemyAICalculationState aiState;

        public AttackEnemyAction(ExampleEnemyAICalculationState aiState)
        {
            this.aiState = aiState;
        }

        public override NodeState Evaluate()
        {
            if (aiState.CurrentAction == ExampleEnemyAIAction.Attacking)
                return NodeState.Success;

            return base.Evaluate();
        }
    }
}