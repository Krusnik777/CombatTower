using StateMachine;
using DI;
using R3;
using System;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleMovementState : MovementState
    {
        private IStateMachine _playerStateMachine;

        private float _targetTime = 10f;
        private float _timer;

        private IDisposable _attackListenerDisposable;
        private IDisposable _returnToCalmTimerListenerDisposable;

        public BattleMovementState(IStateMachine parentStateMachine, Player player, IStateMachine playerStateMachine, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer)
        {
            _playerStateMachine = playerStateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            _player.Animator.SetBool(_battleStateBool, true);

            _attackListenerDisposable?.Dispose();
            _attackListenerDisposable = _gameInputService.OnAttackPressed.Subscribe(_ => OnAttack());

            _returnToCalmTimerListenerDisposable?.Dispose();
            _returnToCalmTimerListenerDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => UpdateTimer());
        }

        public override void Exit()
        {
            _attackListenerDisposable?.Dispose();
            _returnToCalmTimerListenerDisposable?.Dispose();

            base.Exit();
        }

        private void OnAttack()
        {
            _attackListenerDisposable?.Dispose();
            
            _parentStateMachine.SetState<AttackState>();
        }

        private void UpdateTimer()
        {
            if (_gameInputService.GetMovementInput() != Vector3.zero) _timer = 0f;
            else _timer++;

            if (_timer >= _targetTime)
            {
                _returnToCalmTimerListenerDisposable?.Dispose();
                _playerStateMachine.SetState<CalmState>();
            }
        }
    }
}
