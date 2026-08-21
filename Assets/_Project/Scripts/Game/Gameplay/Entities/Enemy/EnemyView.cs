using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public Transform LookTarget { get; private set; }
        [field: SerializeField] public Damageable Damageable { get; private set; }
    }
}
