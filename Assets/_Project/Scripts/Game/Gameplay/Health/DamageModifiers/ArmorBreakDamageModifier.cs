namespace CombatTower.Game.Gameplay.HealthSystem
{
    public class ArmorBreakDamageModifier : IDamageModifier
    {
        public void Modify(ref Damage damage)
        {
            damage.Type = DamageType.ArmorBreak;
        }
    }
}
