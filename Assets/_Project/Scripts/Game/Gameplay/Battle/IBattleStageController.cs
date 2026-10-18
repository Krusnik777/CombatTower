using UnityEngine;
using Enemy = CombatTower.Game.Gameplay.Entities.Enemy.Enemy;

namespace CombatTower.Game.Gameplay.Battle
{
    public interface IBattleStageController
    {
        public bool TryGetPosition(Enemy enemy, out Transform moveTarget);
        public void RemoveEnemy(Enemy enemy);
    }
}
