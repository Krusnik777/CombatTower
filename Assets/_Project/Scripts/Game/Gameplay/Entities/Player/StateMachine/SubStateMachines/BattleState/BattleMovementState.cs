using StateMachine;
using DI;
using R3;
using System;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleMovementState : MovementState
    {       
        private float _timer;

        private IDisposable _attackListenerDisposable;
        private IDisposable _dodgeListenerDisposable;
        private IDisposable _guardListenerDisposable;
        private IDisposable _returnToCalmTimerListenerDisposable;

        public BattleMovementState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }

        public override void Enter()
        {
            base.Enter();

            DisposeOfListeners();

            _player.Animator.SetBool(_battleStateBool, true);

            _attackListenerDisposable = _gameInputService.OnAttackPressed.Subscribe(OnAttack);
            _dodgeListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ => OnDodge());
            _guardListenerDisposable = _gameInputService.Guard.Where(v => v == true).Subscribe(_ => OnGuard());
            _returnToCalmTimerListenerDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => UpdateTimer());
        }

        public override void Exit()
        {
            DisposeOfListeners();

            base.Exit();
        }

        private void DisposeOfListeners()
        {
            _attackListenerDisposable?.Dispose();
            _dodgeListenerDisposable?.Dispose();
            _guardListenerDisposable?.Dispose();
            _returnToCalmTimerListenerDisposable?.Dispose();
        }

        private void OnAttack(bool isHoldAttack)
        {
            DisposeOfListeners();
            
            _parentStateMachine.SetState<AttackState, bool>(isHoldAttack);
        }

        private void OnDodge()
        {
            DisposeOfListeners();

            _parentStateMachine.SetState<BattleExitState, BattleState.ExitTag>(BattleState.ExitTag.Dodge);
        }

        private void OnGuard()
        {
            DisposeOfListeners();

            _parentStateMachine.SetState<BattleExitState, BattleState.ExitTag>(BattleState.ExitTag.Guard);
        }

        private void UpdateTimer()
        {
            if (_gameInputService.GetMovementInput() != Vector3.zero) _timer = 0f;
            else _timer++;

            if (_timer >= _player.ParametersConfig.BattleStateExitTime)
            {
                DisposeOfListeners();
                
                _parentStateMachine.SetState<BattleExitState, BattleState.ExitTag>(BattleState.ExitTag.Timer);
            }
        }
    }
}
