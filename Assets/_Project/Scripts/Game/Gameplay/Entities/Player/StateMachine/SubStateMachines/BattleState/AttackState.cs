using StateMachine;
using DI;
using R3;
using CombatTower.Game.Services;
using System;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class AttackState : IEnterableState
    {
        private const string _attackComboStartTrigger = "AttackComboStart";
        private const string _attackComboInt = "AttackCombo";

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        private GameInputService _gameInputService;
        private LockOnHandler _lockOnHandler;

        private IEnemyDetector _enemyDetector;

        private int _currentCombo;
        private bool _isChainable;

        private IDisposable _comboWindowListenerDisposable;
        private IDisposable _dodgeListenerDisposable;
        private IDisposable _guardListenerDisposable;
        private CompositeDisposable _attackEventsListenerDisposables;

        public AttackState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;
            _gameInputService = _sceneContainer.Resolve<GameInputService>();
            _lockOnHandler = _sceneContainer.Resolve<LockOnHandler>();
            
            _enemyDetector = new EnemyDetector(Root.LayerMasks.Enemy, _player.Rigidbody.transform, _player.ParametersConfig.CloseTargetDetectionRange);
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

            _currentCombo = 1;

            _player.Movement.IsControlledByRootMotion = true;
            _player.Movement.SetRotationDirection(GetDirection(Vector3.zero));
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
            _guardListenerDisposable?.Dispose();
        }

        private void OnAttackStarted(int comboNumber)
        {
            
        }

        private void OnAttackExecuted(int comboNumber)
        {
            StartListenToCombo();
            StartListenDodgeOrGuard();
        }

        private void OnAttackFinished(int comboNumber)
        {
            _attackEventsListenerDisposables?.Dispose();
            StopListenToCombo();
            StopListenDodgeAndGuard();

            _parentStateMachine.SetState<BattleMovementState>();
        }

        private void OnAttackPressed()
        {
            if (_isChainable)
            {
                StopListenToCombo();
                StopListenDodgeAndGuard();
                _currentCombo++;

                _player.Movement.SetRotationDirection(GetDirection(_gameInputService.GetMovementInput()), _player.ParametersConfig.RotationSpeed);
                //_player.Animator.SetTrigger(_attackComboStartTrigger + _currentCombo.ToString());
                _player.Animator.SetInteger(_attackComboInt, _currentCombo);
            }
        }

        private void StartListenToCombo()
        {
            _comboWindowListenerDisposable?.Dispose();

            if (_currentCombo == _player.ParametersConfig.MaxCombo) return;

            _isChainable = true;

            _comboWindowListenerDisposable = Observable.Timer(TimeSpan.FromMilliseconds(_player.ParametersConfig.ComboWindowMs)).Subscribe(_ => StopListenToCombo());
        }

        private void StopListenToCombo()
        {
            _comboWindowListenerDisposable?.Dispose();
            _isChainable = false;
        }

        private void StartListenDodgeOrGuard()
        {
            _dodgeListenerDisposable = _gameInputService.OnDodgePressed.Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<BattleExitState, BattleState.ExitTag>(BattleState.ExitTag.Dodge);

                return;
            });

            _guardListenerDisposable = _gameInputService.Guard.Where(v => v == true).Subscribe(_ =>
            {
                DisposeOfListeners();

                _parentStateMachine.SetState<BattleExitState, BattleState.ExitTag>(BattleState.ExitTag.Guard);

                return;
            });
        }

        private void StopListenDodgeAndGuard()
        {
            _dodgeListenerDisposable?.Dispose();
            _guardListenerDisposable?.Dispose();
        }

        private Vector3 GetDirection(Vector3 defaultDirection)
        {
            var direction = defaultDirection;

            if (_lockOnHandler.CurrentEnemy == null)
            {
                var enemyView = _enemyDetector.TryGetClosestEnemy();

                if (enemyView != null)
                {
                    var enemyPosition = enemyView.transform.position;
                    enemyPosition.y = 0;

                    var playerPosition = _player.Rigidbody.transform.position;
                    playerPosition.y = 0;

                    direction = (enemyPosition - playerPosition).normalized;
                }
            }

            return direction;
        }
    }
}
