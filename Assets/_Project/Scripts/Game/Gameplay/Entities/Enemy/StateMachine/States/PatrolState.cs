using System;
using DI;
using R3;
using StateMachine;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class PatrolState : IEnterableState
    {
        protected const string _forwardMoveFloat = "ForwardMove";
        protected const string _sidewardMoveFloat = "SidewardMove";
        protected const float _movementThreshold = 0.05f;

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Enemy _enemy;

        private int _currentPointIndex;
        private bool _isStopped;

        private IDisposable _movementUpdateDisposable;
        private IDisposable _waitUpdateListenerDisposable;

        public PatrolState(IStateMachine parentStateMachine, Enemy enemy, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _enemy = enemy;
            _sceneContainer = sceneContainer;

            _currentPointIndex = 0;
        }

        public void Enter()
        {
            DisposeOfListeners();

            if (_enemy.PatrolRoute == null || _enemy.PatrolRoute.Length <= 1)
            {
                _parentStateMachine.SetState<IdleState>();
                return;
            }

            _enemy.Agent.isStopped = false;

            _enemy.Agent.SetDestination(_enemy.PatrolRoute[_currentPointIndex].transform.position);

            _movementUpdateDisposable = Observable.EveryUpdate().Subscribe(_ => UpdateMovement());
        }

        public void Exit()
        {
            DisposeOfListeners();
        }

        private void DisposeOfListeners()
        {
            _movementUpdateDisposable?.Dispose();
            _waitUpdateListenerDisposable?.Dispose();
        }

        private void UpdateMovement()
        {
            if (_enemy.Agent == null)
            {
                DisposeOfListeners();
                return;
            }

            _enemy.Animator.SetFloat(_forwardMoveFloat, _enemy.Agent.velocity.magnitude >= _movementThreshold ? 1f : 0f); // temp
            _enemy.Animator.SetFloat(_sidewardMoveFloat, 0f);

            if (_isStopped) return;

            if (Vector3.Distance(_enemy.Agent.transform.position, _enemy.PatrolRoute[_currentPointIndex].transform.position) <= 1f)
            {
                _isStopped = true;

                _waitUpdateListenerDisposable = Observable.Timer(TimeSpan.FromSeconds(_enemy.PatrolRoute[_currentPointIndex].WaitingTime)).Subscribe(_ => MoveToNextPatrolPoint());

                _enemy.Agent.isStopped = true;
            }
        }

        private void MoveToNextPatrolPoint()
        {
            _waitUpdateListenerDisposable?.Dispose();

            _currentPointIndex = (_currentPointIndex+ 1) % _enemy.PatrolRoute.Length; 

            _isStopped = false;
            
            _enemy.Agent.isStopped = false;
            _enemy.Agent.SetDestination(_enemy.PatrolRoute[_currentPointIndex].transform.position);
        }
    }
}
