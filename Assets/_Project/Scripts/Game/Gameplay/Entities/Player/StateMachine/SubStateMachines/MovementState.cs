using StateMachine;
using DI;
using System;
using R3;
using CombatTower.Game.Services;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class MovementState : IEnterableState
    {
        protected const string _battleStateBool = "IsBattleState";
        protected const string _forwardMoveFloat = "ForwardMove";
        protected const string _sidewardMoveFloat = "SidewardMove";
        protected const float _movementThreshold = 0.05f;

        protected IStateMachine _parentStateMachine;
        protected DIContainer _sceneContainer;
        protected Player _player;
        protected GameInputService _gameInputService;

        protected virtual float _movementSpeed => 250f; // TEMP
        protected virtual float _rotationSpeed => 500f; // TEMP

        private IDisposable _animatorMovementUpdateDisposable;

        public MovementState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;

            _gameInputService = _sceneContainer.Resolve<GameInputService>();
        }

        public virtual void Enter()
        {
            _player.Animator.SetBool(_battleStateBool, false);
            _player.Movement.Bind(_gameInputService, _movementSpeed, _rotationSpeed);

            _animatorMovementUpdateDisposable?.Dispose();
            _animatorMovementUpdateDisposable = Observable.EveryUpdate().Subscribe(_ => UpdateAnimatorMovement());
        }

        public virtual void Exit()
        {
            _player.Movement.Bind(null);
            _animatorMovementUpdateDisposable?.Dispose();
        }

        private void UpdateAnimatorMovement()
        {
            if (_player.Movement == null) return;

            var localLookDirection = _player.Movement.GetLocalLookDirection();

            _player.Animator.SetFloat(_forwardMoveFloat, localLookDirection.z);
            _player.Animator.SetFloat(_sidewardMoveFloat, localLookDirection.x);
        }
    }
}
