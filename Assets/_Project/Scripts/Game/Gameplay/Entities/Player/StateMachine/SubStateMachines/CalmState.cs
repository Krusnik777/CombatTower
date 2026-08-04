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

        public CalmState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }

        public override void Enter()
        {
            base.Enter();

            DisposeOfListeners();

            _attackListenerDisposable = _gameInputService.OnAttackPressed.Subscribe(_ => OnAttack());
            _dodgeListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ => OnDodge());
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
    }
}
