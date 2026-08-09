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

                var directionBetween = (potentialTarget.transform.position - _detectionCenterTransform.position).normalized;
                directionBetween.y = 0;
                var lookDirection = Camera.main != null ? Camera.main.transform.forward : _detectionCenterTransform.forward;
                lookDirection.y = 0;
                var angleBetween = Vector3.Angle(lookDirection, directionBetween);

                var targetPosition = potentialTarget.transform.position;
                targetPosition.y = 0;
                var centerPosition = _detectionCenterTransform.position;
                centerPosition.y = 0;
                var distance = Vector3.Distance(targetPosition, centerPosition);

                var score = angleBetween + distance * 0.1f;

                if (score < targetScore)
                {
                    targetScore = score;
                    target = potentialTarget;
                }
            }

            return target;
        }

        public EnemyView GetClosestEnemyByDirection(EnemyView currentEnemy, int direction)
        {
            Collider[] colliders = Physics.OverlapSphere(_detectionCenterTransform.position, _detectionRange, _enemyMask);

            var directionToTarget = (currentEnemy.transform.position - _detectionCenterTransform.position).normalized;
            directionToTarget.y = 0;
            var lookDirection = Camera.main != null ? Camera.main.transform.forward : _detectionCenterTransform.forward;
            lookDirection.y = 0;
            float currentAngle = Vector3.SignedAngle(lookDirection, directionToTarget, Vector3.up);
            float bestDelta = (direction > 0) ? float.MaxValue : float.MinValue;

            EnemyView target = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                var potentialTarget = colliders[i].transform.root.GetComponent<EnemyView>();

                if (potentialTarget == null) continue;
                if (currentEnemy != null && potentialTarget == currentEnemy) continue;

                var dir = (potentialTarget.transform.position - _detectionCenterTransform.position).normalized;
                dir.y = 0;
                float targetAngle = Vector3.SignedAngle(lookDirection, dir, Vector3.up);
                float delta = Mathf.DeltaAngle(currentAngle, targetAngle);

                if (direction > 0 && delta > 0 && delta < bestDelta)
                {
                    bestDelta = delta;
                    target = potentialTarget;
                }
                else if (direction < 0 && delta < 0 && delta > bestDelta)
                {
                    bestDelta = delta;
                    target = potentialTarget;
                }
            }

            return target;
        }

        public bool IsCloseEnoughToTarget(EnemyView targetEnemy)
        {
            var targetPosition = targetEnemy.transform.position;
            targetPosition.y = 0;
            var centerPosition = _detectionCenterTransform.position;
            centerPosition.y = 0;
            var distance = Vector3.Distance(targetPosition, centerPosition);

            return distance <= _detectionRange;
        }
    }
}
