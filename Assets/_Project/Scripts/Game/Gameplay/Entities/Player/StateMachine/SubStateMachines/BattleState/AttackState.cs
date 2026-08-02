using StateMachine;
using DI;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class AttackState : IEnterableState
    {
        private const string _attackTrigger = "Attack";

        private IStateMachine _parentStateMachine;
        private DIContainer _sceneContainer;
        private Player _player;
        
        private System.IDisposable _attackFinishListenerDisposable;

        public AttackState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer)
        {
            _parentStateMachine = parentStateMachine;
            _player = player;
            _sceneContainer = sceneContainer;
        }

        public void Enter()
        {
            _attackFinishListenerDisposable?.Dispose();
            _attackFinishListenerDisposable = _player.EventsCollector.OnAttackFinish.Subscribe(OnAttackFinished);

            _player.Movement.IsControlledByRootMotion = true;
            _player.Animator.SetTrigger(_attackTrigger);
        }

        public void Exit()
        {
            _player.Movement.IsControlledByRootMotion = false;

            _attackFinishListenerDisposable?.Dispose();
        }

        private void OnAttackFinished(int attackType)
        {
            _attackFinishListenerDisposable?.Dispose();

            _parentStateMachine.SetState<BattleMovementState>();
        }
    }
}
