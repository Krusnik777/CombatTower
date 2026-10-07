using System;
using CombatTower.Game.Configs;
using CombatTower.Game.Gameplay.HealthSystem;
using R3;
using UnityEngine;
using UnityEngine.AI;

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
        public NavMeshAgent Agent => _view.Agent;

        public PatrolPointView[] PatrolRoute { get; private set; }
        public FieldOfView FieldOfView { get; private set; }

        private EnemyStateMachine _stateMachine;

        private IDisposable _damageableListenerDisposable;
        private IDisposable _deathDisposable;
        private IDisposable _TEMPfieldOfViewListenerDisposable;

        public Enemy(EnemyConfig config, EnemyView view, DI.DIContainer sceneContainer, PatrolPointView[] patrolRoute)
        {
            _config = config;
            _view = view;

            Health = new(new DamageProcessor(), config.MaxHealth);
            FieldOfView = new(view.FieldOfViewParameters, sceneContainer.Resolve<Player.Player>().Movement.transform);
            PatrolRoute = patrolRoute;

            _damageableListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);

            _stateMachine = new(this, sceneContainer);

            // TEMP

            _TEMPfieldOfViewListenerDisposable = FieldOfView.CanSeeTarget.Subscribe(result =>
            {
                if (result) Debug.Log("[ENEMY] " + _view.gameObject.name + " SEE PLAYER");
            });
        }

        public void Dispose()
        {
            FieldOfView?.Dispose();
            _damageableListenerDisposable?.Dispose();
            _deathDisposable?.Dispose();

            _TEMPfieldOfViewListenerDisposable?.Dispose();
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
