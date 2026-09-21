using System;
using DI;
using StateMachine;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class DamageState : IEnterableState
    {
        private const string _getHitTrigger = "GetHit";
        private const string _hitTypeInt = "HitType";
        private const int _hitTypesAmount = 5;

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

        public void Enter()
        {
            DisposeOfListeners();

            _hitRecoveryListenerDisposable = _enemy.EventsCollector.OnRecoveryAfterHit.Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<IdleState>();
            });

            _enemy.Animator.SetInteger(_hitTypeInt, UnityEngine.Random.Range(0, _hitTypesAmount));
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
