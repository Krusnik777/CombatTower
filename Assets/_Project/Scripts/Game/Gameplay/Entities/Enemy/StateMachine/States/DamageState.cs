using System;
using DI;
using StateMachine;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class DamageState : IEnterableState<HealthSystem.Damage>
    {
        private const string _getHitTrigger = "GetHit";
        private const string _hitTypeInt = "HitType";
        private readonly int[] _frontSimpleHits = new[]{ 0, 1, 2 };
        private readonly int[] _frontHardHits = new[]{ 2 };
        private readonly int[] _backSimpleHits = new[]{ 3, 4 };
        private readonly int[] _backHardHits = new[]{ 4 };

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Enemy _enemy;

        private IDisposable _hitRecoveryListenerDisposable;

        public DamageState(IStateMachine parentStateMachine, Enemy enemy, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _enemy = enemy;
            _sceneContainer = sceneContainer;
        }

        public void Enter(HealthSystem.Damage damage)
        {
            DisposeOfListeners();

            _hitRecoveryListenerDisposable = _enemy.EventsCollector.OnRecoveryAfterHit.Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<IdleState>();
            });

            int hitIndex = 0;

            if (damage.Direction == HealthSystem.DamageDirection.Back)
            {
                hitIndex = _backSimpleHits[UnityEngine.Random.Range(0, _backSimpleHits.Length)];
            }
            else
            {
                hitIndex = _frontSimpleHits[UnityEngine.Random.Range(0, _frontSimpleHits.Length)];
            }

            _enemy.Animator.SetInteger(_hitTypeInt, hitIndex);
            _enemy.Animator.SetTrigger(_getHitTrigger);
        }

        public void Exit()
        {
            DisposeOfListeners();


        }

        private void DisposeOfListeners()
        {
            _hitRecoveryListenerDisposable?.Dispose();
        }
    }
}
