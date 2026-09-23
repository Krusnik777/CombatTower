using CombatTower.Game.Services;
using CombatTower.Game.Settings;
using Unity.Cinemachine;
using UnityEngine;

namespace CombatTower.Game.Gameplay
{
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow m_orbitalFollow;

        private GameInputService _gameInputService;
        private (CameraSettings, CameraSettings) _settings;

        private Vector3 _velocity;

        public void Bind(GameInputService gameInputService, (CameraSettings, CameraSettings) settings)
        {
            _settings = settings;
            _gameInputService = gameInputService;
        }

        public void UnbindInput()
        {
            _gameInputService = null;
        }

        private void Update()
        {
            if (_gameInputService == null) return;

            var rotationAxis = _gameInputService.GetCameraRotationAxis();
            var isGamepad = InputDeviceDetectService.CurrentControlDevice is UnityEngine.InputSystem.Gamepad;
            var currentCameraSettings = isGamepad ? _settings.Item1 : _settings.Item2;

            float x = Mathf.Clamp(rotationAxis.x, -1f, 1f)
                    * currentCameraSettings.Sensitivity.x * currentCameraSettings.SmoothKoef;
            float y = Mathf.Clamp(rotationAxis.y, -1f, 1f)
                * currentCameraSettings.Sensitivity.y * currentCameraSettings.SmoothKoef;

            _velocity = new Vector3(x, -y, 0f);

            m_orbitalFollow.HorizontalAxis.Value += _velocity.x * Time.deltaTime;

            _velocity *= currentCameraSettings.DecayRate;
        }
    }
}
