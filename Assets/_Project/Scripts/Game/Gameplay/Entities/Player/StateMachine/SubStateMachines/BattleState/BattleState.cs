using StateMachine;
using DI;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleState : IEnterableState<bool>
    {
        public enum ExitTag
        {
            Timer,
            Dodge
        }

        protected const string _battleStateBool = "IsBattleState";

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;

        private BattleStateMachine _battleStateMachine;

        private System.IDisposable _stateMachineExitListenerDisposable;

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

            _stateMachineExitListenerDisposable = _battleStateMachine.OnExit.Subscribe(exitTag =>
            {
                switch(exitTag)
                {
                    case ExitTag.Timer : _parentStateMachine.SetState<CalmState>(); return;
                    case ExitTag.Dodge : _parentStateMachine.SetState<DodgeState, IState>(this); return;

                    default : throw new System.ArgumentOutOfRangeException($"Unsupported exit tag: {exitTag}");
                }
            });

            if (startByAttack) _battleStateMachine.SetState<AttackState>();
            else _battleStateMachine.SetState<BattleMovementState>();
        }

        public void Exit()
        {       
            _stateMachineExitListenerDisposable?.Dispose();    
            _battleStateMachine?.Dispose();
        }
    }
}
