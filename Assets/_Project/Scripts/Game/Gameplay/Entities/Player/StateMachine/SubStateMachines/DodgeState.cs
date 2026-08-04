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

        private IState _previousState;

        private Vector3 _headingDirection;

        private IDisposable _dodgeInputListenerDisposable;
        private IDisposable _dodgeFinishListenerDisposable;

        public DodgeState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;

            _gameInputService = _sceneContainer.Resolve<GameInputService>();
        }

        public virtual void Enter(IState previousState)
        {
            DisposeOfListeners();

            _previousState = previousState;

            _dodgeInputListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ =>
            {
                DisposeOfListeners();

                var newDirection = _gameInputService.GetMovementInput();
                _player.Movement.SetRotationDirection(newDirection, 500f);

                _player.Animator.SetTrigger(_rollTrigger);
                _player.Animator.SetFloat(_forwardMoveFloat, newDirection != Vector3.zero ? 1 : -1f);

                _dodgeFinishListenerDisposable = _player.EventsCollector.OnDodgeEnd.Subscribe(_ => OnDodgeEnd());
            });
            _dodgeFinishListenerDisposable = _player.EventsCollector.OnDodgeEnd.Subscribe(_ => OnDodgeEnd());

            _headingDirection = _gameInputService.GetMovementInput();

            _player.Movement.IsControlledByRootMotion = true;

            _player.Movement.SetRotationDirection(_headingDirection, 500f);
            _player.Animator.SetTrigger(_dodgeTrigger);
            _player.Animator.SetFloat(_forwardMoveFloat, _headingDirection != Vector3.zero ? 1 : -1f);
        }
        public virtual void Exit()
        {
            DisposeOfListeners();

            _player.Movement.IsControlledByRootMotion = false;
        }

        private void DisposeOfListeners()
        {
            _dodgeInputListenerDisposable?.Dispose();
            _dodgeFinishListenerDisposable?.Dispose();
        }

        private void OnDodgeEnd()
        {
            DisposeOfListeners();

            if (_previousState is CalmState) _parentStateMachine.SetState<CalmState>();
            if (_previousState is BattleState) _parentStateMachine.SetState<BattleState, bool>(false);
        }
    }
}

