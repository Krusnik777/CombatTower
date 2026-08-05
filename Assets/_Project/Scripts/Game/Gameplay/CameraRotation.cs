using CombatTower.Game.Services;
using Unity.Cinemachine;
using UnityEngine;

namespace CombatTower.Game.Gameplay
{
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow m_orbitalFollow;
        [Header("Sensitivity Settings")] // TEMP? - to configs?
        [SerializeField] private Vector2 m_baseSensitivity = new Vector2(10f, 0.02f);
        [SerializeField] private float m_smoothKoef = 10f;
        [SerializeField, Range(0f, 1f)] private float m_decayRate = 0.95f;

        private GameInputService _gameInputService;

        private Vector3 _velocity;

        public void Bind(GameInputService gameInputService) => _gameInputService = gameInputService;

        private void Update()
        {
            if (_gameInputService == null) return;

            float x = Mathf.Clamp(_gameInputService.GetCameraRotationAxis().x, -1f, 1f)
                    * m_baseSensitivity.x * m_smoothKoef;
            float y = Mathf.Clamp(_gameInputService.GetCameraRotationAxis().y, -1f, 1f)
                * m_baseSensitivity.y * m_smoothKoef;

            _velocity = new Vector3(x, -y, 0f);

            m_orbitalFollow.HorizontalAxis.Value += _velocity.x * Time.deltaTime;

            _velocity *= m_decayRate;
        }
    }
}
