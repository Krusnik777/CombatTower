using DI;
using StateMachine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerStateMachine : AbstractStateMachine
    {
        public PlayerStateMachine(Player player, DIContainer sceneContainer)
        {
            _states = new()
            {
                [typeof(MovementState)] = new MovementState(this, player, sceneContainer),
                [typeof(CalmState)] = new CalmState(this, player, sceneContainer),
                [typeof(BattleState)] = new BattleState(this, player, sceneContainer),
                [typeof(DodgeState)] = new DodgeState(this, player, sceneContainer),
                [typeof(DeathState)] = new DeathState(this, player, sceneContainer)
            };
        }
    }
}
