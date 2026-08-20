using CombatTower.Game.Configs;
using Random = UnityEngine.Random;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public static class DamageFactory
    {
        public static Damage Create(/*AttackData data*/)
        {
            //bool isCritical = Random.value >= 1 - data.CriticalChance;
            //bool isArmorBreak = Random.value >= 1 - data.ArmorBreakChance;

            var damage = new Damage()
            {
                BaseValue = /*data.Damage*/1,
                ResultValue = /*data.Damage*/1,
                Modifiers = new()
            };

            //if (isCritical) damage.Modifiers.Add(new CriticalDamageModifier());
            //if (isArmorBreak) damage.Modifiers.Add(new ArmorBreakDamageModifier());

            return damage;
        }
    }
}
