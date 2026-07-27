using System;
using R3;
using UnityEngine.InputSystem;

namespace CombatTower.Game.Services
{
    public class UIInputController : IDisposable
    {
        public Subject<Unit> OnSubmitPressed { get; private set; } = new();
        public Subject<Unit> OnCancelPressed { get; private set; } = new();

        private GameInput _gameInput;

        public UIInputController(GameInput gameInput)
        {
            _gameInput = gameInput;
            //_gameInput.UI.Enable();
            _gameInput.UIGamepadButtons.Enable();

            _gameInput.UIGamepadButtons.Submit.performed += OnSubmit;
            _gameInput.UIGamepadButtons.Cancel.performed += OnCancel;
        }

        public void Dispose()
        {
            _gameInput.UIGamepadButtons.Submit.performed -= OnSubmit;
            _gameInput.UIGamepadButtons.Cancel.performed -= OnCancel;
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            OnSubmitPressed?.OnNext(Unit.Default);
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            OnCancelPressed?.OnNext(Unit.Default);
        }
    }
}
