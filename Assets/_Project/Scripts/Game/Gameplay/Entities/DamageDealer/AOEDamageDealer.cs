using System;
using UnityEngine;
using R3;
using CombatTower.Game.Gameplay.HealthSystem;

namespace CombatTower.Game.Gameplay.Entities
{
    public class AOEDamageDealer : IDamageDealer
    {
        private LayerMask _targetMask;
        private Transform _transform;
        private float _range;
        private AnimatorEventsCollector _eventsCollector;

        private Action _onAttack;

        private IDisposable _attackExecutedListenerDisposable;

        public AOEDamageDealer(LayerMask targetMask, Transform transform, float range, AnimatorEventsCollector eventsCollector)
        {
            _targetMask = targetMask;
            _transform = transform;
            _range = range;
            _eventsCollector = eventsCollector;

            _attackExecutedListenerDisposable = _eventsCollector.OnAttackExecute.Subscribe(_ => DamageAllInRange());
        }

        public void Dispose()
        {
            _attackExecutedListenerDisposable?.Dispose();
        }

        public void SubscribeToAttack(Action onAttack)
        {
            _onAttack += onAttack;
        }

        private void DamageAllInRange()
        {
            //_attackExecutedListenerDisposable?.Dispose();

            //var damage = DamageFactory.Create(_attackConfig);
            var damage = DamageFactory.Create();

            //Collider[] colliders = Physics.OverlapSphere(_transform.position, _attackConfig.Range, _targetMask);
            Collider[] colliders = Physics.OverlapSphere(_transform.position, _range, _targetMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                var damageable = colliders[i].transform.GetComponent<IDamageable>();

                damageable?.Hit(damage);
            }

            _onAttack?.Invoke();
        }
    }
}
