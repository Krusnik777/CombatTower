using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CombatTower.Game.Services
{
    public class GameInputService : IDisposable
    {
        public Subject<Unit> OnAttackPressed { get; private set; } = new();

        public Subject<Unit> OnDodgePressed { get; private set; } = new();
        public Subject<Unit> OnLockOnPressed { get; private set; } = new();

        public Subject<int> OnLockOnTargetSwitchPressed { get; private set; } = new();

        public ReadOnlyReactiveProperty<bool> Guard => _guard;
        private ReactiveProperty<bool> _guard;

        private GameInput _gameInput;
        public InputActionAsset ActionsAsset => _gameInput.asset;

        private UIInputController _uiInputController;
        public UIInputController UIInputController => _uiInputController;

        private IDisposable _anyButtonPressListenerDisposable;

        public GameInputService()
        {
            _gameInput = new();
            _gameInput.Enable();

            _guard = new(false);

            _uiInputController = new(_gameInput);

            _gameInput.Player.Attack.performed += OnAttack;
            _gameInput.Player.Dodge.performed += OnDodge;

            _gameInput.Player.Guard.started += OnGuardStart;
            _gameInput.Player.Guard.canceled += OnGuardEnd;

            _gameInput.Player.LockOn.performed += OnLockOn;
            _gameInput.Player.SwitchLockOnTargetToLeft.performed += OnSwitchTargetToLeft;
            _gameInput.Player.SwitchLockOnTargetToRight.performed += OnSwitchTargetToRight;
        }

        public void Dispose()
        {
            _anyButtonPressListenerDisposable?.Dispose();
            _uiInputController?.Dispose();

            _gameInput.Player.Attack.performed -= OnAttack;
            _gameInput.Player.Dodge.performed -= OnDodge;

            _gameInput.Player.LockOn.performed -= OnLockOn;
            _gameInput.Player.SwitchLockOnTargetToLeft.performed -= OnSwitchTargetToLeft;
            _gameInput.Player.SwitchLockOnTargetToRight.performed -= OnSwitchTargetToRight;
        }

        public Vector3 GetMovementInput(bool isInverse = false)
        {
            var input = _gameInput.Player.Move.ReadValue<Vector2>();
            if (isInverse) input *= -1f;

            if (Camera.main != null)
            {
                var cameraTransform = Camera.main.transform;

                var forward = cameraTransform.forward;
                forward.y = 0;
                forward.Normalize();

                var right = cameraTransform.right;
                right.y = 0;
                right.Normalize();

                return forward * input.y + right * input.x;
            }

            return new Vector3(input.x, 0, input.y);
        }

        public Vector2 GetCameraRotationAxis()
        {
            var input = _gameInput.Player.Look.ReadValue<Vector2>();

            return input;
        }

        public void ClearReactionForAnyButtonPress() => _anyButtonPressListenerDisposable?.Dispose();

        public void SetReactionForAnyButtonPress(Action action)
        {
            _anyButtonPressListenerDisposable?.Dispose();

            _anyButtonPressListenerDisposable = InputSystem.onAnyButtonPress.Call(_ =>
            {
                _anyButtonPressListenerDisposable?.Dispose();

                action?.Invoke();
            });
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            OnAttackPressed?.OnNext(Unit.Default);
        }

        private void OnDodge(InputAction.CallbackContext context)
        {
            OnDodgePressed?.OnNext(Unit.Default);
        }

        private void OnGuardStart(InputAction.CallbackContext context)
        {
            _guard.Value = true;
        }

        private void OnGuardEnd(InputAction.CallbackContext context)
        {
            _guard.Value = false;
        }

        private void OnLockOn(InputAction.CallbackContext context)
        {
            OnLockOnPressed?.OnNext(Unit.Default);
        }

        private void OnSwitchTargetToLeft(InputAction.CallbackContext context) => OnSwitchTarget(-1);
        private void OnSwitchTargetToRight(InputAction.CallbackContext context) => OnSwitchTarget(1);
        private void OnSwitchTarget(int direction) => OnLockOnTargetSwitchPressed?.OnNext(direction);
    }
}
