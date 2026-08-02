using StateMachine;
using DI;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class DefenseState : IEnterableState
    {
        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        //private Animator _animator;

        public DefenseState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _sceneContainer = sceneContainer;

            //_animator = _sceneContainer.Resolve<PlayerView>().Animator;
        }

        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}
