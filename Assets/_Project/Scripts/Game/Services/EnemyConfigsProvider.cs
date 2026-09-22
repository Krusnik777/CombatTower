using CombatTower.Game.Configs;
using CombatTower.Game.Gameplay.Entities.Enemy;
using UnityEngine;

namespace CombatTower.Game.Services
{
    public class EnemyConfigsProvider
    {
        private const string _configsPath = "Settings/Enemies/";

        public EnemyConfig GetConfig(EnemyType enemyType)
        {
            switch(enemyType)
            {
                case EnemyType.SkeletonWarrior : return Resources.Load<EnemyConfig>(_configsPath + "SkeletonWarrior");
            }

            throw new System.NullReferenceException("[Enemy Configs Provider] Couldn't find enemy config for : " + enemyType);
        }
    }
}
