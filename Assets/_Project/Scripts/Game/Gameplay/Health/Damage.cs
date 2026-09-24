using System.Collections.Generic;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public enum DamageType
    {
        Simple,
        ArmorBreak
    }

    public enum DamageDirection
    {
        Front,
        Back
    }

    public struct Damage
    {
        public int BaseValue;
        public int ResultValue;
        public DamageType Type;
        public DamageDirection Direction;
        public bool IsCritical;
        public List<IDamageModifier> Modifiers;
    }
}
