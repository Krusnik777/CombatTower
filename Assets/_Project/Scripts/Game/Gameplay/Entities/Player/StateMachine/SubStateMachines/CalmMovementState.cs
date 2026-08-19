using StateMachine;
using DI;
using R3;
using System;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class CalmMovementState : MovementState
    {
        private IDisposable _attackListenerDisposable;
        private IDisposable _dodgeListenerDisposable;
        private IDisposable _guardListenerDisposable;

        public CalmMovementState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }

        public override void Enter()
        {
            base.Enter();

            DisposeOfListeners();

            _player.SetWeaponActive(false);
            _player.Animator.SetBool(_battleStateBool, false);

            _attackListenerDisposable = _gameInputService.OnAttackPressed.Subscribe(OnAttack);
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

        private void OnAttack(bool isHoldAttack)
        {
            DisposeOfListeners();

            _parentStateMachine.SetState<BattleState, BattleState.EntryTag>(isHoldAttack ? BattleState.EntryTag.HoldAttack : BattleState.EntryTag.SimpleAttack);
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
