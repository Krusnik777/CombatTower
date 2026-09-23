using UnityEngine;

namespace CombatTower.Game.Settings
{
    [System.Serializable]
    public class CameraSettings
    {
        public Vector2 Sensitivity = new Vector2(10f, 0.02f);
        public float SmoothKoef = 10f;
        [field: Range(0f, 1f)] public float DecayRate = 0.95f;
    }

    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [field: Header("Battle Grid")]
        [field: SerializeField] public int GridCapacity { get; private set; } = 12;
        [field: SerializeField] public int AttackCapacity { get; private set; } = 10;
        [field: Header("Camera Settings")]
        [field: SerializeField] public CameraSettings GamepadCameraSettings { get; private set; }
        [field: SerializeField] public CameraSettings MouseCameraSettings { get; private set; }

        public (CameraSettings, CameraSettings) GetCameraSettings() => (GamepadCameraSettings, MouseCameraSettings);
    }
}
