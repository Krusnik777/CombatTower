using System;
using DI;
using R3;
using StateMachine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class GuardState : MovementState
    {
        protected const string _guardStateBool = "IsGuardState";
        protected const string _hasShieldBool = "HasShield";

        protected override float _movementSpeed => _player.ParametersConfig.MovementSpeedInGuard;
        protected override float _rotationSpeed => _player.ParametersConfig.RotationSpeedInGuard;

        private IDisposable _guardEndListenerDisposable;

        public GuardState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }

        public override void Enter()
        {
            base.Enter();

            _player.Animator.SetBool(_hasShieldBool, false); // TO for something like _player.HasShield;
            _player.Animator.SetBool(_guardStateBool, true);
            _player.Animator.SetBool(_battleStateBool, false);

            _guardEndListenerDisposable = _gameInputService.Guard.Where(v => v == false).Subscribe(_ =>
            {
                _guardEndListenerDisposable?.Dispose();

                _parentStateMachine.SetState<BattleState, bool>(false);
            });
        }

        public override void Exit()
        {
            DisposeOfListeners();

            _player.Animator.SetBool(_guardStateBool, false);

            base.Exit();
        }

        private void DisposeOfListeners()
        {
            _guardEndListenerDisposable?.Dispose();
        }
    }
}
