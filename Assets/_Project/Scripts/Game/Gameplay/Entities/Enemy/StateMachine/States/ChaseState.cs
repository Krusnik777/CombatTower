using System;
using CombatTower.Game.Gameplay.Battle;
using DI;
using R3;
using StateMachine;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class ChaseState : IEnterableState
    {
        protected const string _forwardMoveFloat = "ForwardMove";
        protected const string _sidewardMoveFloat = "SidewardMove";
        protected const float _movementThreshold = 0.05f;

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Enemy _enemy;
        private IBattleStageController _battleStageController;

        private IDisposable _movementUpdateDisposable;

        public ChaseState(IStateMachine parentStateMachine, Enemy enemy, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _enemy = enemy;
            _sceneContainer = sceneContainer;

            _battleStageController = sceneContainer.Resolve<IBattleStageController>();
        }

        public void Enter()
        {
            DisposeOfListeners();

            _enemy.Agent.isStopped = false;

            _movementUpdateDisposable = Observable.EveryUpdate().Subscribe(_ => UpdateMovement());
        }

        public void Exit()
        {
            DisposeOfListeners();

            _battleStageController.RemoveEnemy(_enemy);
        }

        private void DisposeOfListeners()
        {
            _movementUpdateDisposable?.Dispose();
        }

        private void UpdateMovement()
        {
            if (_enemy.Agent == null)
            {
                DisposeOfListeners();
                return;
            }

            Vector3 targetPos = _enemy.Agent.transform.position;

            if (_battleStageController.TryGetPosition(_enemy, out Transform moveTarget))
            {
                targetPos = moveTarget.position;
            }
            
            _enemy.Agent.SetDestination(targetPos);

            _enemy.Animator.SetFloat(_forwardMoveFloat, _enemy.Agent.velocity.magnitude >= _movementThreshold ? 1f : 0f); // temp
            _enemy.Animator.SetFloat(_sidewardMoveFloat, 0f);
        }
    }
}
