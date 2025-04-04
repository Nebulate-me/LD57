using BehaviorTree.Common;

namespace _Scripts.BehaviorTree.Example.Enemy.AI
{
    /// <summary>
    /// Sample class to define pseudo-enum for different forms of AI behavior
    /// </summary>
    public class ExampleEnemyAICalculationResultType : IAICalculationResultType
    {
        public static readonly string None = "None";
        public static readonly string Move = "Move";
        public static readonly string Attack = "Attack";

        public string Default => None;

        public bool IsValid(string enemyAiResultType)
        {
            return enemyAiResultType == None || enemyAiResultType == Move || enemyAiResultType == Attack;
        }
    }
}