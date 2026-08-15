namespace CombatTower.Game.Configs
{
    public interface ILockOnParameters
    {
        public float LockOnDetectionRange { get; }
        public float CooldownAfterLockOnSwitchTargetMs { get; }
    }
}
