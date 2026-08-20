namespace CombatTower.Game.Gameplay.HealthSystem
{
    public class CriticalDamageModifier : IDamageModifier
    {
        private float _boosterValue;

        public CriticalDamageModifier(float boosterValue = 1.5f)
        {
            _boosterValue = boosterValue;
        }

        public void Modify(ref Damage damage)
        {
            damage.IsCritical = true;
            damage.ResultValue = (int)(damage.BaseValue * _boosterValue);
        }
    }
}
