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

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        private GameInputService _gameInputService;
        private LockOnHandler _lockOnHandler;

        private IState _previousState;
        private bool _isRoll;
        private bool _guardHolded;
        private bool _holdAttackPending;

        private IDisposable _dodgeInputListenerDisposable;
        private IDisposable _dodgeFinishListenerDisposable;
        private IDisposable _guardListenerDisposable;
        private IDisposable _guardLinkListenerDisposable;
        private CompositeDisposable _holdAttackListenersDisposables;

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

            _player.Animator.ResetTrigger(_rollTrigger);

            _previousState = previousState;
            _isRoll = false;
            _guardHolded = false;
            _holdAttackPending = false;

            _dodgeInputListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ =>
            {
                DisposeOfListeners(false);

                SetDirection();
                _isRoll = true;
                _player.Animator.SetTrigger(_rollTrigger);

                _dodgeFinishListenerDisposable = _player.EventsCollector.OnDodgeEnd.Subscribe(OnDodgeEnd);
                _guardLinkListenerDisposable = _player.EventsCollector.OnDodgeToGuardLink.Subscribe(TryLinkToGuard);
            });
            _dodgeFinishListenerDisposable = _player.EventsCollector.OnDodgeEnd.Subscribe(OnDodgeEnd);
            _guardLinkListenerDisposable = _player.EventsCollector.OnDodgeToGuardLink.Subscribe(TryLinkToGuard);

            _guardListenerDisposable = _gameInputService.Guard.Subscribe(value => _guardHolded = value);

            _holdAttackListenersDisposables = new()
            {
                _gameInputService.OnAttackPressed.Where(v => v == true).Subscribe(_ => _holdAttackPending = true),
                _gameInputService.OnAttackCancel.Subscribe(_ => _holdAttackPending = false)
            };

            _player.Movement.IsControlledByRootMotion = true;
            _lockOnHandler.IsEnabled = false;

            SetDirection();
            _player.Animator.SetTrigger(_dodgeTrigger);
        }

        private void TryLinkToGuard(int dodgeType)
        {
            if (_isRoll && dodgeType != 1) return;
            if (!_guardHolded) return;

            DisposeOfListeners();

            _parentStateMachine.SetState<GuardState>();
        }

        public virtual void Exit()
        {
            DisposeOfListeners();

            _player.Movement.IsControlledByRootMotion = false;
            _lockOnHandler.IsEnabled = true;
        }

        private void DisposeOfListeners(bool includeGuardListenerAndHoldAttackListeners = true)
        {
            _dodgeInputListenerDisposable?.Dispose();
            _dodgeFinishListenerDisposable?.Dispose();
            _guardLinkListenerDisposable?.Dispose();

            if (includeGuardListenerAndHoldAttackListeners)
            {
                _guardListenerDisposable?.Dispose();
                _holdAttackListenersDisposables?.Dispose();
            }
        }

        private void OnDodgeEnd(int dodgeType)
        {
            if (_isRoll && dodgeType != 1) return;

            DisposeOfListeners();

            _player.Animator.SetFloat(_sidewardMoveFloat, 0f);
            _player.Animator.SetFloat(_forwardMoveFloat, 0f);

            if (_guardHolded)
            {
                _parentStateMachine.SetState<GuardState>();

                return;
            }

            if (_previousState is CalmMovementState) _parentStateMachine.SetState<CalmMovementState>();
            if (_previousState is BattleState)
            {
                if (_holdAttackPending) _parentStateMachine.SetState<BattleState, BattleState.EntryTag>(BattleState.EntryTag.HoldAttack);
                else _parentStateMachine.SetState<BattleState, BattleState.EntryTag>(BattleState.EntryTag.Movement);
            }
        }

        private void SetDirection()
        {
            var direction = _gameInputService.GetMovementInput();

            if (direction == Vector3.zero)
            {
                _player.Animator.SetFloat(_sidewardMoveFloat, 0f);
                _player.Animator.SetFloat(_forwardMoveFloat, -1f);

                return;
            }

            if (_lockOnHandler.CurrentEnemy != null)
            {
                var localDirection = _player.Movement.GetLocalLookDirection(direction);

                if (Mathf.Abs(localDirection.x) < 0.9f || Mathf.Abs(localDirection.x) < Mathf.Abs(localDirection.z))
                {
                    var deltaAngle = _player.Movement.GetDeltaAngleBetweenDirectionAndLookTarget(direction);

                    if (deltaAngle > -60f && deltaAngle <= 60f)
                    {
                        _player.Movement.SetRotationDirection(direction, 5000f, true);
                    }
                    else if (deltaAngle > 60f && deltaAngle <= 120f)
                    {
                        SetRotationDirectionByDefault(direction);
                    }
                    else if (deltaAngle < -60f && deltaAngle >= -120f)
                    {
                        SetRotationDirectionByDefault(direction);
                    }
                    else
                    {
                        _player.Movement.SetRotationDirection(-direction, 5000f, true);
                    }

                    localDirection.Normalize();

                    _player.Animator.SetFloat(_sidewardMoveFloat, 0f);
                    _player.Animator.SetFloat(_forwardMoveFloat, localDirection.z);
                }
                else
                {
                    localDirection.Normalize();
                    SetRotationDirectionByDefault(direction);

                    _player.Animator.SetFloat(_forwardMoveFloat, 0);
                    _player.Animator.SetFloat(_sidewardMoveFloat, localDirection.x);
                }

                return;
            }

            SetRotationDirectionByDefault(direction);
            _player.Animator.SetFloat(_sidewardMoveFloat, 0f);
            _player.Animator.SetFloat(_forwardMoveFloat, direction != Vector3.zero ? 1 : -1f);
        }

        private void SetRotationDirectionByDefault(Vector3 direction)
        {
            _player.Movement.SetRotationDirection(direction, _player.ParametersConfig.RotationSpeed);
        }
    }
}

