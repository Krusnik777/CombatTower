using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CombatTower.Game.Services
{
    public class GameInputService : IDisposable
    {
        public Subject<Unit> OnAbilityAPressed { get; private set; } = new();
        public Subject<Unit> OnAbilityBPressed { get; private set; } = new();
        public Subject<Unit> OnAbilityXPressed { get; private set; } = new();
        public Subject<Unit> OnAbilityYPressed { get; private set; } = new();
        public Subject<Unit> OnLockOnPressed { get; private set; } = new();

        private GameInput _gameInput;
        public InputActionAsset ActionsAsset => _gameInput.asset;

        private UIInputController _uiInputController;
        public UIInputController UIInputController => _uiInputController;

        private IDisposable _anyButtonPressListenerDisposable;

        public GameInputService()
        {
            _gameInput = new();
            _gameInput.Enable();

            _uiInputController = new(_gameInput);

            _gameInput.Player.AbilityA.performed += OnAbilityA;
            _gameInput.Player.AbilityB.performed += OnAbilityB;
            _gameInput.Player.AbilityX.performed += OnAbilityX;
            _gameInput.Player.AbilityY.performed += OnAbilityY;

            _gameInput.Player.LockOn.performed += OnLockOn;
        }

        public void Dispose()
        {
            _anyButtonPressListenerDisposable?.Dispose();
            _uiInputController?.Dispose();

            _gameInput.Player.AbilityA.performed -= OnAbilityA;
            _gameInput.Player.AbilityB.performed -= OnAbilityB;
            _gameInput.Player.AbilityX.performed -= OnAbilityX;
            _gameInput.Player.AbilityY.performed -= OnAbilityY;

            _gameInput.Player.LockOn.performed -= OnLockOn;
        }

        public Vector3 GetMovementInput(bool isInverse = true)
        {
            var input = _gameInput.Player.Move.ReadValue<Vector2>();
            if (isInverse) input *= -1f;

            return new Vector3(input.x, 0, input.y);
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

        private void OnAbilityA(InputAction.CallbackContext context)
        {
            OnAbilityAPressed?.OnNext(Unit.Default);
        }

        private void OnAbilityB(InputAction.CallbackContext context)
        {
            OnAbilityBPressed?.OnNext(Unit.Default);
        }

        private void OnAbilityX(InputAction.CallbackContext context)
        {
            OnAbilityXPressed?.OnNext(Unit.Default);
        }

        private void OnAbilityY(InputAction.CallbackContext context)
        {
            OnAbilityYPressed?.OnNext(Unit.Default);
        }

        private void OnLockOn(InputAction.CallbackContext context)
        {
            OnLockOnPressed?.OnNext(Unit.Default);
        }
    }
}
