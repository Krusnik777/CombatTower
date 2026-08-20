using CombatTower.Game.Gameplay.HealthSystem;

namespace CombatTower.Game.Gameplay.Entities
{
    public interface IDamageable
    {
        public void Hit(Damage damage);
    }
}
