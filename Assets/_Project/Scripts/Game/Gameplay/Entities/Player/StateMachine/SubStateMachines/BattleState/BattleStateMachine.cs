using StateMachine;
using DI;
using R3;

namespace CombatTower.Game.Gameplay.Entities.Player
{
    public class BattleStateMachine : AbstractStateMachine
    {
        public Subject<BattleState.ExitTag> OnExit { get; private set; } = new();

        private System.IDisposable _exitStateListenerDisposable;

        public BattleStateMachine(Player player, IStateMachine rootStateMachine, DIContainer sceneContainer)
        {
            var exitState = new BattleExitState(this, player, sceneContainer);

            _states = new()
            {
                [typeof(BattleMovementState)] = new BattleMovementState(this, player, sceneContainer),
                [typeof(AttackState)] = new AttackState(this, player, sceneContainer),
                [typeof(BattleExitState)] = exitState
            };

            _exitStateListenerDisposable = exitState.OnExitSignal.Subscribe(exitTag => OnExit?.OnNext(exitTag));
        }

        public override void Dispose()
        {
            _exitStateListenerDisposable?.Dispose();

            base.Dispose();
        }
    }
}
