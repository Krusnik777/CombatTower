using CombatTower.Game.Gameplay.Entities.Enemy;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities
{
    public class EnemyDetector : IEnemyDetector
    {
        private LayerMask _enemyMask;
        private Transform _detectionCenterTransform;
        private float _detectionRange;

        public EnemyDetector(LayerMask enemyMask, Transform detectionCenterTransform, float detectionRange)
        {
            _enemyMask = enemyMask;
            _detectionCenterTransform = detectionCenterTransform;
            _detectionRange = detectionRange;
        }

        public void Dispose()
        {
            
        }

        public EnemyView TryGetClosestEnemy(EnemyView currentDetectedEnemy = null)
        {
            Collider[] colliders = Physics.OverlapSphere(_detectionCenterTransform.position, _detectionRange, _enemyMask);

            EnemyView target = null;
            float targetScore = float.MaxValue;

            for (int i = 0; i < colliders.Length; i++)
            {
                var potentialTarget = colliders[i].transform.root.GetComponent<EnemyView>();

                if (potentialTarget == null) continue;
                if (currentDetectedEnemy != null && potentialTarget == currentDetectedEnemy) continue;

                var direction = (potentialTarget.transform.position - _detectionCenterTransform.position).normalized;
                var angleBetween = Vector3.Angle(Camera.main != null ? Camera.main.transform.forward : _detectionCenterTransform.forward, direction);
                var distance = Vector3.Distance(potentialTarget.transform.position, _detectionCenterTransform.position);
                var score = angleBetween + distance * 0.1f;

                if (score < targetScore)
                {
                    targetScore = score;
                    target = potentialTarget;
                }
            }
            
            return target;
        }
    }
}
