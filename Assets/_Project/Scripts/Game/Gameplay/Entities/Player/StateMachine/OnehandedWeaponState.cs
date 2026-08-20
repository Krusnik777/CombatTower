using StateMachine;
using DI;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class OnehandedWeaponState : ChosenWeaponState
    {
        protected override int _weaponLayerIndex => 0;

        public OnehandedWeaponState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }
    }
}
