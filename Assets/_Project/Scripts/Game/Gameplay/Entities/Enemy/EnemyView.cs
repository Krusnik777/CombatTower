using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public Transform LookTarget { get; private set; }
    }
}
