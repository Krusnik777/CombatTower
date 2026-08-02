using DI;
using StateMachine;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class PlayerWeaponStateMachine : AbstractStateMachine
    {
        public PlayerWeaponStateMachine(Player player, DIContainer sceneContainer)
        {
            _states = new()
            {
                [typeof(OnehandedWeaponState)] = new OnehandedWeaponState(this, player, sceneContainer)
            };

            SetState<OnehandedWeaponState>();
        }
    }
}
