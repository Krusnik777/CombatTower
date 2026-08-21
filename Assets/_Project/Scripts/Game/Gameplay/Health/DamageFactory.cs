using Random = UnityEngine.Random;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public static class DamageFactory
    {
        public static Damage Create(/*AttackData data*/)
        {
            //bool isCritical = Random.value >= 1 - data.CriticalChance;
            //bool isArmorBreak = Random.value >= 1 - data.ArmorBreakChance;

            var damageValue = Random.Range(1, 5); // Temp

            var damage = new Damage()
            {
                BaseValue = /*data.Damage*/damageValue,
                ResultValue = /*data.Damage*/damageValue,
                Modifiers = new()
            };

            //if (isCritical) damage.Modifiers.Add(new CriticalDamageModifier());
            //if (isArmorBreak) damage.Modifiers.Add(new ArmorBreakDamageModifier());

            return damage;
        }
    }
}
