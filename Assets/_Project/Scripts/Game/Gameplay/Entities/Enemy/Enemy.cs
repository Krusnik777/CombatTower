using System;
using CombatTower.Game.Configs;
using CombatTower.Game.Gameplay.HealthSystem;
using R3;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class Enemy : IDisposable
    {
        public Subject<Enemy> OnDeath { get; }
        public Subject<Damage> OnDamage { get; }

        public Health Health { get; }
        
        private EnemyConfig _config;
        private EnemyView _view;

        public EnemyConfig Config => _config;
        public Animator Animator => _view.Animator;
        public AnimatorEventsCollector EventsCollector => _view.EventsCollector;

        private EnemyStateMachine _stateMachine;

        private IDisposable _damageableListenerDisposable;
        private IDisposable _deathDisposable;

        public Enemy(EnemyConfig config, EnemyView view, DI.DIContainer sceneContainer)
        {
            _config = config;
            _view = view;

            Health = new(new DamageProcessor(), config.MaxHealth);

            _damageableListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);

            _stateMachine = new(this, sceneContainer);
        }

        public void Dispose()
        {
            _damageableListenerDisposable?.Dispose();
            _deathDisposable?.Dispose();
        }

        private void TakeDamage(Damage damage)
        {
            /*bool isBlocked = UnityEngine.Random.value >= 1 - someArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(someArmorValue));*/

            if (Health.TakeDamage(ref damage, out int resultedHealthValue))
            {
                if (resultedHealthValue <= 0)
                {
                    _stateMachine.SetState<DeathState>();

                    _view.enabled = false;

                    _deathDisposable = Observable.Interval(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
                    {
                        _view.gameObject.SetActive(false);

                        Dispose();
                    });

                    // Stop all inner processes
                    OnDeath?.OnNext(this);
                }
                else
                {
                    _stateMachine.SetState<DamageState, Damage>(damage);
                    // Show hit effect ?
                    OnDamage?.OnNext(damage);
                    //_sounds.Play(PlayerSounds.Damage); // ???
                }
            }
        }
    }
}
