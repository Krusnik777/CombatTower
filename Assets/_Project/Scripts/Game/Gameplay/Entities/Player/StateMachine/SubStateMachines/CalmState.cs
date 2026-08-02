using StateMachine;
using DI;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class CalmState : MovementState
    {
        private System.IDisposable _attackListenerDisposable;

        public CalmState(IStateMachine parentStateMachine, Player player, DIContainer sceneContainer) : base(parentStateMachine, player, sceneContainer) { }

        public override void Enter()
        {
            base.Enter();

            _attackListenerDisposable?.Dispose();
            _attackListenerDisposable = _gameInputService.OnAttackPressed.Subscribe(_ => OnAttack());
        }

        public override void Exit()
        {
            _attackListenerDisposable?.Dispose();

            base.Exit();
        }

        private void OnAttack()
        {
            _attackListenerDisposable?.Dispose();

            _parentStateMachine.SetState<BattleState, bool>(true);
        }
    }
}
