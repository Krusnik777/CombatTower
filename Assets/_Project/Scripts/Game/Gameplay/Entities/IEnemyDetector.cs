using CombatTower.Game.Gameplay.Entities.Enemy;

namespace CombatTower.Game.Gameplay.Entities
{
    public interface IEnemyDetector : System.IDisposable
    {
        public EnemyView TryGetClosestEnemy(EnemyView currentDetectedEnemy = null);
    }
}
