using StateMachine;
using DI;
using R3;
using CombatTower.Game.Services;
using System;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class AttackState : IEnterableState<bool>
    {
        private const string _simpleAttackTrigger = "SimpleAttack";
        private const string _holdAttackTrigger = "HoldAttack";
        private const string _attackComboInt = "AttackCombo";

        private const float _simpleAttackRange = 1f; // TEMP
        private const float _holdAttackRange = 1.75f; // TEMP

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        private GameInputService _gameInputService;
        private LockOnHandler _lockOnHandler;

        private IEnemyDetector _enemyDetector;
        private IDamageDealer _damageDealer;

        private int _currentCombo;
        private bool _isChainable;
        
        private bool _isHoldAttack;
        private bool _isHoldAttackPending;

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

        public void Enter(bool isHoldAttack)
        {
            DisposeOfListeners();

            _attackEventsListenerDisposables = new()
            {
                _player.EventsCollector.OnAttackStart.Subscribe(OnAttackStarted),
                _player.EventsCollector.OnAttackExecute.Subscribe(OnAttackExecuted),
                _player.EventsCollector.OnAttackFinish.Subscribe(OnAttackFinished),
                _gameInputService.OnAttackPressed.Subscribe(OnAttackPressed)
            };

            CreateDamageDealer(isHoldAttack);

            _currentCombo = 1;
            _isHoldAttack = isHoldAttack;
            _isHoldAttackPending = false;

            _player.Movement.IsControlledByRootMotion = true;
            _player.Movement.SetRotationDirection(GetDirection(Vector3.zero));
            _player.Animator.SetInteger(_attackComboInt, _currentCombo);
            _player.Animator.SetTrigger(isHoldAttack ? _holdAttackTrigger : _simpleAttackTrigger);

            _player.IsNotStaggeredByDamage = _isHoldAttack;
        }

        public void Exit()
        {
            _player.Movement.IsControlledByRootMotion = false;
            _player.IsNotStaggeredByDamage = false;

            DisposeOfListeners();
        }

        private void DisposeOfListeners()
        {
            _attackEventsListenerDisposables?.Dispose();
            _comboWindowListenerDisposable?.Dispose();
            _dodgeListenerDisposable?.Dispose();
            _guardListenerDisposable?.Dispose();
            _damageDealer?.Dispose();
        }

        private void CreateDamageDealer(bool isHoldAttack)
        {
            _damageDealer?.Dispose();
            _damageDealer = new WeaponDamageDealer(Root.LayerMasks.Enemy, 
                                                   _player.Rigidbody.transform, 
                                                   isHoldAttack ? _holdAttackRange : _simpleAttackRange, 
                                                   _player.EventsCollector, 
                                                   _player.WeaponHolderTransform);
        }

        private void OnAttackStarted(int comboNumber)
        {

        }

        private void OnAttackExecuted(int comboNumber)
        {
            if (_isHoldAttack) return;

            if (_isHoldAttackPending)
            {
                HandlePendingHoldAttack();
                return;
            }

            StartListenToCombo();
            StartListenDodgeOrGuard();
        }
        
        private void OnAttackFinished(int comboNumber)
        {
            if (_isHoldAttackPending)
            {
                HandlePendingHoldAttack();
                return;
            }

            _attackEventsListenerDisposables?.Dispose();
            StopListenToCombo();
            StopListenDodgeAndGuard();

            _parentStateMachine.SetState<BattleMovementState>();
        }

        private void OnAttackPressed(bool isHoldAttack)
        {
            if (_isHoldAttack || _isHoldAttackPending) return;

            if (isHoldAttack)
            {
                _isHoldAttackPending = true;
            }
            else
            {
                if (_isChainable) HandleNextAttack();
            }
        }

        private void HandleNextAttack(bool isHoldAttack = false)
        {
            StopListenToCombo();
            StopListenDodgeAndGuard();
            _currentCombo++;
            if (isHoldAttack && _currentCombo >= _player.ParametersConfig.MaxCombo) _currentCombo = _player.ParametersConfig.MaxCombo;

            CreateDamageDealer(isHoldAttack);

            _player.Movement.SetRotationDirection(GetDirection(_gameInputService.GetMovementInput()), _player.ParametersConfig.RotationSpeed);
            _player.Animator.SetInteger(_attackComboInt, _currentCombo);
            _player.Animator.SetTrigger(isHoldAttack ? _holdAttackTrigger : _simpleAttackTrigger);

            _player.IsNotStaggeredByDamage = _isHoldAttack;
        }

        private void HandlePendingHoldAttack()
        {
            _isHoldAttack = true;
            _isHoldAttackPending = false;

            HandleNextAttack(true);
        }

        private void StartListenToCombo()
        {
            _comboWindowListenerDisposable?.Dispose();

            if (_currentCombo >= _player.ParametersConfig.MaxCombo) return;

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
