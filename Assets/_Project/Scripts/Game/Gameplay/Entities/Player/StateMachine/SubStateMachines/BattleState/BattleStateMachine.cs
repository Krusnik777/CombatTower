using StateMachine;
using DI;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleStateMachine : AbstractStateMachine
    {
        public BattleStateMachine(Player player, IStateMachine playerStateMachine, DIContainer sceneContainer)
        {
            _states = new()
            {
                [typeof(BattleMovementState)] = new BattleMovementState(this, player, playerStateMachine, sceneContainer),
                [typeof(AttackState)] = new AttackState(this, player, sceneContainer),
                [typeof(DefenseState)] = new DefenseState(this, player, sceneContainer),
            };
        }
    }
}
