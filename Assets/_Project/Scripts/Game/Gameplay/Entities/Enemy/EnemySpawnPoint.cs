using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [field: SerializeField] public EnemyType SpawnedEnemy { get; private set; }
        // Parameters & States/Decisions ?
    }
}
