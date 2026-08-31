using CombatTower.Game.Settings;

namespace CombatTower.Game.Gameplay.Battle
{
    public class BattleStageController : IBattleStageController
    {
        private readonly int _maxGridCapacity;
        private readonly int _maxAttackCapacity;

        private int _gridCapacity;
        private int _attackCapacity;

        private BattleGrid _battleGrid;

        public BattleStageController(GameSettings gameSettings, BattleGridView battleGridView)
        {
            _maxGridCapacity = gameSettings.GridCapacity;
            _maxAttackCapacity = gameSettings.AttackCapacity;

            _gridCapacity = _maxGridCapacity;
            _attackCapacity = _maxAttackCapacity;

            _battleGrid = new(battleGridView);
        }

    }
}
