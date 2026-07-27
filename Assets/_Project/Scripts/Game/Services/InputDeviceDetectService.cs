using System;
using R3;
using UnityEngine.InputSystem;

namespace UnityR3ProjectTemplate.Game.Services
{
    public class InputDeviceDetectService : IDisposable
    {
        private static ReactiveProperty<InputDevice> _currentDevice;
        public static Observable<InputDevice> CurrentControlDevice => _currentDevice;

        public void Dispose()
        {
            InputSystem.onEvent -= OnDeviceChange;
        }

        public InputDeviceDetectService()
        {
            _currentDevice = new();

            var devices = InputSystem.devices;
            if (devices.Count > 0)
            {
                _currentDevice.Value = devices[0];
            }

            InputSystem.onEvent += OnDeviceChange;
        }

        private void OnDeviceChange(UnityEngine.InputSystem.LowLevel.InputEventPtr eventPtr, InputDevice device)
        {
            if (_currentDevice.Value == device) return;

            _currentDevice.Value = device;
        }
    }
}
