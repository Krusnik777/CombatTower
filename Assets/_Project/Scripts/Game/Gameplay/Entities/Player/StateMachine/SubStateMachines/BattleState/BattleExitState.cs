using StateMachine;
using DI;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleExitState : IEnterableState<BattleState.ExitTag>
    {
        public Subject<BattleState.ExitTag> OnExitSignal { get; private set; } = new();

        protected const string _battleStateBool = "IsBattleState";

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;

        public BattleExitState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;
        }

        public void Enter(BattleState.ExitTag exitTag)
        {
            OnExitSignal?.OnNext(exitTag);
        }

        public void Exit()
        {           
            
        }
    }
}
