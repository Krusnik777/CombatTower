using StateMachine;
using DI;
using R3;
using System;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class CalmState : MovementState
    {
        private IDisposable _attackListenerDisposable;
        private IDisposable _dodgeListenerDisposable;
        private IDisposable _guardListenerDisposable;

        public CalmState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }

        public override void Enter()
        {
            base.Enter();

            DisposeOfListeners();

            _attackListenerDisposable = _gameInputService.OnAttackPressed.Subscribe(_ => OnAttack());
            _dodgeListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ => OnDodge());
            _guardListenerDisposable = _gameInputService.Guard.Where(v => v == true).Subscribe(_ => OnGuard());
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
        }

        private void OnAttack()
        {
            DisposeOfListeners();

            _parentStateMachine.SetState<BattleState, bool>(true);
        }

        private void OnDodge()
        {
            DisposeOfListeners();

            _parentStateMachine.SetState<DodgeState, IState>(this);
        }

        private void OnGuard()
        {
            DisposeOfListeners();

            _parentStateMachine.SetState<GuardState>();
        }
    }
}
