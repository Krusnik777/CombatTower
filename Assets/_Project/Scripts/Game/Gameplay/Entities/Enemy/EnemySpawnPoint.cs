using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [field: SerializeField] public EnemyType SpawnedEnemy { get; private set; }
        [field: SerializeField] public PatrolPointView[] PatrolRoute { get; private set; }
        // Parameters & States/Decisions ?

        #if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (PatrolRoute == null) return;
            if (PatrolRoute.Length == 0) return;

            Gizmos.color = Color.red;

            var offsetY = new Vector3(0, 0.1f, 0);

            for (int i = 0; i < PatrolRoute.Length - 1; i++)
            {
                Gizmos.DrawLine(PatrolRoute[i].transform.position + offsetY, PatrolRoute[i + 1].transform.position + offsetY);
            }

            Gizmos.DrawLine(PatrolRoute[0].transform.position + offsetY, PatrolRoute[PatrolRoute.Length - 1].transform.position + offsetY);
        }

        #endif
    }
}
