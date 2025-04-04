using _Scripts.BehaviorTree.Example.Enemy.AI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Scripts.BehaviorTree.Example.Enemy
{
    public class ExampleEnemyAIController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Inject, ShowInInspector, ReadOnly] private ExampleEnemyBehaviorTree behaviorTree;

        private void Update()
        {
            behaviorTree.Evaluate();
            var state = behaviorTree.State;

            Debug.LogWarning($"state.CurrentAction: {state.CurrentAction}");

            if (state.CurrentAction == ExampleEnemyAIAction.Moving)
            {
                transform.position = Vector3.Lerp(transform.position, state.NextPosition, Time.deltaTime * 1f);
            }

            if (state.CurrentAction == ExampleEnemyAIAction.Attacking)
            {
                spriteRenderer.color = Color.red;
            }
            else if (state.CurrentAction == ExampleEnemyAIAction.Idle)
            {
                spriteRenderer.color = Color.green;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}