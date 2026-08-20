using UnityEngine;

namespace CombatTower.Game.Gameplay.HealthSystem
{
    public interface IDamageProcessor
    {
        public void Process(ref Damage damage);
    }
}
