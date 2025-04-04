using System.Collections.Generic;
using BehaviorTree.Common;
using UnityEngine;

namespace _Scripts.BehaviorTree.Example.Enemy.AI
{
    /// <summary>
    /// Sample class for enemy state calculation 
    /// </summary>
    public class ExampleExampleEnemyAICalculationState : AICalculationState<ExampleEnemyAICalculationResultType>, IExampleEnemyAICalculationState
    {
        public Transform Transform { get; set; }
        public Vector2 NextPosition { get; }

        public ExampleEnemyAIAction CurrentAction { get; }

        public List<Vector2> MoveSequence { get; } = new();
        public List<Vector2> AttackPoints { get; } = new();

        public ExampleExampleEnemyAICalculationState(ExampleEnemyAICalculationResultType resultType) : base(resultType)
        {
        }

        public override void Clear()
        {
            base.Clear();
            MoveSequence.Clear();
            AttackPoints.Clear();
        }
    }
}