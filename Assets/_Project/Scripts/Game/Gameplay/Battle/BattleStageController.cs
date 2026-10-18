using System.Collections.Generic;
using System.Linq;
using CombatTower.Game.Settings;
using UnityEngine;
using Enemy = CombatTower.Game.Gameplay.Entities.Enemy.Enemy;

namespace CombatTower.Game.Gameplay.Battle
{
    public class BattleStageController : IBattleStageController
    {
        private readonly int _maxGridCapacity;
        private readonly int _maxAttackCapacity;

        private int _gridCapacity;
        private int _attackCapacity;

        private Dictionary<Enemy, GridSlot> _closeEnemiesMap;
        private Dictionary<Enemy, GridSlot> _farEnemiesMap;

        private BattleGrid _battleGrid;

        public BattleStageController(GameSettings gameSettings, BattleGridView battleGridView)
        {
            _maxGridCapacity = gameSettings.GridCapacity;
            _maxAttackCapacity = gameSettings.AttackCapacity;

            _gridCapacity = _maxGridCapacity;
            _attackCapacity = _maxAttackCapacity;

            _battleGrid = new(battleGridView);

            _closeEnemiesMap = new();
            _farEnemiesMap = new();
        }

        public bool TryGetPosition(Enemy enemy, out Transform moveTarget)
        {
            moveTarget = null;

            if (_closeEnemiesMap.ContainsKey(enemy))
            {
                moveTarget = _closeEnemiesMap[enemy].ClosePoint;
                return true;
            }

            if (_gridCapacity < enemy.Config.GridWeight) return false;

            for (int i = 0; i < _battleGrid.Slots.Count; i++)
            {
                var slot = _battleGrid.Slots[i];

                if (_closeEnemiesMap.ContainsValue(slot)) continue;

                moveTarget = slot.ClosePoint;
                _closeEnemiesMap.Add(enemy, slot);
                _gridCapacity -= enemy.Config.GridWeight;

                return true;
            }

            return TryGetFarPosition(enemy, out moveTarget);
        }

        public bool TryGetFarPosition(Enemy enemy, out Transform moveTarget)
        {
            moveTarget = null;

            if (_farEnemiesMap.ContainsKey(enemy))
            {
                moveTarget = _farEnemiesMap[enemy].ClosePoint;
                return true;
            }

            for (int i = 0; i < _battleGrid.Slots.Count; i++)
            {
                var slot = _battleGrid.Slots[i];

                if (_farEnemiesMap.ContainsValue(slot)) continue;

                moveTarget = slot.ClosePoint;
                _farEnemiesMap.Add(enemy, slot);

                return true;
            }

            return false;
        }

        public void RemoveEnemy(Enemy enemy)
        {
            if (TryRemoveFromMap(enemy, ref _closeEnemiesMap))
            {
                _gridCapacity += enemy.Config.GridWeight;
            }
            else
            {
                TryRemoveFromMap(enemy, ref _farEnemiesMap);
            }
        }

        private bool TryRemoveFromMap(Enemy enemy, ref Dictionary<Enemy, GridSlot> enemiesMap)
        {
            if (enemiesMap.ContainsKey(enemy))
            {
                enemiesMap.Remove(enemy);

                return true;
            }

            return false;
        }
    }
}
