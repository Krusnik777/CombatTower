using StateMachine;
using DI;
using R3;
using CombatTower.Game.Services;
using System;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class AttackState : IEnterableState
    {
        private const string _attackComboStartTrigger = "AttackComboStart";
        private const string _attackComboInt = "AttackCombo";
        private const int _maxCombo = 5;
        private const float _comboWindowMs = 200f;
        private const float _rotationSpeedInCombo = 10f; // temp

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        private GameInputService _gameInputService;

        private int _currentCombo;
        private bool _isChainable;

        private IDisposable _comboWindowListenerDisposable;
        private IDisposable _dodgeListenerDisposable;
        private CompositeDisposable _attackEventsListenerDisposables;

        public AttackState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;
            _gameInputService = _sceneContainer.Resolve<GameInputService>();
        }

        public void Enter()
        {
            DisposeOfListeners();
            
            _attackEventsListenerDisposables = new()
            {
                _player.EventsCollector.OnAttackStart.Subscribe(OnAttackStarted),
                _player.EventsCollector.OnAttackExecute.Subscribe(OnAttackExecuted),
                _player.EventsCollector.OnAttackFinish.Subscribe(OnAttackFinished),
                _gameInputService.OnAttackPressed.Subscribe(_ => OnAttackPressed())
            };

            _dodgeListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<BattleExitState, BattleState.ExitTag>(BattleState.ExitTag.Dodge);

                return;
            });

            _currentCombo = 1;

            _player.Movement.IsControlledByRootMotion = true;
            _player.Movement.SetRotationDirection(UnityEngine.Vector3.zero);
            _player.Animator.SetInteger(_attackComboInt, _currentCombo);
            _player.Animator.SetTrigger(_attackComboStartTrigger);
        }

        public void Exit()
        {
            _player.Movement.IsControlledByRootMotion = false;

            DisposeOfListeners();
        }

        private void DisposeOfListeners()
        {
            _attackEventsListenerDisposables?.Dispose();
            _comboWindowListenerDisposable?.Dispose();
            _dodgeListenerDisposable?.Dispose();
        }

        private void OnAttackStarted(int comboNumber)
        {
            
        }

        private void OnAttackExecuted(int comboNumber)
        {
            StartListenToCombo();
        }

        private void OnAttackFinished(int comboNumber)
        {
            _attackEventsListenerDisposables?.Dispose();
            StopListenToCombo();

            _parentStateMachine.SetState<BattleMovementState>();
        }

        private void OnAttackPressed()
        {
            if (_isChainable)
            {
                StopListenToCombo();
                _currentCombo++;

                _player.Movement.SetRotationDirection(_gameInputService.GetMovementInput(), _rotationSpeedInCombo);
                //_player.Animator.SetTrigger(_attackComboStartTrigger + _currentCombo.ToString());
                _player.Animator.SetInteger(_attackComboInt, _currentCombo);
            }
        }

        private void StartListenToCombo()
        {
            _comboWindowListenerDisposable?.Dispose();

            if (_currentCombo == _maxCombo) return;

            _isChainable = true;

            _comboWindowListenerDisposable = Observable.Timer(TimeSpan.FromMilliseconds(_comboWindowMs)).Subscribe(_ => StopListenToCombo());
        }

        private void StopListenToCombo()
        {
            _comboWindowListenerDisposable?.Dispose();
            _isChainable = false;
        }
    }
}
