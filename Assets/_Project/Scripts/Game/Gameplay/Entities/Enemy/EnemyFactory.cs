using CombatTower.Game.Services;
using DI;
using UnityEngine;

namespace CombatTower.Game.Gameplay.Entities.Enemy
{
    public class EnemyFactory
    {
        private const string _prefabsPath = "Prefabs/Gameplay/Enemies/";

        private DIContainer _sceneContainer;
        private EnemyConfigsProvider _configsProvider;

        public EnemyFactory(DIContainer sceneContainer)
        {
            _sceneContainer = sceneContainer;
            _configsProvider = sceneContainer.Resolve<EnemyConfigsProvider>();
        }

        public Enemy CreateEnemy(EnemySpawnPoint spawnPoint)
        {
            var config = _configsProvider.GetConfig(spawnPoint.SpawnedEnemy);
            var enemyPrefabPath = _prefabsPath + config.PrefabName;
            var enemyViewPrefab = Resources.Load<EnemyView>(enemyPrefabPath);

            if (enemyViewPrefab == null) throw new System.NullReferenceException(
                    $"[Enemy Factory] Couldn't find prefab by name {config.PrefabName} in path {_prefabsPath} for config id {config.ID}");

            var enemyView = GameObject.Instantiate(enemyViewPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            var enemy = new Enemy(config, enemyView, _sceneContainer, spawnPoint.PatrolRoute);

            return enemy;
        }
    }
}
