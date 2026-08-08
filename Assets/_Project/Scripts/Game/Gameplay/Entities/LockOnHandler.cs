using System;
using CombatTower.Game.Gameplay.Entities.Enemy;
using R3;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class LockOnHandler : IDisposable
    {
        private const float _lockOnDetectionRange = 7f; // Temp?

        public bool IsEnabled { get; set; }
        public EnemyView CurrentEnemy { get; private set; }

        private Transform _controlledCamera;
        private LockOnCamera _lockOnCamera;
        private PlayerAvatarMovement _playerAvatarMovement;

        private IEnemyDetector _enemyDetector;

        private IDisposable _lockOnInputListenerDisposable;

        public LockOnHandler(Transform controlledCamera, LockOnCamera lockOnCamera, PlayerAvatarMovement playerAvatarMovement, Transform detectionCenterTransform)
        {
            _controlledCamera = controlledCamera;
            _lockOnCamera = lockOnCamera;
            _playerAvatarMovement = playerAvatarMovement;

            _enemyDetector = new EnemyDetector(1 << LayerMask.NameToLayer("Enemy"), detectionCenterTransform, _lockOnDetectionRange);

            IsEnabled = true;
        }

        public void SubcribeToLockOnInput(Subject<Unit> onLockOnPressed)
        {
            _lockOnInputListenerDisposable?.Dispose();
            _lockOnInputListenerDisposable = onLockOnPressed.Subscribe(_ => TryLookAtTarget());
        }

        private void TryLookAtTarget()
        {
            if (!IsEnabled) return;

            if (CurrentEnemy == null)
            {
                CurrentEnemy = _enemyDetector.TryGetClosestEnemy();
                // need to do subcribtion to death
            }
            else
            {
                CurrentEnemy = null;
            }

            bool isLockedOnEnemy = CurrentEnemy != null;

            _playerAvatarMovement.SetLookTransform(isLockedOnEnemy ? CurrentEnemy.LookTarget : null);
            _controlledCamera.gameObject.SetActive(!isLockedOnEnemy);
            _lockOnCamera.SetLookTarget(isLockedOnEnemy ? CurrentEnemy.LookTarget : null);
            _lockOnCamera.gameObject.SetActive(isLockedOnEnemy);
        }

        public void Dispose()
        {
            _lockOnInputListenerDisposable?.Dispose();
        }
    }
}
