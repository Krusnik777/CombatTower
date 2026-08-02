using StateMachine;
using DI;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleState : IEnterableState<bool>
    {
        protected const string _battleStateBool = "IsBattleState";

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;

        private BattleStateMachine _battleStateMachine;

        public BattleState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;
        }

        public void Enter(bool startByAttack)
        {
            _player.Animator.SetBool(_battleStateBool, true);
            
            _battleStateMachine?.Dispose();
            _battleStateMachine = new(_player, _parentStateMachine, _sceneContainer);
            if (startByAttack) _battleStateMachine.SetState<AttackState>();
            else _battleStateMachine.SetState<BattleMovementState>();
        }

        public void Exit()
        {           
            _battleStateMachine?.Dispose();
        }
    }
}
