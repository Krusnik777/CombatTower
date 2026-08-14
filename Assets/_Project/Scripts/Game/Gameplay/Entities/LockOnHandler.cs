using System;
using CombatTower.Game.Gameplay.Entities.Enemy;
using R3;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class LockOnHandler : IDisposable
    {
        private const float _lockOnDetectionRange = 8f; // Temp?
        private const float _cooldownAfterSwitchTargetMs = 250f;

        public bool IsEnabled { get; set; }
        public EnemyView CurrentEnemy { get; private set; }

        private Transform _controlledCamera;
        private LockOnCamera _lockOnCamera;
        private PlayerAvatarMovement _playerAvatarMovement;

        private IEnemyDetector _enemyDetector;
        private Observable<int> _onSwitchTargetPressedObservable;

        private bool _inCooldown;

        private IDisposable _lockOnInputListenerDisposable;
        private IDisposable _targetLostUpdateListenerDisposable;
        private IDisposable _switchTargetListenerDisposable;
        private IDisposable _switchTargetCooldownListenerDisposable;

        public LockOnHandler(Transform controlledCamera, LockOnCamera lockOnCamera, PlayerAvatarMovement playerAvatarMovement, Transform detectionCenterTransform)
        {
            _controlledCamera = controlledCamera;
            _lockOnCamera = lockOnCamera;
            _playerAvatarMovement = playerAvatarMovement;

            _enemyDetector = new EnemyDetector(1 << LayerMask.NameToLayer("Enemy"), detectionCenterTransform, _lockOnDetectionRange);

            IsEnabled = true;
            _inCooldown = false;
        }

        public void Dispose()
        {
            _lockOnInputListenerDisposable?.Dispose();
            _targetLostUpdateListenerDisposable?.Dispose();
            _switchTargetListenerDisposable?.Dispose();
            _switchTargetCooldownListenerDisposable?.Dispose();
        }

        public void SubcribeToLockOnInput(Subject<Unit> onLockOnPressed, Subject<int> onSwitchTargetPressed)
        {
            _lockOnInputListenerDisposable?.Dispose();
            _lockOnInputListenerDisposable = onLockOnPressed.Subscribe(_ => TrySetTarget(true));

            _switchTargetListenerDisposable?.Dispose();
            _switchTargetCooldownListenerDisposable?.Dispose();
            _onSwitchTargetPressedObservable = onSwitchTargetPressed;
        }

        private void TrySetTarget(bool byInput = false)
        {
            if (IsEnabled)
            {
                if (CurrentEnemy == null || !byInput)
                {
                    CurrentEnemy = _enemyDetector.TryGetClosestEnemy();
                    // need to do subcribtion to death
                }
                else
                {
                    CurrentEnemy = null;
                }
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

            _switchTargetListenerDisposable?.Dispose();
            _targetLostUpdateListenerDisposable?.Dispose();
            SetCooldownAsFinished();

            if (isLockedOnEnemy)
            {
                _switchTargetListenerDisposable = _onSwitchTargetPressedObservable.Subscribe(TrySwitchTarget);
                _targetLostUpdateListenerDisposable = Observable.EveryUpdate().Subscribe(_ =>
                {
                    if (CurrentEnemy == null || CurrentEnemy != null && !_enemyDetector.IsCloseEnoughToTarget(CurrentEnemy))
                    {
                        _targetLostUpdateListenerDisposable?.Dispose();
                        TrySetTarget();
                    }
                });
            }
        }

        private void TrySwitchTarget(int direction)
        {
            if (!IsEnabled) return;
            if (_inCooldown) return;
            if (CurrentEnemy == null) return;

            var target = _enemyDetector.GetClosestEnemyByDirection(CurrentEnemy, direction);
            if (target == null) return;

            CurrentEnemy = target;
            // need to do subcribtion to death

            _playerAvatarMovement.SetLookTransform(CurrentEnemy.LookTarget);
            _lockOnCamera.SetLookTarget(CurrentEnemy.LookTarget);

            _inCooldown = true;

            _switchTargetCooldownListenerDisposable = Observable.Timer(TimeSpan.FromMilliseconds(_cooldownAfterSwitchTargetMs)).Subscribe(_ => SetCooldownAsFinished());
        }

        private void SetCooldownAsFinished()
        {
            _switchTargetCooldownListenerDisposable?.Dispose();
            _inCooldown = false;
        }
    }
}
