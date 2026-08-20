using UnityEngine;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public interface IDamageModifier
    {
        public void Modify(ref Damage damage);
    }
}
