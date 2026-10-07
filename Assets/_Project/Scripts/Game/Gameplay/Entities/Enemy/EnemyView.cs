using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public AnimatorEventsCollector EventsCollector { get; private set; }
        [field: SerializeField] public Transform LookTarget { get; private set; }
        [field: SerializeField] public Damageable Damageable { get; private set; }
        [field: SerializeField] public FieldOfViewParameters FieldOfViewParameters { get; private set; }
    }
}
