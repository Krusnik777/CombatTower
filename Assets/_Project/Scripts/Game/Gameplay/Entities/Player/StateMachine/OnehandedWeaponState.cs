using StateMachine;
using DI;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class OnehandedWeaponState : IEnterableState
    {
        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;

        private PlayerStateMachine _playerStateMachine;

        public OnehandedWeaponState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _sceneContainer = sceneContainer;

            _player = player;
            
            _playerStateMachine = new(_player, _sceneContainer);
        }

        public void Enter()
        {
            _player.Animator.SetLayerWeight(0, 1f);

            //_playerStateMachine?.Dispose();
            _playerStateMachine.SetState<CalmState>(); // TEMP
        }

        public void Exit()
        {
            _playerStateMachine?.Dispose();
        }
    }
}
