using System;
using UnityEngine;
using R3;
using System.Collections.Generic;
using CombatTower.Game.Gameplay.HealthSystem;

namespace CombatTower.Game.Gameplay.Entities
{
    public class WeaponDamageDealer : IDamageDealer
    {
        private const float _arcAngle = 90f;

        private LayerMask _targetMask;
        private Transform _transform;
        private float _range;
        private AnimatorEventsCollector _eventsCollector;
        private Transform _swordTransform;

        private Action _onAttack;

        private HashSet<IDamageable> _hittedDamageables;

        private IDisposable _attackStartedListenerDisposable;
        private IDisposable _attackCheckDisposable;
        private IDisposable _attackExecutedListenerDisposable;

        public WeaponDamageDealer(LayerMask targetMask, Transform transform, float range, AnimatorEventsCollector eventsCollector, Transform swordTransform)
        {
            _targetMask = targetMask;
            _transform = transform;
            _range = range;
            _eventsCollector = eventsCollector;
            _swordTransform = swordTransform;

            _hittedDamageables = new();

            _attackStartedListenerDisposable = _eventsCollector.OnAttackStart.Subscribe(_ => StartTrackingEnemies());
            _attackExecutedListenerDisposable = _eventsCollector.OnAttackExecute.Subscribe(_ => DamageTrackedEnemies());
        }

        public void Dispose()
        {
            _attackStartedListenerDisposable?.Dispose();
            _attackCheckDisposable?.Dispose();
            _attackExecutedListenerDisposable?.Dispose();
        }

        public void SubscribeToAttack(Action onAttack)
        {
            _onAttack += onAttack;
        }

        private void StartTrackingEnemies()
        {
            _attackStartedListenerDisposable?.Dispose();

            _attackCheckDisposable = Observable.EveryUpdate().Subscribe(_ => TrackEnemies());
        }

        private void DamageTrackedEnemies()
        {
            _attackCheckDisposable?.Dispose();
            _attackExecutedListenerDisposable?.Dispose();

            foreach (var damageable in _hittedDamageables)
            {
                //var damage = DamageFactory.Create(_attackConfig);
                var damage = DamageFactory.Create();
                damageable.Hit(damage);
            }

            _onAttack?.Invoke();

            _hittedDamageables.Clear();
        }

        private void TrackEnemies()
        {
            //Collider[] colliders = Physics.OverlapSphere(_swordTransform.position, _attackConfig.Range, _targetMask);
            Collider[] colliders = Physics.OverlapSphere(_swordTransform.position, _range, _targetMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                var damageable = colliders[i].transform.GetComponent<IDamageable>();

                if (damageable == null) continue;
                if (_hittedDamageables.Contains(damageable)) continue;

                Vector3 directionToDamageable = (colliders[i].transform.root.position - _transform.position).normalized;

                float angle = Vector3.Angle(_transform.forward, directionToDamageable);

                if (angle > _arcAngle) continue;

                _hittedDamageables.Add(damageable);
            }
        }
    }
}
