using StateMachine;
using DI;
using R3;
using CombatTower.Game.Services;
using System;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class DodgeState : IEnterableState<IState>
    {
        private const string _forwardMoveFloat = "ForwardMove";
        private const string _sidewardMoveFloat = "SidewardMove";
        private const string _dodgeTrigger = "Dodge";
        private const string _rollTrigger = "Roll";

        private const float _invulnerabilityWindowMs = 200f;

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        private GameInputService _gameInputService;
        private LockOnHandler _lockOnHandler;

        private IState _previousState;
        private bool _guardHolded;

        private IDisposable _dodgeInputListenerDisposable;
        private IDisposable _dodgeFinishListenerDisposable;
        private IDisposable _guardListenerDisposable;

        public DodgeState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;

            _gameInputService = _sceneContainer.Resolve<GameInputService>();
            _lockOnHandler = _sceneContainer.Resolve<LockOnHandler>();
        }

        public virtual void Enter(IState previousState)
        {
            DisposeOfListeners();

            _previousState = previousState;
            _guardHolded = false;

            _dodgeInputListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ =>
            {
                DisposeOfListeners();

                SetDirection();
                _player.Animator.SetTrigger(_rollTrigger);

                _dodgeFinishListenerDisposable = _player.EventsCollector.OnDodgeEnd.Subscribe(_ => OnDodgeEnd());
            });
            _dodgeFinishListenerDisposable = _player.EventsCollector.OnDodgeEnd.Subscribe(_ => OnDodgeEnd());

            _guardListenerDisposable = _gameInputService.Guard.Subscribe(value => _guardHolded = value);

            _player.Movement.IsControlledByRootMotion = true;
            _lockOnHandler.IsEnabled = false;

            SetDirection();
            _player.Animator.SetTrigger(_dodgeTrigger);
        }
        public virtual void Exit()
        {
            DisposeOfListeners();

            _player.Movement.IsControlledByRootMotion = false;
            _lockOnHandler.IsEnabled = true;
        }

        private void DisposeOfListeners()
        {
            _dodgeInputListenerDisposable?.Dispose();
            _dodgeFinishListenerDisposable?.Dispose();
            _guardListenerDisposable?.Dispose();
        }

        private void OnDodgeEnd()
        {
            DisposeOfListeners();

            if (_guardHolded)
            {
                _parentStateMachine.SetState<GuardState>();

                return;
            }

            if (_previousState is CalmState) _parentStateMachine.SetState<CalmState>();
            if (_previousState is BattleState) _parentStateMachine.SetState<BattleState, bool>(false);
        }

        private void SetDirection()
        {
            var direction = _gameInputService.GetMovementInput();

            if (direction == Vector3.zero)
            {
                _player.Animator.SetFloat(_forwardMoveFloat, -1f);
                _player.Animator.SetFloat(_sidewardMoveFloat, 0f);

                return;
            }

            if (_lockOnHandler.CurrentEnemy != null)
            {
                var localDirection = _player.Rigidbody.transform.InverseTransformDirection(direction);

                if (Mathf.Abs(localDirection.x) < Mathf.Abs(localDirection.z))
                {
                    localDirection.Normalize();
                    _player.Animator.SetFloat(_sidewardMoveFloat, 0f);
                    _player.Animator.SetFloat(_forwardMoveFloat, localDirection.z);
                }
                else
                {
                    localDirection.Normalize();
                    _player.Animator.SetFloat(_sidewardMoveFloat, localDirection.x);
                    _player.Animator.SetFloat(_forwardMoveFloat, 0);
                }

                return;
            }

            _player.Movement.SetRotationDirection(direction, 5f);
            _player.Animator.SetFloat(_sidewardMoveFloat, 0f);
            _player.Animator.SetFloat(_forwardMoveFloat, direction != Vector3.zero ? 1 : -1f);
        }
    }
}

