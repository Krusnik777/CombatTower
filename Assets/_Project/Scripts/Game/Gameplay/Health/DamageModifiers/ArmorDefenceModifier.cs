namespace CombatTower.Game.Gameplay.HealthSystem
{
    public class ArmorDefenceModifier : IDamageModifier
    {
        private int _armor;

        public ArmorDefenceModifier(int armor = 10)
        {
            _armor = armor;
        }

        public void Modify(ref Damage damage)
        {
            if (damage.Type == DamageType.ArmorBreak) return;

            damage.ResultValue -= _armor;
        }
    }
}
