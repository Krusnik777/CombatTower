using StateMachine;
using DI;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleState : IEnterableState<BattleState.EntryTag>
    {
        public enum EntryTag
        {
            SimpleAttack,
            HoldAttack,
            ChangeWeapon,
            Movement
        }

        public enum ExitTag
        {
            Timer,
            Dodge,
            Guard
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

        public void Enter(EntryTag entryTag)
        {
            _player.SetWeaponActive(true);
            _player.Animator.SetBool(_battleStateBool, true);
            
            _battleStateMachine?.Dispose();
            _battleStateMachine = new(_player, _parentStateMachine, _sceneContainer);

            _stateMachineExitListenerDisposable = _battleStateMachine.OnExit.Subscribe(exitTag =>
            {
                switch(exitTag)
                {
                    case ExitTag.Timer : _parentStateMachine.SetState<CalmMovementState>(); return;
                    case ExitTag.Dodge : _parentStateMachine.SetState<DodgeState, IState>(this); return;
                    case ExitTag.Guard : _parentStateMachine.SetState<GuardState>(); return;

                    default : throw new System.ArgumentOutOfRangeException($"Unsupported exit tag: {exitTag}");
                }
            });

            switch(entryTag)
            {
                case EntryTag.SimpleAttack : _battleStateMachine.SetState<AttackState, bool>(false); break;
                case EntryTag.HoldAttack : _battleStateMachine.SetState<AttackState, bool>(true); break;
                case EntryTag.ChangeWeapon : _battleStateMachine.SetState<BattleMovementState>(); break;
                case EntryTag.Movement : _battleStateMachine.SetState<BattleMovementState>(); break;
                default : _battleStateMachine.SetState<BattleMovementState>(); break;
            }
        }

        public void Exit()
        {       
            _stateMachineExitListenerDisposable?.Dispose();    
            _battleStateMachine?.Dispose();
        }
    }
}
