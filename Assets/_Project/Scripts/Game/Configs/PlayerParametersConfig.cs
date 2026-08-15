using UnityEngine;

namespace CombatTower.Game.Configs
{
    [CreateAssetMenu(fileName = "PlayerParametersConfig", menuName = "Scriptable Objects/Player/Player Parameters Config")]
    public class PlayerParametersConfig : ScriptableObject, ILockOnParameters
    {
        [field: Header("Basic Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 200f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;
        [field: Header("Battle")]
        [field: SerializeField] public float BattleStateExitTime { get; private set; } = 10f;
        [field: Header("Guard")]
        [field: SerializeField] public float MovementSpeedInGuard { get; private set; } = 100f;
        [field: SerializeField] public float RotationSpeedInGuard { get; private set; } = 5f;
        [field: Header("Dodge")]
        [field: SerializeField] public float DodgeInvulnerabilityWindowMs { get; private set; } = 200f;
        [field: Header("Attack")]
        [field: SerializeField] public int MaxCombo { get; private set; } = 5;
        [field: SerializeField] public float ComboWindowMs { get; private set; } = 200f;
        [field: SerializeField] public float CloseTargetDetectionRange { get; private set; } = 3f;
        [field: Header("Lock-On")]
        [field: SerializeField] public float LockOnDetectionRange { get; private set; } = 8f;
        [field: SerializeField] public float CooldownAfterLockOnSwitchTargetMs { get; private set; } = 250f;
    }
}
