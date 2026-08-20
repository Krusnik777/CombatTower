using System.Collections.Generic;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public enum DamageType
    {
        Simple,
        ArmorBreak
    }

    public struct Damage
    {
        public int BaseValue;
        public int ResultValue;
        public DamageType Type;
        public bool IsCritical;
        public List<IDamageModifier> Modifiers;
    }
}
